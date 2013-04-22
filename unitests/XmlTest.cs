

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using voidsoft.DataBlock.ObjectQuery;

namespace XmlTests
{

    /// <summary>
    /// Write the summary for the test class here.
    /// </summary>
    [TestFixture]
    public class XmlTest
    {
	    
       private XmlDataDocument table = null;
		    
	    
       
        public void GetData()
        {
            try
            {
                FileStream fs = new FileStream("file.xml", FileMode.Open, FileAccess.Read);


                XmlDataDocument xdoc = new XmlDataDocument();
                xdoc.DataSet.ReadXmlSchema("file.xml");
                xdoc.Load("file.xml");

	        	this.table = xdoc; 

            }
            catch (Exception)
            {

                throw;
            }
        }

	
	     /// <summary>
        /// 
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.GetData();
        }


         #region SELECT
        [Test]
        public void TestSimpleSelect()
        {
            ObjectQuery<XmlDataDocument> query = new ObjectQuery<XmlDataDocument>();
            XmlElement[] rows = (XmlElement[]) query.Select(this.table, "ProductName like G%");

            //    Assert.IsTrue(rows.Length == 2);
        }


        [Test]
        public void TestComplexSelect()
        {
            ObjectQuery<XmlDataDocument> query = new ObjectQuery<XmlDataDocument>();
            XmlElement[] rows = (XmlElement[]) query.Select(this.table, "ProductName like G% and SupplierID > 3 or CategoryID > 45 and UnitPrice < 1000.34 and UnitsOnOrder < 10");
        }
#endregion



        #region min

        //[Test]
        //public void TestMinDate()
        //{
        //    ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
        //    DataRow d = (DataRow)query.Min(table, "BirthDate");

        //    DateTime dt = Convert.ToDateTime(d["BirthDate"]);
        //    Assert.IsTrue(dt.Year == 1991);
        //}


        [Test]
        public void TestMinSmallValue()
        {
            ObjectQuery<XmlDataDocument> query = new ObjectQuery<XmlDataDocument>();
            XmlElement d = (XmlElement)query.Min(table, "UnitsOnOrder");
            //Assert.IsTrue(d["UnitsOnOrder"].ToString() == "1");
        }

        [Test]
        public void TestMinSalary()
        {
            ObjectQuery<XmlDataDocument> query = new ObjectQuery<XmlDataDocument>();
            XmlElement d = (XmlElement)query.Min(table, "UnitPrice");

           // Assert.IsTrue(d["Salary"].ToString() == "44.65");
        }
        #endregion




        #region max
        //[Test]
        //public void TestMaxDate()
        //{
        //    ObjectQuery<DataTable> query = new ObjectQuery<DataTable>();
        //    DataRow d = (DataRow)query.Max(table, "BirthDate");

        //    DateTime dt = Convert.ToDateTime(d["BirthDate"]);
        //    Assert.IsTrue(dt.Year == 2006);
        //}


        [Test]
        public void TestMaxSmallValue()
        {
            ObjectQuery<XmlDataDocument> query = new ObjectQuery<XmlDataDocument>();
            XmlElement d = (XmlElement)query.Max(table, "UnitsInStock");

           // Assert.IsTrue(d["SmallValue"].ToString() == "192");
        }

        /// <summary>
        /// Tests the max salary.
        /// </summary>
        [Test]
        public void TestMaxWithCondition()
        {
            ObjectQuery<XmlDataDocument> query = new ObjectQuery<XmlDataDocument>();
            XmlElement d = (XmlElement)query.Max(table, "UnitPrice", "UnitPrice > 0");
        }
        #endregion

        #region Avg
        [Test]
        public void TestAvg()
        {
            ObjectQuery<XmlDataDocument> query = new ObjectQuery<XmlDataDocument>();
            decimal d = Convert.ToDecimal(query.Avg(table, "UnitPrice"));
        }
        #endregion

        #region Sum
        [Test]
        public void TestSum()
        {
            ObjectQuery<XmlDataDocument> query = new ObjectQuery<XmlDataDocument>();
            decimal d = Convert.ToDecimal(query.Sum(table, "UnitPrice"));
        }


        [Test]
        public void TestConditionalSum()
        {
            ObjectQuery<XmlDataDocument> query = new ObjectQuery<XmlDataDocument>();
            decimal d = Convert.ToDecimal(query.Sum(table, "UnitPrice", "UnitPrice > 3"));
        }
        #endregion
    }

}
