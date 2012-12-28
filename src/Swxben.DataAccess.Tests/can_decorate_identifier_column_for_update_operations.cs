using System;
using NUnit.Framework;
using Shouldly;
using swxben.dataaccess;

namespace Tests
{
    [TestFixture]
    public class can_decorate_identifier_column_for_update_operations
    {
        class Example
        {
            [DataAccess.Identifier]
            public Guid ExampleGuid { get; set; }
            public string Name { get; set; }
        }

        [Test]
        public void update_sql_uses_identifier_property()
        {
            var sql = DataAccess.GetUpdateSqlFor<Example>();
            sql.ShouldBeCloseTo("UPDATE Examples SET Name = @Name WHERE 1=1 AND ExampleGuid = @ExampleGuid");
        }

        class CompoundIdentifierExample
        {
            [DataAccess.Identifier]
            public Guid IdentifierPartOneGuid { get; set; }
            [DataAccess.Identifier]
            public Guid IdentifierPartTwoGuid { get; set; }
            [DataAccess.Identifier]
            public string IdentifierPartThree { get; set; }
            public string Name { get; set; }
        }

        [Test]
        public void condition_is_correct_for_compound_identifiers()
        {
            DataAccess
                .GetUpdateSqlFor<CompoundIdentifierExample>()
                .ShouldBeCloseTo("UPDATE CompoundIdentifierExamples SET Name = @Name WHERE 1=1 AND IdentifierPartOneGuid = @IdentifierPartOneGuid AND IdentifierPartTwoGuid = @IdentifierPartTwoGuid AND IdentifierPartThree = @IdentifierPartThree");
        }

        class DtoWithIdentifiersAndIgnores
        {
            [DataAccess.Identifier]
            public Guid DtoGuidPartOne { get; set; }
            [DataAccess.Identifier]
            public Guid DtoGuidPartTwo { get; set; }
            public string Name;
            public int Age;
            [DataAccess.Ignore]
            public string IgnoreOne;
            [DataAccess.Ignore]
            public string IgnoreTwo;
        }

        [Test]
        public void sql_is_correct_for_identifiers_and_ignores()
        {
            DataAccess
                .GetUpdateSqlFor<DtoWithIdentifiersAndIgnores>()
                .ShouldBeCloseTo("UPDATE DtoWithIdentifiersAndIgnoress SET Name = @Name, Age = @Age WHERE 1=1 AND DtoGuidPartOne = @DtoGuidPartOne AND DtoGuidPartTwo = @DtoGuidPartTwo");
        }

        [Test]
        public void passing_empty_set_of_identifiers_uses_decorated_identifiers()
        {
            DataAccess
                .GetUpdateSqlFor<DtoWithIdentifiersAndIgnores>(new string[0])
                .ShouldBeCloseTo("UPDATE DtoWithIdentifiersAndIgnoress SET Name = @Name, Age = @Age WHERE 1=1 AND DtoGuidPartOne = @DtoGuidPartOne AND DtoGuidPartTwo = @DtoGuidPartTwo");
        }
    }
}
