using System;
using NUnit.Framework;
using Shouldly;
using swxben.dataaccess;

namespace Tests
{
    [TestFixture]
    public class get_update_sql_is_sane
    {
        class ExampleOne
        {
            public Guid ExampleOneIdentifier;
            public string ExampleOneStringProperty { get; set; }
            public string ExampleOneIntValue;
        }

        [Test]
        public void update_sql_is_correct_for_example_one()
        {
            var sql = DataAccess.GetUpdateSqlFor<ExampleOne>("ExampleOneIdentifier");

            sql.ShouldBeCloseTo(
"UPDATE ExampleOnes SET ExampleOneIntValue = @ExampleOneIntValue, ExampleOneStringProperty = @ExampleOneStringProperty WHERE ExampleOneIdentifier = @ExampleOneIdentifier");
        }
    }
}
