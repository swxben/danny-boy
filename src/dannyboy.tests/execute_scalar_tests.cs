using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    [TestFixture]
    public class execute_scalar_tests
    {
        readonly IDataAccess _dataAccess = new DataAccess(TestConfiguration.ConnectionString);

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.DropTable("TestResults");
        }

        class TestResult
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public int Score { get; set; }
            public TestResult(string name, int age, int score) { Name = name; Age = age; Score = score; }
        }

        [Test]
        public void can_get_count()
        {
            CreateTestTable();
            InsertTestData();
            ((int)_dataAccess.ExecuteScalar("SELECT COUNT(*) FROM TestResults")).ShouldBe(14);
            _dataAccess.ExecuteScalar<int>("SELECT COUNT(*) FROM TestResults").ShouldBe(14);
        }

        [Test]
        public void can_get_single_result()
        {
            CreateTestTable();
            InsertTestData();
            _dataAccess
                .ExecuteScalar<int>("SELECT Score FROM TestResults WHERE Name = @Name", new { Name = "Sulr" })
                .ShouldBe(86);
        }

        [Test]
        public void can_get_average()
        {
            CreateTestTable();
            InsertTestData();
            _dataAccess
                .ExecuteScalar<double>("SELECT AVG(CAST(Age AS FLOAT)) FROM TestResults WHERE Score >= 80")
                .ShouldBe(27.66f, 0.01f);
        }

        [Test]
        public void gets_first_result()
        {
            CreateTestTable();
            InsertTestData();
            _dataAccess
                .ExecuteScalar<string>("SELECT Name, Age, Score FROM TestResults ORDER BY Age ASC, Name DESC")
                .ShouldBe("Rayt");
        }


        void CreateTestTable()
        {
            _dataAccess.ExecuteCommand(@"
CREATE TABLE TestResults(
    Name NVARCHAR(200) NOT NULL,
    Age INT NOT NULL,
    Score INT NOT NULL
)");
        }

        void InsertTestData()
        {
            _dataAccess.Insert(new TestResult("Aemy", 30, 46));
            _dataAccess.Insert(new TestResult("Iormi", 26, 87));
            _dataAccess.Insert(new TestResult("Sulr", 31, 86));
            _dataAccess.Insert(new TestResult("Rayph", 41, 98));
            _dataAccess.Insert(new TestResult("Essp", 20, 83));
            _dataAccess.Insert(new TestResult("Radch", 39, 13));
            _dataAccess.Insert(new TestResult("Adt", 37, 52));
            _dataAccess.Insert(new TestResult("Sud", 22, 54));
            _dataAccess.Insert(new TestResult("Banld", 31, 29));
            _dataAccess.Insert(new TestResult("Rayt", 20, 13));
            _dataAccess.Insert(new TestResult("Ent", 29, 59));
            _dataAccess.Insert(new TestResult("Aqua", 24, 84));
            _dataAccess.Insert(new TestResult("Bouz", 32, 71));
            _dataAccess.Insert(new TestResult("Icero", 24, 94));
        }
    }
}
