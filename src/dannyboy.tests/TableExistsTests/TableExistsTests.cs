using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests.TableExistsTests
{
    public class TableExistsTests : DataAccessTestBase
    {
        [Test]
        public void ReturnsFalseWhenTableDoesNotExist()
        {
            DataAccess.TableExists("Persons").ShouldBe(false);
        }

        [Test]
        public void ReturnsTrueWhenTableDoesExists()
        {
            CreatePersonsTable();

            DataAccess.TableExists("Persons").ShouldBe(true);
        }
    }
}