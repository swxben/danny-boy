using System;
using NUnit.Framework;

namespace dannyboy.tests.DeleteTests
{
    public abstract class DeleteTestBase : DataAccessTestBase
    {
        protected void  CreateUsersTable()
        {
            DataAccess.ExecuteCommand(@"
CREATE TABLE [Users](
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL
)");
        }

        [TearDown]
        public override void TearDown()
        {
            DataAccess.DropTable("Users");
            base.TearDown();
        }

        protected class User
        {
            [DataAccess.Identifier]
            public Guid Id { get; set; }

            public string Name { get; set; }
        }
    }
}