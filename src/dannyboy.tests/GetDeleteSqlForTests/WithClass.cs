using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.GetDeleteSqlForTests
{
    public class WithClass
    {
        [Test]
        public void ItIsValid()
        {
            DataAccess
                .GetDeleteSqlFor(typeof (User), new {Id = "aabbcc"}, null)
                .ShouldBe("DELETE FROM [Users] WHERE 1=1 AND [Id] = @Id");
        }

        private class User
        {
        }
    }
}