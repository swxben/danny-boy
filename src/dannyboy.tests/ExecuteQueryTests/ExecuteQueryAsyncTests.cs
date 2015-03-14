using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.ExecuteQueryTests
{
    public class ExecuteQueryAsyncTests : DataAccessTestBase
    {
        [Test]
        public void ReturnsSomeResults()
        {
            CreatePersonsTable();

            DataAccess.Insert(new { Name = "George" }, "Persons");
            DataAccess.Insert(new { Name = "Ringo" }, "Persons");
            DataAccess.Insert(new { Name = "Paul" }, "Persons");
            DataAccess.Insert(new { Name = "John" }, "Persons");

            var result = DataAccessAsync
                .ExecuteQueryAsync("SELECT Name FROM Persons WHERE Name LIKE '%ingo%'")
                .Result
                .Single();

            ((string)result.Name).ShouldBe("Ringo");
        }

    }
}
