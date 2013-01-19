using System.Linq;
using NUnit.Framework;
using Shouldly;
using swxben.dataaccess;

namespace Tests
{
    [TestFixture]
    public class can_pass_table_name_in_to_insert_update_and_select
    {
        readonly IDataAccess _dataAccess = new DataAccess(TestConfiguration.CONNECTION_STRING);

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.DropTable("Things");
        }

        void SetUpTable()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Things(ColA NVARCHAR(10), ColB NVARCHAR(10))");
        }

        [Test]
        public void insert_sql_is_sane()
        {
            var thing = new { ColA = "aaa", ColB = "bbb" };
            var sql = DataAccess.GetInsertSqlFor(thing.GetType(), "Things");
            sql.ShouldBeCloseTo("INSERT INTO Things(ColA, ColB) VALUES(@ColA, @ColB)");
        }

        [Test]
        public void can_insert()
        {
            SetUpTable();
            _dataAccess.Insert(new { ColA = "aaa", ColB = "bbb" }, "Things");
            _dataAccess.Any("Things").ShouldBe(true);
        }

        [Test]
        public void update_sql_is_sane()
        {
            var thing = new { ColA = "aaa", ColB = "bbb", Id = 1 };
            var sql = DataAccess.GetUpdateSqlFor(thing.GetType(), new[] { "Id" }, "Things");
            sql.ShouldBeCloseTo("UPDATE Things SET ColA = @ColA, ColB = @ColB WHERE 1=1 AND Id = @Id");
        }

        [Test]
        public void can_update()
        {
            SetUpTable();
            _dataAccess.Insert(new { ColA = "aaa", ColB = "bbb" }, "Things");
            _dataAccess.Update(new { ColA = "aaa", ColB = "ccc" }, new[] { "ColA" }, "Things");

            var results = _dataAccess.ExecuteQuery("SELECT * FROM Things");

            results.Count().ShouldBe(1);
            ((string)results.First().ColA).ShouldBe("aaa");
            ((string)results.First().ColB).ShouldBe("ccc");
        }

        [Test]
        public void select_sql_is_sane()
        {
            var sql = DataAccess.GetSelectSqlFor(null, null, null, "Things");
            sql.ShouldBeCloseTo("SELECT * FROM Things");
        }

        [Test]
        public void can_select()
        {
            SetUpTable();
            _dataAccess.Insert(new { ColA = "aaa", ColB = "bbb" }, "Things");
            var things = _dataAccess.Select("Things");
            things.Count().ShouldBe(1);
        }

        class XXXThingXXX
        {
            public string ColA;
            public string ColB;
        }

        [Test]
        public void can_select_strongly_typed()
        {
            SetUpTable();
            _dataAccess.Insert(new { ColA = "aaa", ColB = "bbb" }, "Things");
            var things = _dataAccess.Select<XXXThingXXX>("Things");
            things.Count().ShouldBe(1);
            things.First().GetType().ShouldBe(typeof(XXXThingXXX));
        }
    }
}
