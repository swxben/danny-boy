using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace swxben.dannyboy.tests
{
    [TestFixture]
    public class can_use_a_simple_table
    {
        readonly IDataAccess _dataAccess = new DataAccess(TestConfiguration.CONNECTION_STRING);

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.ExecuteCommand(@"
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SimpleTable]') AND type IN (N'U'))
    DROP TABLE SimpleTable
");
        }

        [Test]
        public void table_is_created()
        {
            CreateSimpleTable();

            _dataAccess
                .ExecuteQuery("SELECT * FROM SimpleTable")
                .Count()
                .ShouldBe(0);
        }

        [Test]
        public void can_add_a_row_to_table()
        {
            CreateSimpleTable();

            _dataAccess.ExecuteCommand(string.Format(
                "INSERT INTO SimpleTable(RowGuid, Name) VALUES('{0}', 'Test')",
                Guid.NewGuid().ToString()));

            _dataAccess.ExecuteQuery("SELECT * FROM SimpleTable").Count().ShouldBe(1);
        }

        [Test]
        public void can_add_a_row_to_table_using_parameters()
        {
            CreateSimpleTable();

            _dataAccess.ExecuteCommand(
                "INSERT INTO SimpleTable(RowGuid, Name) VALUES(@RowGuid, @Name)",
                new
                {
                    RowGuid = Guid.NewGuid(),
                    Name = "Test"
                });

            _dataAccess.ExecuteQuery("SELECT * FROM SimpleTable").Count().ShouldBe(1);
        }

        [Test]
        public void can_retrieve_values_from_table()
        {
            CreateSimpleTable();

            _dataAccess.ExecuteCommand(
                "INSERT INTO SimpleTable(RowGuid, Name) VALUES(@RowGuid, @Name)",
                new { RowGuid = Guid.NewGuid(), Name = "Test" });

            var result = _dataAccess.ExecuteQuery("SELECT * FROM SimpleTable").First();

            ((string)result.Name).ShouldBe("Test");
        }

        private void CreateSimpleTable()
        {
            _dataAccess.ExecuteCommand(@"
CREATE TABLE SimpleTable(
    RowGuid UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(200) NOT NULL
)");
        }

        [Test]
        public void can_assign_a_null_value()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE SimpleTable(RowGuid UNIQUEIDENTIFIER NOT NULL, Name NVARCHAR(200))");
            _dataAccess.ExecuteCommand(
                "INSERT INTO SimpleTable(RowGuid) VALUES(@RowGuid)",
                new { RowGuid = Guid.NewGuid() });

            ((string)_dataAccess.ExecuteQuery("SELECT * FROM SimpleTable").First().Name).ShouldBe(null);
        }

        [Test]
        public void can_insert_and_query_multiple_rows()
        {
            CreateSimpleTable();

            _dataAccess.ExecuteCommand(
                "INSERT INTO SimpleTable(RowGuid, Name) VALUES(@RowGuid, @Name)",
                new { RowGuid = Guid.NewGuid(), Name = "Test 1" });
            _dataAccess.ExecuteCommand(
                "INSERT INTO SimpleTable(RowGuid, Name) VALUES(@RowGuid, @Name)",
                new { RowGuid = Guid.NewGuid(), Name = "Test 2" });

            var results = _dataAccess
                .ExecuteQuery("SELECT * FROM SimpleTable")
                .ToList();

            ((string)results[0].Name).ShouldBe("Test 1");
            ((string)results[1].Name).ShouldBe("Test 2");
        }

        [Test]
        public void can_query_with_parameters()
        {
            CreateSimpleTable();

            _dataAccess.ExecuteCommand(
                "INSERT INTO SimpleTable(RowGuid, Name) VALUES(@RowGuid, @Name)",
                new { RowGuid = Guid.NewGuid(), Name = "Test 1" });
            _dataAccess.ExecuteCommand(
                "INSERT INTO SimpleTable(RowGuid, Name) VALUES(@RowGuid, @Name)",
                new { RowGuid = Guid.NewGuid(), Name = "Test 2" });

            var results = _dataAccess.ExecuteQuery(
                "SELECT * FROM SimpleTable WHERE Name = @Name",
                new { Name = "Test 2" });

            results.Count().ShouldBe(1);
            string name = results.First().Name;

            name.ShouldBe("Test 2");
        }

        class SimpleTableDto
        {
            public Guid RowGuid;
            public string Name;
            public int NotUsed;
        }

        [Test]
        public void can_query_into_simple_dto()
        {
            CreateSimpleTable();

            _dataAccess.ExecuteCommand(
                "INSERT INTO SimpleTable(RowGuid, Name) VALUES(@RowGuid, @Name)",
                new { RowGuid = Guid.NewGuid(), Name = "Test 1" });
            _dataAccess.ExecuteCommand(
                "INSERT INTO SimpleTable(RowGuid, Name) VALUES(@RowGuid, @Name)",
                new { RowGuid = Guid.NewGuid(), Name = "Test 2" });

            var results = _dataAccess.ExecuteQuery<SimpleTableDto>("SELECT * FROM SimpleTable");

            results.Count().ShouldBe(2);
            results.First().Name.ShouldBe("Test 1");
            results.Skip(1).First().Name.ShouldBe("Test 2");
        }

        [Test]
        public void can_use_dto_for_commands()
        {
            CreateSimpleTable();

            _dataAccess.ExecuteCommand(
                "INSERT INTO SimpleTable(RowGuid, Name) VALUES(@RowGuid, @Name)",
                new SimpleTableDto { RowGuid = Guid.NewGuid(), Name = "Test" });

            var results = _dataAccess.ExecuteQuery("SELECT * FROM SimpleTable");

            results.Count().ShouldBe(1);
            ((string)results.First().Name).ShouldBe("Test");
        }
    }
}
