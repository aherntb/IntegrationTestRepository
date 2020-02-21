using System;
using System.Data.SqlClient;


namespace IntegrationService
{
    public class DabaseSetup
    {
        private static Guid _databaseUniqueId = Guid.NewGuid();
        private string _databaseName = string.Concat("IT_", _databaseUniqueId.ToString().Replace("-", ""));
        private string _connectionString = @"Server=BRIANPC;Database=master;Trusted_Connection=True;";

        public void CreateDatabase()
        {
            string queryString = string.Format(@"CREATE DATABASE {0};", _databaseName);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, sqlConnection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void RemoveDatabase()
        {
            string queryString = string.Format("DROP DATABASE {0}", _databaseName);
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, sqlConnection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
