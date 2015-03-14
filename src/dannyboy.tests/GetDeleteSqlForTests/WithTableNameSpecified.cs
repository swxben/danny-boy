using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.GetDeleteSqlForTests
{
    public class WithTableNameSpecified
    {
        [Test]
        public void ItIsValid()
        {
            DataAccess
                .GetDeleteSqlFor(typeof (Foo), new {Id = "aabbcc"}, "Bar")
                .ShouldBe("DELETE FROM [Bar] WHERE 1=1 AND [Id] = @Id");
        }

        private class Foo
        {
        }
    }
}