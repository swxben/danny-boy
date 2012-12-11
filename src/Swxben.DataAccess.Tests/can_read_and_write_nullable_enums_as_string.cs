using System.Linq;
using NUnit.Framework;
using Shouldly;
using swxben.dataaccess;

namespace Tests
{
    [TestFixture]
    public class can_read_and_write_nullable_enums_as_string
    {
        IDataAccess _dataAccess = new DataAccess(@"Server=.\sqlexpress; Database=swxben_dataaccess; User Id=sa; Password=test;");

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.DropTable("Drinks");
        }

        enum Colour
        {
            Black,
            Red,
            Brown,
            Yellow
        }

        enum Cost { Expensive, NotExpensive };

        class Drink
        {
            public string Name;
            public Colour? DrinkColour;
            public Cost? DrinkCost { get; set; }
        }

        [Test]
        public void non_null_value_is_written_correctly()
        {
            CreateTable();
            _dataAccess.Insert(new Drink { Name = "Test", DrinkColour = Colour.Black });

            var result = _dataAccess.ExecuteQuery("SELECT * FROM Drinks").First();

            ((string)result.Name).ShouldBe("Test");
            ((string)result.DrinkColour).ShouldBe("Black");
        }

        private void CreateTable()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Drinks(Name NVARCHAR(100), DrinkColour NVARCHAR(100), DrinkCost NVARCHAR(20))");
        }

        [Test]
        public void null_value_is_written_correctly()
        {
            CreateTable();
            _dataAccess.Insert(new Drink { Name = "Test", DrinkColour = null });

            var result = _dataAccess.ExecuteQuery("SELECT * FROM Drinks").First();

            ((string)result.Name).ShouldBe("Test");
            ((string)result.DrinkColour).ShouldBe(null);
        }

        [Test]
        public void non_null_value_is_read_correctly()
        {
            CreateTable();
            _dataAccess.Insert(new Drink { Name = "Test", DrinkColour = Colour.Black });

            var drink = _dataAccess.Select<Drink>().First();

            drink.Name.ShouldBe("Test");
            drink.DrinkColour.ShouldBe(Colour.Black);
        }

        [Test]
        public void null_value_is_read_correctly()
        {
            CreateTable();
            _dataAccess.Insert(new Drink { Name = "Test", DrinkColour = null });

            var drink = _dataAccess.Select<Drink>().First();

            drink.Name.ShouldBe("Test");
            drink.DrinkColour.ShouldBe(null);
        }

        [Test]
        public void non_null_property_is_read_correctly()
        {
            CreateTable();
            _dataAccess.Insert(new Drink { Name = "Test", DrinkColour = Colour.Black, DrinkCost = Cost.Expensive });

            var drink = _dataAccess.Select<Drink>().First();

            drink.DrinkCost.ShouldBe(Cost.Expensive);
        }

        [Test]
        public void null_property_is_read_correctly()
        {
            CreateTable();
            _dataAccess.Insert(new Drink { Name = "Test", DrinkColour = Colour.Black, DrinkCost = null });

            var drink = _dataAccess.Select<Drink>().First();

            drink.DrinkCost.ShouldBe(null);
        }
    }
}
