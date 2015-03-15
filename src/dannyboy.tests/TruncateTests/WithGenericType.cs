using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.TruncateTests
{
    public class WithGenericType : TruncateTestsBase
    {
        [Test]
        public void AllDataIsTruncated()
        {
            DataAccess.Truncate<Person>();

            DataAccess.ExecuteScalar<int>("SELECT COUNT(*) FROM Persons")
                .ShouldBe(0);
        }
    }
}