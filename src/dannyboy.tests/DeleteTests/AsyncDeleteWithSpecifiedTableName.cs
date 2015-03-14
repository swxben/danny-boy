using System;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.DeleteTests
{
    public class AsyncDeleteWithSpecifiedTableName : DeleteTestBase
    {
        [Test]
        public void ItDeletesFromTheAppropriateTable()
        {
            CreateUsersTable();

            var id = Guid.NewGuid();

            DataAccess.Insert(new { Id = id, Name = "Trey" }, "Users");
            DataAccess.Insert(new { Id = Guid.NewGuid(), Name = "Mike" }, "Users");


            DataAccess.DeleteAsync(new { Id = id }, "Users").Wait();

            DataAccess
                .ExecuteScalar<int>("SELECT COUNT(*) FROM [Users]")
                .ShouldBe(1);
        }
    }
}