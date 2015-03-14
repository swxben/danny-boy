using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.InsertTests
{
    public class InsertAsyncTests : DataAccessTestBase
    {
        [Test]
        public void InsertsSomeData()
        {
            CreatePersonsTable();

            DataAccess.InsertAsync(new { Name = "George" }, "Persons").Wait();
            DataAccess.InsertAsync(new { Name = "Ringo" }, "Persons").Wait();
            DataAccess.InsertAsync(new { Name = "Paul" }, "Persons").Wait();
            DataAccess.InsertAsync(new { Name = "John" }, "Persons").Wait();

            DataAccess.ExecuteScalar<int>("SELECT COUNT(*) FROM [Persons]").ShouldBe(4);
        }
    }
}
