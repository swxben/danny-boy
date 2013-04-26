using System.Linq;
using NUnit.Framework;
using Shouldly;

namespace swxben.dannyboy.tests
{
    [TestFixture]
    public class can_read_and_write_enums_as_a_string_field
    {
        IDataAccess _dataAccess = new DataAccess(@"Server=.\sqlexpress; Database=swxben_dataaccess; User Id=sa; Password=test;");

        [SetUp, TearDown]
        public void set_up_and_tear_down()
        {
            _dataAccess.ExecuteCommand(@"
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Drinks]') AND type IN (N'U'))
    DROP TABLE Drinks
                ");
        }

        enum Colour
        {
            Black,
            Red,
            Brown,
            Yellow
        }

        class DrinkDto
        {
            public string Name;
            public Colour DrinkColour;
        }

        [Test]
        public void enum_value_is_written_correctly()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Drinks(Name NVARCHAR(100), DrinkColour NVARCHAR(100))");
            _dataAccess.ExecuteCommand(
                "INSERT INTO Drinks(Name, DrinkColour) VALUES(@Name, @DrinkColour)",
                new DrinkDto { Name = "Bloody Mary", DrinkColour = Colour.Red });

            var result = _dataAccess.ExecuteQuery("SELECT * FROM Drinks").First();

            ((string)result.DrinkColour).ShouldBe("Red");
        }

        [Test]
        public void enum_value_is_read_back_correctly()
        {
            _dataAccess.ExecuteCommand("CREATE TABLE Drinks(Name NVARCHAR(100), DrinkColour NVARCHAR(100))");
            _dataAccess.ExecuteCommand(
                "INSERT INTO Drinks(Name, DrinkColour) VALUES(@Name, @DrinkColour)",
                new DrinkDto { Name = "Rum", DrinkColour = Colour.Brown });

            var dto = _dataAccess.ExecuteQuery<DrinkDto>("SELECT * FROM Drinks").First();

            dto.DrinkColour.ShouldBe(Colour.Brown);
        }
    }
}
