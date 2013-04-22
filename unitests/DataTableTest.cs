using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Data;
using voidsoft.DataBlock.ObjectQuery;



namespace ObjectQueryTests
{
    /// <summary>
    /// Write the summary for the test class here.
    /// </summary>
    [TestFixture]
    public class DataTableTest
    {
        private DataTable table = null;


        private void GetData()
        {
            table = new DataTable();

            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Age", typeof(Int32));
            table.Columns.Add("Salary", typeof(Decimal));
            table.Columns.Add("BirthDate", typeof(DateTime));
            table.Columns.Add("SmallValue", typeof(byte));
            table.Columns.Add("IsMale", typeof(bool));

            object[] vars = { "SlickEdit", "43", "435344.65", new DateTime(2006, 1, 3), "3", "true"};
            table.Rows.Add(vars);
            object[] v = { "Googoman", "13", "44.65", new DateTime(1992,2,3), "1" , "false"};
            table.Rows.Add(v);
            object[] va = { "Edit", "73", "344.65", new DateTime(2000, 12, 23), "9" , "true"};
            table.Rows.Add(va);
            object[] vax = { "Gangaroo", "93", "118", new DateTime(1991,5,5), "192", "false" };
            table.Rows.Add(vax);
        }

        /// <summary>
        /// Write your setup story here.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.GetData();
        }

        /// <summary>
        /// Write your teardown story here.
        /// </summary>
        [TearDown]
        public void Dispose()
        {

        }

        #region SELECT
        [Test]
        public void TestSimpleSelect()
        {
            ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
            DataRow[] rows = (DataRow[]) query.Select(this.table, "Name like G%");

            Assert.IsTrue(rows.Length == 2);
        }

        [Test]
        public void TestComplexSelect()
        {
            ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
            DataRow[] rows = (DataRow[])query.Select(this.table, "Name like G% and Age > 3 or Salary > 45 and Salary < 1000.34 and BirthDate < [1,1,2005] and SmallValue < 10");
        }
        #endregion



        #region min

        [Test]
        public void TestMinDate()
        {
            ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
            DataRow d = (DataRow)query.Min(table, "BirthDate");

            DateTime dt = Convert.ToDateTime(d["BirthDate"]);
            Assert.IsTrue(dt.Year == 1991);
        }


        [Test]
        public void TestMinSmallValue()
        {
            ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
            DataRow d = (DataRow)query.Min(table, "SmallValue");

            Assert.IsTrue(d["SmallValue"].ToString() == "1");
        }

        [Test]
        public void TestMinSalary()
        {
            ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
            DataRow d = (DataRow)query.Min(table, "Salary");

            Assert.IsTrue(d["Salary"].ToString() == "44.65");
        }
        #endregion






        #region max
        [Test]
        public void TestMaxDate()
        {
            ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
            DataRow d = (DataRow)query.Max(table, "BirthDate");

            DateTime dt = Convert.ToDateTime(d["BirthDate"]);
            Assert.IsTrue(dt.Year == 2006);
        }


        [Test]
        public void TestMaxSmallValue()
        {
            ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
            DataRow d = (DataRow)query.Max(table, "SmallValue");

            Assert.IsTrue(d["SmallValue"].ToString() == "192");
        }

        [Test]
        public void TestMaxSalary()
        {
            ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
            DataRow d = (DataRow)query.Max(table, "Salary");

            Assert.IsTrue(d["Salary"].ToString() == "435344.65");
        }
        #endregion


        #region Avg
        [Test]
        public void TestAvg()
        {
            ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
            decimal d = Convert.ToDecimal( query.Avg(table, "Salary"));
        }
        #endregion



        #region Sum
        [Test]
        public void TestSum()
        {
            ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
            decimal d = Convert.ToDecimal(query.Sum(table, "SmallValue"));
        }


        [Test]
        public void TestConditionalSum()
        {
            ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
            decimal d = Convert.ToDecimal(query.Sum(table, "SmallValue", "SmallValue > 3"));
        }
        #endregion

    }
}
