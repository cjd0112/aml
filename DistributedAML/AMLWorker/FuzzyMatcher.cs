using System;
using System.Collections.Generic;
using System.Diagnostics;
using Comms;
using Logger;
using Microsoft.Data.Sqlite;
using Shared;

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
                $"Opened server with bucket {server.BucketId} and data dir - {server.GetConfigProperty("DataDirectory")}");

            connectionString = (string)server.GetConfigProperty("DataDirectory") +
                                   $"/FuzzyMatcher_{server.BucketId}";
            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = newConnection())
            {
                connection.Open();

                if (!SqlHelper.TableExists(connection,"FuzzyPhrase"))
                {
                    SqlHelper.ExecuteCommandLog(connection, FuzzyPhraseCreate);
                    SqlHelper.ExecuteCommandLog(connection, FuzzyTripleCreate);
                    SqlHelper.ExecuteCommandLog(connection, FuzzyPhraseToDocument);

                }
            }
        }

        SqliteConnection newConnection()
        {
            return new SqliteConnection(
                "" + new SqliteConnectionStringBuilder {DataSource = $"{connectionString}"});
        }


        public override bool AddEntry(List<FuzzyWordEntry> entries)
        {
            try
            {
                var s = new Stopwatch();
                s.Start();
                L.Trace($"FuzzyMatcher - {this.server.BucketId} - hit add entry with {entries.Count} at {DateTime.Now}");
                using (var connection = newConnection())
                {
                    connection.Open();
                    var txn = connection.BeginTransaction();

                    foreach (var c in entries)
                    {
                        var existsCmd = connection.CreateCommand();
                        existsCmd.CommandText = "select rowid from FuzzyPhrase where Phrase=($phrase)";

                        existsCmd.Parameters.AddWithValue("$phrase", c.Phrase);
                        var exists = existsCmd.ExecuteReader();
                        if (exists.HasRows)
                        {
                            var insert1Cmd = connection.CreateCommand();
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
                            catch (SqliteException )
                            {
//                                L.Trace(e.Message);
                            }
                        }
                        else
                        {
                            var insert3Cmd = connection.CreateCommand();
                            insert3Cmd.Transaction = txn;
                            insert3Cmd.CommandText =
                                $@"insert into FuzzyPhrase (phrase) values ($phrase);
                    insert into FuzzyPhraseToDocument (phraseid,documentid) values (last_insert_rowid(),$documentid);
                    insert into FuzzyTriple (triple,phrase) values ($triple,$phrase);";

                            insert3Cmd.Parameters.AddWithValue("$phrase", c.Phrase);
                            insert3Cmd.Parameters.AddWithValue("$documentid", c.DocId);
                            insert3Cmd.Parameters.AddWithValue("$triple", StringFunctions.CreateTriple(c.Phrase));

                            var row = insert3Cmd.ExecuteNonQuery();
                            if (row != 3)
                                throw new Exception($"Could not insert row .. {c.Phrase}");
                        }

                    }

                    txn.Commit();
                    s.Stop();
                    L.Trace($"Committed from FuzzyMatcher - {server.BucketId} @ {DateTime.Now} - op took - {s.ElapsedMilliseconds}ms");
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public override List<FuzzyQueryResponse> FuzzyQuery(List<string> phrases)
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
                        fqr2.Query = phrase;
                        cmd.CommandText =
                            $@"select docid,phrase,matchinfo(FuzzyTriple,""s"") from FuzzyTriple where triple MATCH '{StringFunctions.TripleQuery(phrase)}'";

                        var p = cmd.ExecuteReader();
                        int cnt = 0;

                        while (p.Read())
                        {
                            var docid = (Int64) p.GetValue(0);
                            var index_phrase = (string) p.GetValue(1);
                            var matchinfo = p.GetValue(2) as byte[];
                            var p2 = StringFunctions.LevensteinDistance(index_phrase, phrase);
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
