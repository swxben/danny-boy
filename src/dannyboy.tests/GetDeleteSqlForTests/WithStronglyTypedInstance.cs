using System;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.GetDeleteSqlForTests
{
    public class WithStronglyTypedInstance
    {
        [Test]
        public void ItIsValid()
        {
            DataAccess
                .GetDeleteSqlFor(new User())
                .ShouldBe("DELETE FROM [Users] WHERE 1=1 AND [Id1] = @Id1 AND [Id2] = @Id2");
        }

        private class User
        {
            [DataAccess.Identifier]
            public Guid Id1 { get; set; }

            [DataAccess.Identifier]
            public Guid Id2 { get; set; }

            public string Name { get; set; }
        }
    }
}