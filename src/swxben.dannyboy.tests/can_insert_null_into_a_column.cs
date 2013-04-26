using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace swxben.dannyboy.tests
{
    [TestFixture]
    public class can_insert_null_into_a_column
    {
        IDataAccess _dataAccess = new DataAccess(TestConfiguration.CONNECTION_STRING);


        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.ExecuteCommand(@"
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SimpleTable]') AND type IN (N'U'))
    DROP TABLE SimpleTable
");
        }

        class SimpleTableDto
        {
            public Guid RowGuid;
            public string Name;
        }

        [Test]
        public void table_is_created()
        {
            var dto = new SimpleTableDto { RowGuid = Guid.NewGuid() };
            _dataAccess.ExecuteCommand("CREATE TABLE SimpleTable(RowGuid UNIQUEIDENTIFIER NOT NULL, Name NVARCHAR(200))");

            _dataAccess.ExecuteCommand("INSERT INTO SimpleTable(RowGuid, Name) VALUES(@RowGuid, @Name)", dto);

            var result = _dataAccess.ExecuteQuery<SimpleTableDto>("SELECT * FROM SimpleTable").First();

            result.RowGuid.ShouldBe(dto.RowGuid);
            result.Name.ShouldBe(null);
        }
    }
}
