using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    [TestFixture]
    public class built_in_insert_works
    {
        IDataAccess _dataAccess = new DataAccess(@"Server=.\sqlexpress; Database=swxben_dataaccess; User Id=sa; Password=test;");

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
