using System;
using NUnit.Framework;
using Shouldly;

namespace swxben.dannyboy.tests
{
    [TestFixture]
    public class get_insert_sql_is_sane
    {
        class ExampleOne
        {
            public Guid ExampleOneIdentifier;
            public string ExampleOneStringProperty { get; set; }
            public string ExampleOneIntValue;
        }

        [Test]
        public void insert_sql_is_correct_for_example_one()
        {
            var sql = DataAccess.GetInsertSqlFor(typeof(ExampleOne));
            sql.ShouldBeCloseTo(
"INSERT INTO ExampleOnes(ExampleOneIdentifier, ExampleOneIntValue, ExampleOneStringProperty) VALUES(@ExampleOneIdentifier, @ExampleOneIntValue, @ExampleOneStringProperty)");
        }
    }
}
