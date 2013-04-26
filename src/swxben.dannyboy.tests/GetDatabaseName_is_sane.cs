using NUnit.Framework;
using Shouldly;

namespace swxben.dannyboy.tests
{
    [TestFixture]
    class GetDatabaseName_is_sane
    {
        [Test]
        public void using_database_syntax()
        {
            var dataAccess = new DataAccess(@"Server=.\sqlexpress; Database=magical_unicorn; User Id=sa; Password=test;");
            dataAccess.GetDatabaseName().ShouldBe("magical_unicorn");
        }

        [Test]
        public void using_initial_catalog_syntax()
        {
            var dataAccess = new DataAccess(@"Data Source=dbserver; Initial Catalog=magical_unicorn; Integrated Security=SSPI; User ID=myDomain\myUsername; Password=myPassword;");
            dataAccess.GetDatabaseName().ShouldBe("magical_unicorn");
        }

        [Test]
        public void using_unsupported_syntax()
        {
            var dataAccess = new DataAccess(@"blerg://dbserver/magical_unicorn/username/password");
            dataAccess.GetDatabaseName().ShouldBe("");
        }
    }
}
