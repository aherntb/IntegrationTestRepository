using System.Data.SqlClient;

namespace NUnit_Test_Project.IntegrationSetup
{
    public static class SQLCommands
    {
        public static string DropIfExistsAndCreateDatabase  = @"IF db_id('{0}') IS NOT NULL BEGIN    ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;drop database [{0}];END;create database [{0}];";
        public static string DropDatabase = @"IF db_id('{0}' IS NOT NULL BEGIN ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;drop database [{0}]; END;";
        public static string CreateTableCustomer = @"CREATE TABLE Customer ( customer_pkid INT NOT NUL IDENTITY(1,1), first_name VARCHAR(25), last_name VARCHAR(25));";
        public static string InsertCustomerBrian = @"INSERT INTO Customer VALUES ('Brian','Ahern')";
        public static string SelectCustomerAll = @"SELECT * FROM Customer";

    }

}
