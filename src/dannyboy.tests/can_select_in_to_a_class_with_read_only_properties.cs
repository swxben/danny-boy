using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    [TestFixture]
    public class can_select_in_to_a_class_with_read_only_properties
    {
        readonly IDataAccess _dataAccess = new DataAccess(TestConfiguration.CONNECTION_STRING);

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.ExecuteCommand(@"
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MyDtos]') AND type IN (N'U'))
    DROP TABLE MyDtos
");
        }

        class MyDto
        {
            public Guid RowGuid { get; set; }
            public string Name { get; set; }

            public string NameDisplay { get { return Name.ToUpper(); } }
        }

        [Test]
        public void select_works()
        {
            _dataAccess.ExecuteCommand(@"
CREATE TABLE MyDtos(
    RowGuid UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    NameDisplay NVARCHAR(200) NOT NULL
)");
            _dataAccess.ExecuteCommand(
    "INSERT INTO MyDtos(RowGuid, Name, NameDisplay) VALUES(@RowGuid, @Name, @NameDisplay)",
    new { RowGuid = Guid.NewGuid(), Name = "Test", NameDisplay = "IGNORE ME" });

            var result = _dataAccess.Select<MyDto>();

            result.Count().ShouldBe(1);
            result.First().NameDisplay.ShouldBe("TEST");
        }
    }
}
