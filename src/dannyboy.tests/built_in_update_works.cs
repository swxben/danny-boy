using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    [TestFixture]
    public class built_in_update_works
    {
        readonly IDataAccess _dataAccess = new DataAccess(@"Server=.\sqlexpress; Database=swxben_dataaccess; User Id=sa; Password=test;");

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
        public void perform_an_update()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Customers(CustomerId INT NOT NULL, CustomerName NVARCHAR(200) NOT NULL)");
            _dataAccess.ExecuteCommand(
                "INSERT INTO Customers(CustomerId, CustomerName) VALUES(@CustomerId, @CustomerName)",
                new Customer { CustomerId = 2, CustomerName = "Customer" });
            _dataAccess.Update(
                new Customer { CustomerId = 2, CustomerName = "Customer updated" },
                new[] { "CustomerId" });

            _dataAccess
                .ExecuteQuery<Customer>("SELECT * FROM Customers")
                .First()
                .CustomerName
                .ShouldBe("Customer updated");
        }
    }
}
