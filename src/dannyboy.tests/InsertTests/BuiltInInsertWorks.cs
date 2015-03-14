using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.InsertTests
{
    [TestFixture]
    public class BuiltInInsertWorks
    {
        readonly IDataAccess _dataAccess = new DataAccess(TestConfiguration.ConnectionString);

        class Customer
        {
            public int CustomerId;
            public string CustomerName;
        }

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.DropTable("Customers");
        }

        [Test]
        public void perform_an_insert()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Customers(CustomerId INT NOT NULL, CustomerName NVARCHAR(200) NOT NULL)");
            _dataAccess.Insert(new Customer { CustomerId = 2, CustomerName = "Inserted customer" });

            _dataAccess
                .ExecuteQuery<Customer>("SELECT * FROM Customers")
                .First()
                .CustomerName
                .ShouldBe("Inserted customer");
        }
    }
}
