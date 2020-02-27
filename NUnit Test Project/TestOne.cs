using NUnit.Framework;
using NUnit_Test_Project.IntegrationSetup;

namespace NUnit_Test_Project
{
    public class TestOne :IntegrationTestBase
    {
        [Test]
        public void Test1()
        {
            //arrange 
            ExecuteQuery(SQLCommands.InsertCustomerBrian);

            //act 
            var result = ExecuteRead(SQLCommands.SelectCustomerAll);

            //assert
            Assert.IsNotNull(result, "Result is null");
            ForceCommit();
        }
    }
}