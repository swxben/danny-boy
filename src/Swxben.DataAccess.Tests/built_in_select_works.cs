using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using swxben.dataaccess;
using Shouldly;

namespace Tests
{
    [TestFixture]
    public class built_in_select_works
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
        public void can_select_all_rows()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Customers(CustomerId INT NOT NULL, CustomerName NVARCHAR(200) NOT NULL)");
            _dataAccess.Insert(new Customer { CustomerId = 1, CustomerName = "Test 1" });
            _dataAccess.Insert(new Customer { CustomerId = 2, CustomerName = "Test 2" });
            _dataAccess.Insert(new Customer { CustomerId = 3, CustomerName = "Test 3" });

            _dataAccess
                .Select<Customer>()
                .Count()
                .ShouldBe(3);
        }

        [Test]
        public void simple_criteria_works()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Customers(CustomerId INT NOT NULL, CustomerName NVARCHAR(200) NOT NULL)");
            _dataAccess.Insert(new Customer { CustomerId = 1, CustomerName = "Test 1" });
            _dataAccess.Insert(new Customer { CustomerId = 2, CustomerName = "A customer" });
            _dataAccess.Insert(new Customer { CustomerId = 3, CustomerName = "A customer" });
            _dataAccess.Insert(new Customer { CustomerId = 4, CustomerName = "Test 4" });

            _dataAccess
                .Select<Customer>(new { CustomerName = "A customer" })
                .Count()
                .ShouldBe(2);
        }

        [Test]
        public void multiple_criteria_works()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Customers(CustomerId INT NOT NULL, CustomerName NVARCHAR(200) NOT NULL)");
            _dataAccess.Insert(new Customer { CustomerId = 1, CustomerName = "Test 1" });
            _dataAccess.Insert(new Customer { CustomerId = 2, CustomerName = "A customer" });
            _dataAccess.Insert(new Customer { CustomerId = 2, CustomerName = "A customer" });
            _dataAccess.Insert(new Customer { CustomerId = 2, CustomerName = "A customer" });
            _dataAccess.Insert(new Customer { CustomerId = 2, CustomerName = "Test 2" });
            _dataAccess.Insert(new Customer { CustomerId = 3, CustomerName = "Test 3" });

            var results = _dataAccess
                .Select<Customer>(new
                {
                    CustomerId = 2,
                    CustomerName = "A customer",
                }).ToList();

            results.Count().ShouldBe(3);

            results[0].CustomerId.ShouldBe(2);
            results[1].CustomerId.ShouldBe(2);
            results[2].CustomerId.ShouldBe(2);

            results[0].CustomerName.ShouldBe("A customer");
            results[1].CustomerName.ShouldBe("A customer");
            results[2].CustomerName.ShouldBe("A customer");
        }

        [Test]
        public void order_by_works()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Customers(CustomerId INT NOT NULL, CustomerName NVARCHAR(200) NOT NULL)");
            _dataAccess.Insert(new Customer { CustomerId = 1, CustomerName = "aaa" });
            _dataAccess.Insert(new Customer { CustomerId = 2, CustomerName = "bbb" });
            _dataAccess.Insert(new Customer { CustomerId = 3, CustomerName = "ggg" });
            _dataAccess.Insert(new Customer { CustomerId = 4, CustomerName = "eee" });
            _dataAccess.Insert(new Customer { CustomerId = 5, CustomerName = "ccc" });
            _dataAccess.Insert(new Customer { CustomerId = 6, CustomerName = "hhh" });

            var results = _dataAccess.Select<Customer>(orderBy: "CustomerName").ToList();

            results[0].CustomerName.ShouldBe("aaa");
            results[0].CustomerId.ShouldBe(1);
            results[1].CustomerName.ShouldBe("bbb");
            results[1].CustomerId.ShouldBe(2);
            results[2].CustomerName.ShouldBe("ccc");
            results[2].CustomerId.ShouldBe(5);
            results[3].CustomerName.ShouldBe("eee");
            results[3].CustomerId.ShouldBe(4);
            results[4].CustomerName.ShouldBe("ggg");
            results[4].CustomerId.ShouldBe(3);
            results[5].CustomerName.ShouldBe("hhh");
            results[5].CustomerId.ShouldBe(6);
        }
    }
}
