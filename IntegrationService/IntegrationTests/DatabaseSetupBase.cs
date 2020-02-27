using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace IntegrationService.IntegrationTests
{
    public abstract class DatabaseSetupBase
    {
        private const string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true";
        private const string ConnectionStringTemplate = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database={0}";
        private static readonly Guid _databaseGuid = Guid.NewGuid();
        private static readonly Regex GoStripper = new Regex(@"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);


        protected virtual string ContextConfigurationKey
        {
            get { return "dbconnection"; }
        }

        protected static string DatabaseNameToUse
        {
            get { return "AA_" + _databaseGuid.ToString("N"); }
        }

        protected static string ConnectionStringForObjectCreation
        {
            get { return string.Format(ConnectionStringTemplate, DatabaseNameToUse); }
        }

        public static string ConnectionStringToUse
        {
            get { return ConnectionStringForObjectCreation; }
        }

        protected void SetupFixture()
        {
            //UpdateConnectionString(ContextConfigurationKey);

            using (var sc = new SqlConnection(ConnectionString))
            {
                sc.Open(); //just to register db
                var cmd = sc.CreateCommand();
                cmd.CommandText = string.Format(@"IF db_id('{0}') IS NOT NULL BEGIN    ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;drop database [{0}];END;create database [{0}];", DatabaseNameToUse);
                Console.WriteLine("Creating local database: {0}", DatabaseNameToUse);
               
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //TODO: make a test that will raise inconlusive if this happens to warn us
                    Console.WriteLine("Failed creating {0} database, tests might misbehave", DatabaseNameToUse);
                    Console.WriteLine(ex.ToString());
                    //continue - if we failed to drop the db previously this would've happned
                    TeardownFixture();
                    throw;
                }

                PrepareDatabase();
            }
        }

        internal abstract void PrepareDatabase();

        /*
        protected void UpdateConnectionString(string key)
        {
            //var connectionString = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

            var configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            configFile = configFile.Substring(0, configFile.Length - ".config".Length);
            var cfg = ConfigurationManager.OpenExeConfiguration(configFile);
            cfg.ConnectionStrings.ConnectionStrings[key].ConnectionString = ConnectionStringForObjectCreation;
            cfg.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
        }*/

        protected void TeardownFixture()
        {
            using (var sc = new SqlConnection(ConnectionString))
            {
                sc.Open(); //just to register db
                var cmd = sc.CreateCommand();
                cmd.CommandText =
                    string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;drop database [{0}];",
                        DatabaseNameToUse);
                Console.WriteLine("Dropping local database: {0}", DatabaseNameToUse);
                cmd.ExecuteNonQuery();
            }
        }

        

        protected void RunScript(string fileName)
        {
            Console.WriteLine("Running script: " + fileName);

            using (var con = new SqlConnection(ConnectionStringForObjectCreation))
            {
                con.Open();
                con.InfoMessage += (sender, args) => Console.WriteLine(args.Message);
                var cmd = con.CreateCommand();
                cmd.CommandText = "sp_executesql @0";
                cmd.Parameters.AddWithValue("@0", GetSqlFromFile(fileName));
                cmd.ExecuteNonQuery();
            }
        }

        protected void ExecuteQuery(string sql)
        {
            using (var con = new SqlConnection(ConnectionStringForObjectCreation))
            {
                con.Open();
                con.InfoMessage += (sender, args) => Console.WriteLine(args.Message);
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        protected void RunSplitScriptFromString(string sqlString)
        {
            foreach (var sql in GetSqlSplitOnGoFromString(sqlString))
            {
                using (var con = new SqlConnection(ConnectionStringForObjectCreation))
                {
                    con.Open();
                    con.InfoMessage += (sender, args) => Console.WriteLine(args.Message);
                    var cmd = con.CreateCommand();
                    cmd.CommandText = "sp_executesql @0";
                    cmd.Parameters.AddWithValue("@0", sql);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// This method is splitting file on GO statement and executing them independently
        /// </summary>
        /// <param name="fileName"></param>
        protected void RunSplitScript(string fileName)
        {
            Console.WriteLine("Running script: " + fileName);

            foreach (var sql in GetSqlSplitOnGoFromFile(fileName))
            {
                using (var con = new SqlConnection(ConnectionStringForObjectCreation))
                {
                    con.Open();
                    con.InfoMessage += (sender, args) => Console.WriteLine(args.Message);
                    var cmd = con.CreateCommand();
                    cmd.CommandText = "sp_executesql @0";
                    cmd.Parameters.AddWithValue("@0", sql);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private IEnumerable<string> GetSqlSplitOnGoFromFile(string fileName)
        {
            using (var sr = new StreamReader(fileName))
            {
                return GoStripper.Split(sr.ReadToEnd());
            }
        }

        private IEnumerable<string> GetSqlSplitOnGoFromString(string query)
        {
            return GoStripper.Split(query);
        }

        private static string GetSqlFromFile(string filename)
        {
            using (var sr = new StreamReader(filename))
            {
                return GoStripper.Replace(sr.ReadToEnd(), "");
            }
        }
    }
}
