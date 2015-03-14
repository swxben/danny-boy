using System;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.DeleteTests
{
    public class DeleteWithSpecifiedTableName : DeleteTestBase
    {
        [Test]
        public void ItDeletesFromTheAppropriateTable()
        {
            CreateUsersTable();

            var id = Guid.NewGuid();

            DataAccess.Insert(new { Id = id, Name = "Trey" }, "Users");
            DataAccess.Insert(new { Id = Guid.NewGuid(), Name = "Mike" }, "Users");


            DataAccess.Delete(new { Id = id }, "Users");

            DataAccess
                .ExecuteScalar<int>("SELECT COUNT(*) FROM [Users]")
                .ShouldBe(1);
        }
    }
}