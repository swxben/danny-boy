using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shouldly;

namespace dannyboy.tests
{
    public class SelectToFieldAccessibilityTests
    {
        readonly IDataAccess _dataAccess = new DataAccess(TestConfiguration.ConnectionString);

        [SetUp, TearDown]
        public void SetUpAndTearDown()
        {
            _dataAccess.DropTable("Hobbits");
        }

        class Hobbit
        {
            private string _father;
            private string _mother;
            public string Name { get; protected set; }
            public int Age { get; private set; }
            public string Father { get { return _father; } protected set { _father = value; } }
            public string Mother { get { return _mother; } private set { _mother = value; } }
            protected bool HasRing { get; set; }
            private string Nephew { get; set; }

            public bool GetHasRing()
            {
                return HasRing;
            }

            public string GetNephew()
            {
                return Nephew;
            }
        }

        [Test]
        public void ProtectedSetterIsAssigned()
        {
            GetHobbit().Name.ShouldBe("Bilbo Baggins");
        }

        [Test]
        public void ProtectedSetterToUnderlyingFieldIsAssigned()
        {
            GetHobbit().Father.ShouldBe("Bungo Baggins");
        }

        [Test]
        public void PrivateSetterIsNotAssigned()
        {
            GetHobbit().Age.ShouldBe(0);
        }

        [Test]
        public void PrivateSetterToUnderlyingFieldIsAssigned()
        {
            GetHobbit().Mother.ShouldBe("Belladonna Took");
        }

        [Test]
        public void ProtectedPropertyIsNotAssigned()
        {
            GetHobbit().GetHasRing().ShouldBe(false);
        }

        [Test]
        public void PrivatePropertyIsNotAssigned()
        {
            GetHobbit().GetNephew().ShouldBe(null);
        }

        private Hobbit GetHobbit()
        {
            CreateTable();
            InsertData();

            var hobbit = _dataAccess.Select<Hobbit>().FirstOrDefault();
            return hobbit;
        }

        void CreateTable()
        {
            _dataAccess.ExecuteCommand(@"
CREATE TABLE Hobbits(
	Name NVARCHAR(MAX) NOT NULL,
    AGE INTEGER NOT NULL,
    Father NVARCHAR(MAX) NOT NULL,
    Mother NVARCHAR(MAX) NOT NULL,
    HasRing BIT NOT NULL,
    Nephew NVARCHAR(MAX) NOT NULL
)
                ");
        }

        void InsertData()
        {
            _dataAccess.ExecuteCommand(@"
INSERT INTO Hobbits(
    Name, Age, Father, Mother, HasRing, Nephew
) VALUES(
    'Bilbo Baggins', 111, 'Bungo Baggins', 'Belladonna Took', 1, 'Frodo Baggins'
)");
        }
    }
}
