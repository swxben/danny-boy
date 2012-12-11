using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using swxben.dataaccess;
using Shouldly;

namespace Tests
{
    [TestFixture]
    public class get_select_sql_is_sane
    {
        class Customer
        {
            public int CustomerId;
            public string CustomerName;
        }

        [Test]
        public void select_sql_is_correct_for_a_simple_select()
        {
            var sql = DataAccess.GetSelectSqlFor<Customer>();

            sql.ShouldBeCloseTo("SELECT * FROM Customers");
        }

        [Test]
        public void sql_is_correct_for_a_single_criteria()
        {
            var sql = DataAccess.GetSelectSqlFor<Customer>(
                criteria: new { CustomerId = 1 });
            sql.ShouldBeCloseTo("SELECT * FROM Customers WHERE 1=1 AND CustomerId = @CustomerId");
        }

        [Test]
        public void sql_is_correct_for_multiple_criteria()
        {
            var sql = DataAccess.GetSelectSqlFor<Customer>(
                criteria: new { ColumnA = "test", ColumnB = 13 });
            sql.ShouldBeCloseTo("SELECT * FROM Customers WHERE 1=1 AND ColumnA = @ColumnA AND ColumnB = @ColumnB");
        }

        [Test]
        public void sql_is_correct_for_order_clause()
        {
            var sql = DataAccess.GetSelectSqlFor<Customer>(orderBy: "ColumnA DESC, ColumnB");
            sql.ShouldBeCloseTo("SELECT * FROM Customers ORDER BY ColumnA DESC, ColumnB");
        }

        [Test]
        public void sql_is_correct_for_criteria_and_order_clauses()
        {
            var sql = DataAccess.GetSelectSqlFor<Customer>(
                criteria: new { ColumnA = "test", ColumnB = 13 },
                orderBy: "ColumnA");
            sql.ShouldBeCloseTo("SELECT * FROM Customers WHERE 1=1 AND ColumnA = @ColumnA AND ColumnB = @ColumnB ORDER BY ColumnA");
        }
    }
}
