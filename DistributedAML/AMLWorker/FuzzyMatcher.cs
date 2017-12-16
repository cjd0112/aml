using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using As.Comms;
using As.Comms.ClientServer;
using As.GraphDB.Sql;
using As.Logger;
using Microsoft.Data.Sqlite;

namespace AMLWorker
{
    public class FuzzyMatcher : FuzzyMatcherServer
    {
        string FuzzyPhraseCreate = "create table FuzzyPhrase (id integer primary key,phrase text,unique(phrase));";
        string FuzzyTripleCreate = "create virtual table FuzzyTriple using fts4(triple,phrase,notindexed=phrase);";
        string FuzzyPhraseToDocument = "create table FuzzyPhraseToDocument (phraseid integer, documentid integer, primary key(phraseid,documentid), foreign key (phraseid) references FuzzyPhrase(id));";

        private string connectionString;
        public FuzzyMatcher(IServiceServer server) : base(server)
        {
            
            L.Trace(
                $"Opened server with bucket {server.BucketId} and data dir - {server.GetConfigProperty("DataDirectory",server.BucketId)}");

            connectionString = SqlTableHelper.GetConnectionString((string)server.GetConfigProperty("DataDirectory", server.BucketId),
                server.BucketId, "AmlWorker");
            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = newConnection())
            {
                connection.Open();

                if (!SqlTableHelper.TableExists(connection,"FuzzyPhrase"))
                {
                    SqlTableHelper.ExecuteCommandLog(connection, FuzzyPhraseCreate);
                    SqlTableHelper.ExecuteCommandLog(connection, FuzzyTripleCreate);
                    SqlTableHelper.ExecuteCommandLog(connection, FuzzyPhraseToDocument);

                }
            }
        }

        SqliteConnection newConnection()
        {
            return new SqliteConnection(
                "" + new SqliteConnectionStringBuilder {DataSource = $"{connectionString}"});
        }


        public override int AddEntry(IEnumerable<FuzzyWordEntry> entries)
        {
            try
            {
                var s = new Stopwatch();
                s.Start();
                L.Trace($"FuzzyMatcher - {this.server.BucketId} - hit add entry with {entries.Count()} at {DateTime.Now}");
                using (var connection = newConnection())
                {
                    connection.Open();
                    var txn = connection.BeginTransaction();

                    int cnt = 0;
                    foreach (var c in entries)
                    {
                        using (var existsCmd = connection.CreateCommand())
                        {
                            existsCmd.CommandText = "select rowid from FuzzyPhrase where Phrase=($phrase)";

                            existsCmd.Parameters.AddWithValue("$phrase", c.Phrase);
                            var exists = existsCmd.ExecuteReader();
                            if (exists.HasRows)
                            {
                                using (var insert1Cmd = connection.CreateCommand())
                                {
                                    insert1Cmd.Transaction = txn;
                                    insert1Cmd.CommandText =
                                        "insert into FuzzyPhraseToDocument (phraseid,documentid) values ($phraseid,$documentid);";

                                    exists.Read();
                                    var phraseid = exists.GetInt32(0);

                                    insert1Cmd.Parameters.AddWithValue("$phraseid", phraseid);
                                    insert1Cmd.Parameters.AddWithValue("$documentid", c.DocId);

                                    try
                                    {
                                        var res = insert1Cmd.ExecuteNonQuery();
                                        if (res != 1)
                                            throw new Exception($"Could not update existing phrase - {c.Phrase}");
                                    }
                                    catch (SqliteException)
                                    {
                                        //                                L.Trace(e.Message);
                                    }

                                }
                            }
                            else
                            {
                                using (var insert3Cmd = connection.CreateCommand())
                                {
                                    insert3Cmd.Transaction = txn;
                                    insert3Cmd.CommandText =
                                        $@"insert into FuzzyPhrase (phrase) values ($phrase);
                                    insert into FuzzyPhraseToDocument (phraseid,documentid) values (last_insert_rowid(),$documentid);
                                    insert into FuzzyTriple (triple,phrase) values ($triple,$phrase);";

                                    insert3Cmd.Parameters.AddWithValue("$phrase", c.Phrase);
                                    insert3Cmd.Parameters.AddWithValue("$documentid", c.DocId);
                                    insert3Cmd.Parameters.AddWithValue("$triple",
                                        StringFunctions.CreateTriple(c.Phrase));

                                    var row = insert3Cmd.ExecuteNonQuery();
                                    if (row != 3)
                                        throw new Exception($"Could not insert row .. {c.Phrase}");

                                    insert3Cmd.Dispose();
                                }
                            }
                        }
                        cnt++;
                        if (cnt % 10000 == 0)
                        {
                            txn.Commit();
                            txn = connection.BeginTransaction();
                        }

                    }

                    txn.Commit();
                    s.Stop();
                    L.Trace($"Committed from FuzzyMatcher - {server.BucketId} @ {DateTime.Now} - op took - {s.ElapsedMilliseconds}ms");
                    return 1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public override IEnumerable<FuzzyQueryResponse> FuzzyQuery(IEnumerable<FuzzyCheck> phrases)
        {
            var fqr = new List<FuzzyQueryResponse>();
            using (var connection = newConnection())
            {
                connection.Open();

                foreach (var phrase in phrases)
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        FuzzyQueryResponse fqr2 = new FuzzyQueryResponse();
                        fqr2.Query = phrase.Phrase;
                        cmd.CommandText =
                            $@"select docid,phrase,matchinfo(FuzzyTriple,""s"") from FuzzyTriple where triple MATCH '{StringFunctions.TripleQuery(phrase.Phrase)}'";

                        var p = cmd.ExecuteReader();
                        int cnt = 0;

                        while (p.Read())
                        {
                            var docid = (Int64) p.GetValue(0);
                            var index_phrase = (string) p.GetValue(1);
                            var matchinfo = p.GetValue(2) as byte[];
                            var p2 = StringFunctions.LevensteinDistance(index_phrase, phrase.Phrase);
                            fqr2.Detail.Add(new FuzzyQueryResponseDetail{Candidate=index_phrase,Score=p2,PhraseId=docid});
                            cnt++;
                        }
                        fqr.Add(fqr2);
                    }
                }
            }
            return fqr;
        }

    }
}
