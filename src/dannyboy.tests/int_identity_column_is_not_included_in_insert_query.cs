using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    [TestFixture]
    class int_identity_column_is_not_included_in_insert_query
    {
        class Thing
        {
            [DataAccess.Identifier]
            public int ThingId { get; set; }
            public string ThingName { get; set; }
        }

        readonly IDataAccess _dataAccess = new DataAccess(TestConfiguration.CONNECTION_STRING);

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.DropTable("Things");
        }

        [Test]
        public void test_sql()
        {
            DataAccess
                .GetInsertSqlFor(typeof(Thing))
                .ShouldBeCloseTo("INSERT INTO Things(ThingName) VALUES (@ThingName)");
        }

        [Test]
        public void test_actually_inserting()
        {
            _dataAccess.ExecuteCommand(@"
CREATE TABLE Things(
    ThingId INT IDENTITY(1,1),
    ThingName NVARCHAR(20)
)");

            var thing = new Thing { ThingName = "test" };
            thing.ThingId = (int)_dataAccess.Insert(thing);

            var retrievedThing = _dataAccess.Select<Thing>(where: new { thing.ThingId }).First();

            retrievedThing.ThingId.ShouldBe(thing.ThingId);
            retrievedThing.ThingName.ShouldBe(thing.ThingName);
        }
    }
}
