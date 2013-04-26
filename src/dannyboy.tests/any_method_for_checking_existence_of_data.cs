using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    [TestFixture]
    public class any_method_for_checking_existence_of_data
    {
        class Person
        {
            public string Name;
            public int Age;
        }

        readonly IDataAccess _dataAccess = new DataAccess(TestConfiguration.CONNECTION_STRING);

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.DropTable("Persons");
        }

        [Test]
        public void false_when_there_is_no_data()
        {
            CreateTable();
            _dataAccess.Any<Person>().ShouldBe(false);
            _dataAccess.Exists<Person>().ShouldBe(false);
        }

        [Test]
        public void false_when_no_matching_data()
        {
            CreateTable();

            _dataAccess.Insert(new Person { Name = "Aaa", Age = 123 });

            _dataAccess.Any<Person>(new { Name = "bbb" }).ShouldBe(false);
            _dataAccess.Any<Person>(new { Age = 12 }).ShouldBe(false);
            _dataAccess.Exists<Person>(new { Name = "bbb" }).ShouldBe(false);
            _dataAccess.Exists<Person>(new { Age = 12 }).ShouldBe(false);
        }

        [Test]
        public void true_when_data_with_no_criteria()
        {
            CreateTable();

            _dataAccess.Insert(new Person { Name = "Aaa", Age = 123 });

            _dataAccess.Any<Person>().ShouldBe(true);
            _dataAccess.Exists<Person>().ShouldBe(true);
        }

        [Test]
        public void true_when_data_with_matching_criteria()
        {
            CreateTable();

            _dataAccess.Insert(new Person { Name = "Aaa", Age = 123 });

            _dataAccess.Any<Person>(new { Name = "Aaa" }).ShouldBe(true);
            _dataAccess.Any<Person>(new { Age = 123 }).ShouldBe(true);
            _dataAccess.Exists<Person>(new { Name = "Aaa" }).ShouldBe(true);
            _dataAccess.Exists<Person>(new { Age = 123 }).ShouldBe(true);
        }

        void CreateTable()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Persons(Name NVARCHAR(100), Age INT)");
        }
    }
}
