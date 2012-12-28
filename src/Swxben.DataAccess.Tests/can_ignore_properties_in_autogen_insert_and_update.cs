using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shouldly;
using swxben.dataaccess;

namespace Tests
{
    [TestFixture]
    public class can_ignore_properties_in_autogen_insert_and_update
    {
        class Example
        {
            public Guid ExampleGuid { get; set; }
            public string Name { get; set; }
            [DataAccess.Ignore]
            public string DisplayName { get { return Name.ToUpper(); } }
        }

        [Test]
        public void dto_is_sane()
        {
            new Example { Name = "test" }.DisplayName.ShouldBe("TEST");
        }

        [Test]
        public void insert_sql_ignores_property()
        {
            var sql = DataAccess.GetInsertSqlFor<Example>();
            sql.ShouldBeCloseTo("INSERT INTO Examples(ExampleGuid, Name) VALUES(@ExampleGuid, @Name)");
        }

        [Test]
        public void update_sql_ignores_property()
        {
            var sql = DataAccess.GetUpdateSqlFor<Example>("ExampleGuid");
            sql.ShouldBeCloseTo("UPDATE Examples SET Name = @Name WHERE 1=1 AND ExampleGuid = @ExampleGuid");
        }
    }
}
