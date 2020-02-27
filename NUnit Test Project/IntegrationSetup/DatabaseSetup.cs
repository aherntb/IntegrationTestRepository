using NUnit.Framework;

namespace NUnit_Test_Project.IntegrationSetup
{
    [SetUpFixture]
    public class DatabaseSetup : DatabaseSetupBase
    {
        public static string DatabaseConnectionString
        {
            get { return DatabaseSetupBase.ConnectionStringToDatabase; }
        }


        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            SetupFixture();
        }

        [OneTimeTearDown]
        public void RunAfterAllTests()
        {
            //base.TeardownFixture();
        }

        internal override void PrepareDatabase()
        {
            ExecuteQuery(SQLCommands.CreateTableCustomer);
        }
    }
}
