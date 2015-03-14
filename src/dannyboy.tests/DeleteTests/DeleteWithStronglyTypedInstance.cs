using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.DeleteTests
{
    public class DeleteWithStronglyTypedInstance : DeleteTestBase
    {
        [Test]
        public void ItDeletesFromTheAppropriateTable()
        {
            CreateUsersTable();

            var user1 = new User { Id = Guid.NewGuid(), Name = "Trey" };
            var user2 = new User { Id = Guid.NewGuid(), Name = "Mike" };

            DataAccess.Insert(user1);
            DataAccess.Insert(user2);

            DataAccess.Delete(user1);

            DataAccess
                .ExecuteScalar<int>("SELECT COUNT(*) FROM [Users]")
                .ShouldBe(1);
            DataAccess.Select<User>().Single().Name.ShouldBe("Mike");
        }
    }
}