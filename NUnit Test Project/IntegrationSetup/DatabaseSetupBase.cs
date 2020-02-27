using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace NUnit_Test_Project.IntegrationSetup
{
    public abstract class DatabaseSetupBase
    {
        private const string _connectionStringToServer = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true";
        private const string _connectionStringTemplate = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database={0}";
        private static readonly Guid _databaseGuid = Guid.NewGuid();
        private static readonly Regex _goStripper = new Regex(@"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        protected static string DatabaseNameToUse
        {
            get { return "IntegrationDB_" + _databaseGuid.ToString("N"); }
        }

        protected static string ConnectionStringToDatabase
        {
            get { return string.Format(_connectionStringTemplate, DatabaseNameToUse); }
        }

        //
        // Implemented in derived class
        //
        internal abstract void PrepareDatabase(); 

        protected void SetupFixture()
        {
            using (var sqlConn = new SqlConnection(_connectionStringToServer))
            {
                sqlConn.Open();
                var sqlCmd = sqlConn.CreateCommand();
                sqlCmd.CommandText = string.Format(SQLCommands.DropIfExistsAndCreateDatabase, DatabaseNameToUse);

                try
                {
                    sqlCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed creating {0} database, tests might misbehave", DatabaseNameToUse);
                    Console.WriteLine(ex.ToString());
                    TearDownFixture();
                    throw;
                }

                PrepareDatabase();
            }
        }

        protected void TearDownFixture()
        {
            using (var sqlConn = new SqlConnection(_connectionStringToServer))
            {
                sqlConn.Open();
                var sqlCommand = sqlConn.CreateCommand();
                sqlCommand.CommandText = string.Format(SQLCommands.DropDatabase, DatabaseNameToUse);
                sqlCommand.ExecuteNonQuery();
                
            }
        }

        protected void ExecuteQuery(string sql)
        {
            using (var sqlConn = new SqlConnection(ConnectionStringToDatabase))
            {
                sqlConn.Open();
                sqlConn.InfoMessage += (sender, args) => Console.WriteLine(args.Message);
                var cmd = sqlConn.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

    }
}
