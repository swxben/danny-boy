using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.TruncateTests
{
    public class AsyncWithTableName : TruncateTestsBase
    {
        [Test]
        public void AllDataIsTruncated()
        {
            DataAccess.TruncateAsync("Persons").Wait();

            DataAccess.ExecuteScalar<int>("SELECT COUNT(*) FROM Persons")
                .ShouldBe(0);
        }
    }
}