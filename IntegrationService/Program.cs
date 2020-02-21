using IntegrationService.IntegrationTests;
using System;

namespace IntegrationService
{
    class Program
    {
        static void Main(string[] args)
        {
            IntegrationTestBase testBase = new IntegrationTestBase();
            testBase.CreateDatabase();
            testBase.RemoveDatabase();
        }
    }
}
