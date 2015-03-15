using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.TruncateTests
{
    public class TruncateTestsBase : DataAccessTestBase
    {
        [SetUp]
        public void SetUp()
        {
            CreatePersonsTable();

            DataAccess.Insert(new Person { Name = "George" });
            DataAccess.Insert(new Person { Name = "Ringo" });
            DataAccess.Insert(new Person { Name = "John" });
            DataAccess.Insert(new Person { Name = "Paul" });

            DataAccess.ExecuteScalar<int>("SELECT COUNT(*) FROM Persons")
                .ShouldBe(4);
        }
    }
}