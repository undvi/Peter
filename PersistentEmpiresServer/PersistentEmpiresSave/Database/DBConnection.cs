using Dapper;
using MySqlConnector;
using PersistentEmpiresLib.Database.DBEntities;
using PersistentEmpiresSave.Database.Helpers;
using System;
using System.Data;
using System.Data.Common;

namespace PersistentEmpiresSave.Database
{
    public class DBConnection
    {
        private static readonly object ConnectionLock = new object();
        private static string _connectionString = string.Empty;
        private static MySqlConnection _connection;

        public static DbConnection Connection
        {
            get
            {
                EnsureConnection();
                return _connection;
            }
        }

        public static void InitializeSqlConnection(string DbConnectionString)
        {
            SqlMapper.AddTypeHandler(new JsonTypeHandler<AffectedPlayer[]>());
            lock (ConnectionLock)
            {
                _connection?.Dispose();
                _connection = null;
                _connectionString = DbConnectionString;
            }
        }

        public static void ExecuteDapper(string query, object param)
        {
            Connection.Execute(query, param);
        }

        private static void EnsureConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("SQL connection has not been initialised.");
            }

            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                return;
            }

            lock (ConnectionLock)
            {
                if (_connection != null)
                {
                    if (_connection.State == System.Data.ConnectionState.Open)
                    {
                        return;
                    }

                    _connection.Dispose();
                    _connection = null;
                }

                _connection = new MySqlConnection(_connectionString);
                _connection.Open();
            }
        }
    }
}
