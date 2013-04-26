using System;
using NUnit.Framework;
using Shouldly;

namespace swxben.dannyboy.tests
{
    [TestFixture]
    public class can_use_compound_identifiers
    {
        class Example
        {
            public Guid ExampleGuid { get; set; }
            public string CompoundPartTwo { get; set; }
            public int CompoundPartThree { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [Test]
        public void sql_is_sane()
        {
            DataAccess
                .GetUpdateSqlFor(typeof(Example), new[] { "ExampleGuid", "CompoundPartTwo", "CompoundPartThree" })
                .ShouldBeCloseTo("UPDATE Examples SET Name = @Name, Age = @Age WHERE 1=1 AND ExampleGuid = @ExampleGuid AND CompoundPartTwo = @CompoundPartTwo AND CompoundPartThree = @CompoundPartThree");
        }
    }
}
