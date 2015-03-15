using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.TruncateTests
{
    public class WithTableName : TruncateTestsBase
    {
        [Test]
        public void AllDataIsTruncated()
        {
            DataAccess.Truncate("Persons");

            DataAccess.ExecuteScalar<int>("SELECT COUNT(*) FROM Persons")
                .ShouldBe(0);
        }
    }
}