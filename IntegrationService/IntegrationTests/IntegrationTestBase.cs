using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace IntegrationService.IntegrationTests
{
    public abstract class IntegrationDbTestsBase : IDisposable
    {

        private TransactionScope currentScope;

        [SetUp]
        public void SetUp()
        {
            //NOTE: this will be used to rollback changes
            currentScope = new TransactionScope();
        }

        [TearDown]
        public void TearDown()
        {
            if (currentScope != null)
                currentScope.Dispose(); //we don't want to complete it - this will roll back our changes
        }

        /// <summary>
        /// Forces the commit. This should only be used for debugging an issue with data in databases
        /// To use it:
        /// Run tests in debug mode. Call this when you want data to be saved and after that stop tests, take a note of database name before stopping.
        /// Stopping tests will leave SQL database file in place. To access the database open SQL Management studio and connect to the following server:
        /// (localdb)\MSSQLLocalDB
        /// use windows security.
        /// Call use [whatever_the_database_name_that_you_noted]
        /// Do whatever query you like now.
        /// </summary>
        protected void ForceCommit()
        {
            currentScope.Complete();
            currentScope.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (currentScope != null)
                {
                    currentScope.Dispose();
                }
            }
        }


        protected string GetConnectionString()
        {
            return DatabaseSetup.ConnectionStringToUse;
        }

        protected void ExecuteQuery(string sql)
        {
            using (var con = new SqlConnection(DatabaseSetupBase.ConnectionStringToUse))
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        protected List<Dictionary<string, string>> ExecuteRead(string sql)
        {
            var result = new List<Dictionary<string, string>>();

            using (var con = new SqlConnection(DatabaseSetup.ConnectionStringToUse))
            {
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = sql;
                using (var dr = cmd.ExecuteReader())
                {
                    var cnt = dr.FieldCount;
                    var fields = Enumerable.Range(0, cnt).Select(x => dr.GetName(x).ToUpperInvariant()).ToList();
                    while (dr.Read())
                    {
                        result.Add(fields.ToDictionary(x => x, x => dr[x].ToString()));
                    }
                }
            }

            return result;
        }
    }
}
