//using System;
//using System.Collections.Generic;
//using System.Text;

using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System.Data;

namespace MySqlApp.Data.Connection
{
    internal sealed class ConnectionToDb
    {
        private static ConnectionToDb _instance;
        private static readonly object _lock = new object();
        private string _connectionString;

        private ConnectionToDb()
        {
            // --- CONFIGURATION ---
            string SERVER_NAME = "CMDB-224899";
            string DB_NAME = "TestDb";

            if (string.IsNullOrWhiteSpace(SERVER_NAME))
            { 
                throw new InvalidOperationException("SERVER_NAME is not set.");
            }
            if (string.IsNullOrWhiteSpace(DB_NAME))
            {
                throw new InvalidOperationException("DB_NAME is not set.");
            }

            _connectionString = $"Server={SERVER_NAME};Database={DB_NAME};Trusted_Connection=True;TrustServerCertificate=True;";
            TestContext.Progress.WriteLine(">>>Start connecting to database..");
        }

        public static ConnectionToDb Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ConnectionToDb();
                    }
                    return _instance;
                }
            }
        }


        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
