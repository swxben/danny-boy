using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    class ConstructorAccessibilityTests
    {
        readonly IDataAccess _dataAccess = new DataAccess(TestConfiguration.ConnectionString);

        class Hobbit
        {
            public string Name { get; protected set; }
        }

        [SetUp, TearDown]
        public void SetUpAndTearDown()
        {
            _dataAccess.DropTable("Hobbits");
        }
        
        class PublicHobbit : Hobbit
        {
            public PublicHobbit()
            {
            }
        }

        [Test]
        public void HobbitWithPublicConstructorCanBeConstructed()
        {
            GetHobbit<PublicHobbit>().Name.ShouldBe("Frodo");
        }

        class ProtectedHobbit : Hobbit
        {
            protected ProtectedHobbit()
            {
            }

            public ProtectedHobbit(int willNotBeUsed)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void HobbitWithProtectedConstructorCanBeConstructed()
        {
            GetHobbit<ProtectedHobbit>().Name.ShouldBe("Frodo");
        }

        class PrivateHobbit : Hobbit
        {
            private PrivateHobbit()
            {
            }

            public PrivateHobbit(int willNotBeUsed)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void HobbitWithPrivateConstructorCanBeConstructed()
        {
            GetHobbit<PrivateHobbit>().Name.ShouldBe("Frodo");
        }

        class UnusableHobbit
        {
            public UnusableHobbit(int willNotBeUsed)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void HobbitWithNoUsableDefaultConstructorsCannotBeConstructed()
        {
            Should.Throw<InvalidEntityTypeException>(() => GetHobbit<UnusableHobbit>());
        }

        private T GetHobbit<T>()
        {
            CreateTable();
            InsertData();

            return _dataAccess.Select<T>(tableName: "Hobbits").FirstOrDefault();
        }

        void CreateTable()
        {
            _dataAccess.ExecuteCommand(@"
CREATE TABLE Hobbits(
	Name NVARCHAR(MAX) NOT NULL
)
                ");
        }

        void InsertData()
        {
            _dataAccess.ExecuteCommand(@"
INSERT INTO Hobbits(
    Name
) VALUES(
    'Frodo'
)");
        }
    }
}
