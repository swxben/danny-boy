using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.TruncateTests
{
    public class AsyncWithGenericType : TruncateTestsBase
    {
        [Test]
        public void AllDataIsTruncated()
        {
            DataAccess.TruncateAsync<Person>().Wait();

            DataAccess.ExecuteScalar<int>("SELECT COUNT(*) FROM Persons")
                .ShouldBe(0);
        }
    }
}