using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    [TestFixture]
    public class select_with_transform_function
    {
        class Address
        {
            public string Line1;
            public string Line2;
        }
        class Customer
        {
            public string Name;
            public Address BillingAddress;
            public Address ShippingAddress;
        }

        readonly IDataAccess _dataAccess = new DataAccess(TestConfiguration.ConnectionString);

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.DropTable("Customers");
        }

        [Test]
        public void transform_into_complex_graph()
        {
            CreateTable();
            _dataAccess.ExecuteCommand("INSERT INTO Customers VALUES('Test Customer', 'billing 1', 'billing 2', 'shipping 1', 'shipping 2')");

            var customers = _dataAccess.Select<Customer>(transform: r => new Customer
                {
                    Name = (string)r.Name,
                    BillingAddress = new Address { Line1 = (string)r.BillingAddressLine1, Line2 = (string)r.BillingAddressLine2 },
                    ShippingAddress = new Address { Line1 = (string)r.ShippingAddressLine1, Line2 = (string)r.ShippingAddressLine2 }
                }).ToList();

            customers.Count().ShouldBe(1);
            customers.First().Name.ShouldBe("Test Customer");
            customers.First().BillingAddress.Line1.ShouldBe("billing 1");
            customers.First().BillingAddress.Line2.ShouldBe("billing 2");
            customers.First().ShippingAddress.Line1.ShouldBe("shipping 1");
            customers.First().ShippingAddress.Line2.ShouldBe("shipping 2");
        }

        [Test]
        public void with_reusable_transform_method()
        {
            CreateTable();
            _dataAccess.ExecuteCommand("INSERT INTO Customers VALUES('Test Customer', 'billing 1', 'billing 2', 'shipping 1', 'shipping 2')");

            var customers = _dataAccess.Select(GetCustomer).ToList();

            customers.Count().ShouldBe(1);
            customers.First().Name.ShouldBe("Test Customer");
            customers.First().BillingAddress.Line1.ShouldBe("billing 1");
            customers.First().BillingAddress.Line2.ShouldBe("billing 2");
            customers.First().ShippingAddress.Line1.ShouldBe("shipping 1");
            customers.First().ShippingAddress.Line2.ShouldBe("shipping 2");
        }

        Customer GetCustomer(dynamic r)
        {
            return new Customer
                {
                    Name = (string)r.Name,
                    BillingAddress = new Address { Line1 = (string)r.BillingAddressLine1, Line2 = (string)r.BillingAddressLine2 },
                    ShippingAddress = new Address { Line1 = (string)r.ShippingAddressLine1, Line2 = (string)r.ShippingAddressLine2 }
                };
        }

        private void CreateTable()
        {
            _dataAccess.ExecuteCommand(@"
CREATE TABLE Customers(
    Name NVARCHAR(200),
    BillingAddressLine1 NVARCHAR(200),
    BillingAddressLine2 NVARCHAR(200),
    ShippingAddressLine1 NVARCHAR(200),
    ShippingAddressLine2 NVARCHAR(200)
)
                ");
        }
    }
}
