using NUnit.Framework;

namespace dannyboy.tests
{
    public abstract class DataAccessTestBase
    {
        readonly DataAccess _dataAccess = new DataAccess(TestConfiguration.ConnectionString);
        protected IDataAccess DataAccess { get { return _dataAccess; } }
        protected IDataAccessAsync DataAccessAsync { get { return _dataAccess; } }

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