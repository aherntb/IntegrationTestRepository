using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace NUnit_Test_Project.IntegrationSetup
{
    public class IntegrationTestBase : IDisposable
    {
        private TransactionScope _currentScope;

        [SetUp]
        public void SetUp()
        {
            
            _currentScope = new TransactionScope();
        }


        [TearDown]
        public void TearDown()
        {
            if(_currentScope != null)
            {
                _currentScope.Dispose(); // Changes rolled back here
            }
        }

        //
        // This can be called from within the test to persist the transaction for querying in Management Studio
        //
        protected void ForceCommit()
        {
            _currentScope.Complete();
            _currentScope.Dispose();
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
                if (_currentScope != null)
                {
                    _currentScope.Dispose();
                }
            }
        }

        protected void ExecuteQuery(string sql)
        {
            using (var sqlConn = new SqlConnection(DatabaseSetup.DatabaseConnectionString))
            {
                sqlConn.Open();
                sqlConn.InfoMessage += (sender, args) => Console.WriteLine(args.Message);
                var cmd = sqlConn.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        protected List<Dictionary<string, string>> ExecuteRead(string sql)
        {
            var result = new List<Dictionary<string, string>>();

            using (var sqlConn = new SqlConnection(DatabaseSetup.DatabaseConnectionString))
            {
                sqlConn.Open();
                var cmd = sqlConn.CreateCommand();
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
