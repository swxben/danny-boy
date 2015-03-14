using NUnit.Framework;

namespace dannyboy.tests
{
    public abstract class DataAccessTestBase
    {
        protected IDataAccess DataAccess = new DataAccess(TestConfiguration.ConnectionString);

        protected void CreatePersonsTable()
        {
            DataAccess
                .ExecuteCommand("CREATE TABLE [Persons]([Name] NVARCHAR(MAX))");
        }

        [TearDown]
        public void TearDown()
        {
            DataAccess.DropTable("Persons");
        }

    }
}