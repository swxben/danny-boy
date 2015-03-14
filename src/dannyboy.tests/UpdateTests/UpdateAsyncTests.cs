using System;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.UpdateTests
{
    public class UpdateAsyncTests : DataAccessTestBase
    {
        [Test]
        public void DataIsInFactUpdated()
        {
            DataAccess.ExecuteCommand(@"
CREATE TABLE [Persons](
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(MAX)
)");
            var person = new Person
            {
                Id = Guid.NewGuid(),
                Name = "Initial Name"
            };
            DataAccess.Insert(person);

            person.Name = "Updated name";

            DataAccessAsync.UpdateAsync(person).Wait();

            DataAccess.ExecuteScalar<string>("SELECT TOP 1 Name FROM Persons")
                .ShouldBe("Updated name");
        }

        private class Person
        {
            [DataAccess.IdentifierAttribute]
            public Guid Id { get; set; }

            public string Name { get; set; }
        }
    }
}