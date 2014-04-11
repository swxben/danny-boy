using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    [TestFixture]
    public class nullable_datetime_works
    {
        readonly IDataAccess _dataAccess = new DataAccess(TestConfiguration.ConnectionString);

        class Customer
        {
            public int CustomerId;
            public string CustomerName;
            public DateTime? Dob;
        }

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.DropTable("Customers");
        }

        [Test]
        public void perform_an_insert_with_non_null_date()
        {
            CreateTable();
            _dataAccess.Insert(new Customer
            {
                CustomerId = 2,
                CustomerName = "Inserted customer",
                Dob = new DateTime(2012, 01, 01)
            });

            _dataAccess
                .ExecuteQuery<Customer>("SELECT * FROM Customers")
                .First()
                .Dob
                .ShouldBe(new DateTime(2012, 01, 01));
        }

        [Test]
        public void perform_an_insert_with_implicit_null_date()
        {
            CreateTable();
            _dataAccess.Insert(new Customer
            {
                CustomerId = 2,
                CustomerName = "Customer"
            });

            _dataAccess.Select<Customer>().First().Dob.ShouldBe(default(DateTime?));
        }

        private void CreateTable()
        {
            _dataAccess.ExecuteCommand(@"
CREATE TABLE Customers(
    CustomerId INT NOT NULL, 
    CustomerName NVARCHAR(200) NOT NULL, 
    Dob DATETIME)");
        }
    }
}
