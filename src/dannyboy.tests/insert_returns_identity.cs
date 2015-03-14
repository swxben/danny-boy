using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    [TestFixture]
    public class insert_returns_identity
    {
        readonly IDataAccess _dataAccess = new DataAccess(TestConfiguration.ConnectionString);

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.DropTable("Things");
        }

        void CreateTable()
        {
            _dataAccess.ExecuteCommand(@"
CREATE TABLE Things(
    ThingId INT IDENTITY(1,1),
    ThingName NVARCHAR(20)
)");
        }

        [Test]
        public void insert_a_couple_of_things()
        {
            CreateTable();

            _dataAccess.Insert(new { ThingName = "one" }, "Things");
            _dataAccess.Insert(new { ThingName = "two" }, "Things");
            var id = _dataAccess.Insert(new { ThingName = "three" }, "Things");

            var three = _dataAccess.Select("Things", where: new { ThingId = id }).First();

            ((string)three.ThingName).ShouldBe("three");
        }

        [Test]
        public void insert_and_delete_a_couple_of_things()
        {
            CreateTable();

            var id1 = _dataAccess.Insert(new { ThingName = "one" }, "Things");
            var id2 = _dataAccess.Insert(new { ThingName = "two" }, "Things");
            var id3 = _dataAccess.Insert(new { ThingName = "three" }, "Things");
            var id4 = _dataAccess.Insert(new { ThingName = "four" }, "Things");
            var id5 = _dataAccess.Insert(new { ThingName = "five" }, "Things");

            _dataAccess.ExecuteCommand("DELETE FROM Things WHERE ThingId = @id", new { id = id2 });
            _dataAccess.ExecuteCommand("DELETE FROM Things WHERE ThingId = @id", new { id = id4 });

            var id6 = _dataAccess.Insert(new { ThingName = "six" }, "Things");

            var six = _dataAccess.Select("Things", where: new { ThingId = id6 }).First();

            ((string)six.ThingName).ShouldBe("six");
        }

        [Test]
        public void insert_return_should_be_castable_to_int()
        {
            CreateTable();

            var id = (int)_dataAccess.Insert(new { ThingName = "one" }, "Things");
            id.ShouldBeOfType<int>();
        }
    }
}
