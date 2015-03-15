using NUnit.Framework;

namespace dannyboy.tests
{
    public abstract class DataAccessTestBase
    {
        protected readonly IDataAccess DataAccess = new DataAccess(TestConfiguration.ConnectionString);

        protected void CreatePersonsTable()
        {
            DataAccess
                .ExecuteCommand("CREATE TABLE [Persons]([Name] NVARCHAR(MAX))");
        }

        [TearDown]
        public virtual void TearDown()
        {
            DataAccess.DropTable("Persons");
        }

        protected class Person
        {
            public string Name { get; set; }
        }
    }
}