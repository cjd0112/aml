﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using AMLWorker.Sql;
using As.Comms;
using As.Comms.ClientServer;
using Fasterflect;
using Google.Protobuf;
using GraphQL;
using As.GraphQL.Interface;
using As.Logger;
using Microsoft.Data.Sqlite;
using As.Shared;

namespace AMLWorker
{
    public class A4ARepository : A4ARepositoryServer
    {
        private string connectionString;
        
        private SqlitePropertiesAndCommands<A4ACategory> categorySql = new SqlitePropertiesAndCommands<A4ACategory>("Categories");
        private SqlitePropertiesAndCommands<A4AMessage> messageSql = new SqlitePropertiesAndCommands<A4AMessage>("Messages");
        private SqlitePropertiesAndCommands<A4AParty> partySql = new SqlitePropertiesAndCommands<A4AParty>("Parties");
        
        public A4ARepository(IServiceServer server) : base(server)
        {
            connectionString = SqlTableHelper.GetConnectionString(
                (string) server.GetConfigProperty("DataDirectory", server.BucketId),
                server.BucketId, "A4Answers");

            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                if (!SqlTableHelper.TableExists(connection, partySql))
                {
                    SqlTableHelper.CreateTable(connection,partySql);
                }

                if (!SqlTableHelper.TableExists(connection, messageSql))
                {
                    SqlTableHelper.CreateTable(connection,messageSql);
                }


                if (!SqlTableHelper.TableExists(connection, categorySql))
                {
                    SqlTableHelper.CreateTable(connection,categorySql);
                }
            }
        }


        public override GraphResponse RunQuery(GraphQuery query)
        {
            using (var connection = SqlTableHelper.NewConnection(connectionString))
            {
                return new GraphResponse
                {
                    Response = new A4ARepositoryGraphDb(connection,partySql,categorySql, messageSql).Run(query.Query).ToString()
                };
            }
        }
    }
}