using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    [TestFixture]
    public class ignored_properties_still_get_assigned_when_query_includes_values_for_them
    {
        IDataAccess _dataAccess = new DataAccess(@"Server=.\sqlexpress; Database=swxben_dataaccess; User Id=sa; Password=test;");

        class Example
        {
            public string Name { get; set; }
            [DataAccess.Ignore]
            public int Age { get; set; }
        }

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.DropTable("Examples");
        }

        [Test]
        public void sanity_check_can_save_to_database()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Examples(Name NVARCHAR(100) NOT NULL)");

            _dataAccess.Insert(new Example { Name = "John" });

            _dataAccess.Select<Example>().First().Name.ShouldBe("John");
            _dataAccess.ExecuteQuery<Example>("SELECT * FROM Examples").First().Name.ShouldBe("John");
            _dataAccess.Select<Example>().First().Age.ShouldBe(default(int));
            _dataAccess.ExecuteQuery<Example>("SELECT * FROM Examples").First().Age.ShouldBe(default(int));
        }

        [Test]
        public void query_that_returns_value_for_property()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Examples(Name NVARCHAR(100) NOT NULL)");

            _dataAccess.Insert(new Example { Name = "John" });

            _dataAccess.ExecuteQuery<Example>("SELECT Name, 32 AS Age FROM Examples").First().Age.ShouldBe(32);
        }
    }
}
