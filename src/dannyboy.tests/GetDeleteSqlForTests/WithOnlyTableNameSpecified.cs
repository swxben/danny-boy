using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.GetDeleteSqlForTests
{
    public class WithOnlyTableNameSpecified
    {
        [Test]
        public void ItIsValid()
        {
            DataAccess
                .GetDeleteSqlFor(null, new { Id = "aabbcc" }, "Bar")
                .ShouldBe("DELETE FROM [Bar] WHERE 1=1 AND [Id] = @Id");
        }
    }
}