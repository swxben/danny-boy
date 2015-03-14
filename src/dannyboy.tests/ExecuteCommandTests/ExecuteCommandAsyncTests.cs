using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.ExecuteCommandTests
{
    public class ExecuteCommandAsyncTests : DataAccessTestBase
    {
        [Test]
        public void CanCreateTableAsync()
        {
            DataAccess
                .ExecuteCommandAsync("CREATE TABLE [Persons]([Name] NVARCHAR(MAX))")
                .Result.ShouldBe(-1);

            DataAccess.TableExists("Persons").ShouldBe(true);
        }
    }
}
