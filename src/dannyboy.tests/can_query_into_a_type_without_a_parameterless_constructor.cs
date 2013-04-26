using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    [TestFixture]
    class can_query_into_a_type_without_a_parameterless_constructor
    {
        IDataAccess _dataAccess = new DataAccess(@"Server=.\sqlexpress; Database=swxben_dataaccess; User Id=sa; Password=test;");

        class VampireService
        { }

        class Vampire
        {
            readonly VampireService _vampireService;

            public Vampire(VampireService vampireService)
            {
                _vampireService = vampireService;
            }

            public string Name { get; set; }
            public int Age { get; set; }
        }

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.DropTable("Vampires");
        }

        [Test]
        public void can_use_execute_query()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Vampires(Name NVARCHAR(200) NOT NULL, Age INT NOT NULL)");
            _dataAccess.ExecuteCommand("INSERT INTO Vampires(Name, Age) VALUES('Bill', 172)");
            _dataAccess.ExecuteCommand("INSERT INTO Vampires(Name, Age) VALUES('Jessica', 17)");
            _dataAccess.ExecuteCommand("INSERT INTO Vampires(Name, Age) VALUES('Erik', 1204)");

            var vampireService = new VampireService();
            var vampires = _dataAccess.ExecuteQuery<Vampire>(
                () => new Vampire(vampireService),
                "SELECT * FROM Vampires");

            vampires.Count().ShouldBe(3);
            vampires.First(v => v.Name == "Bill").Age.ShouldBe(172);
        }

        [Test]
        public void can_use_select()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Vampires(Name NVARCHAR(200) NOT NULL, Age INT NOT NULL)");
            _dataAccess.ExecuteCommand("INSERT INTO Vampires(Name, Age) VALUES('Bill', 172)");
            _dataAccess.ExecuteCommand("INSERT INTO Vampires(Name, Age) VALUES('Jessica', 17)");
            _dataAccess.ExecuteCommand("INSERT INTO Vampires(Name, Age) VALUES('Erik', 1204)");

            var vampireService = new VampireService();
            var erik = _dataAccess.Select<Vampire>(
                () => new Vampire(vampireService),
                where: new { Name = "Erik" }
                ).First();

            erik.Name.ShouldBe("Erik");
            erik.Age.ShouldBe(1204);
        }
    }
}
