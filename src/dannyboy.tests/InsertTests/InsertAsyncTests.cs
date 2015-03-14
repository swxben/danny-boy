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

            DataAccessAsync.InsertAsync(new { Name = "George" }, "Persons").Wait();
            DataAccessAsync.InsertAsync(new { Name = "Ringo" }, "Persons").Wait();
            DataAccessAsync.InsertAsync(new { Name = "Paul" }, "Persons").Wait();
            DataAccessAsync.InsertAsync(new { Name = "John" }, "Persons").Wait();

            DataAccess.ExecuteScalar<int>("SELECT COUNT(*) FROM [Persons]").ShouldBe(4);
        }
    }
}
