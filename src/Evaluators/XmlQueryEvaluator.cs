/*
 
       file : XmlDataDocumentEvaluator.cs
description : Evaluator implementation for XMLDataDocument. This implementation aggregates the DataTable implementation.  
     author : Marius Gheorghe


*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Net;
using System.IO;

namespace voidsoft.DataBlock.ObjectQuery
{
    /// <summary>
    /// XmlDataDocument evaluator
    /// </summary>
    internal class XmlQueryEvaluator : IEvaluator 
    {

        #region IEvaluator Members
        /// <summary>
        /// Evaluates the specified query
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="t">Object type</param>
        /// <param name="dataSource">Datasource for evaluation</param>
        /// <param name="condition">Query to evaluate</param>
        /// <returns>Returns a XmlElement array which contains the selected data</returns>
        public object Evaluate<T>(T t, object dataSource, string condition)
        {
            DataTableQueryEvaluator eval = null;
            XmlElement[] nodes = null;

            try
            {
                eval = new DataTableQueryEvaluator();

                XmlDataDocument xdoc = (XmlDataDocument)dataSource;

                DataRow[] rows = (DataRow[])eval.Evaluate<DataTable>(xdoc.DataSet.Tables[0], xdoc.DataSet.Tables[0], condition);
                
                nodes = new XmlElement[eval.EvaluatorIndexes.Count];

                for (int i = 0; i < eval.EvaluatorIndexes.Count; i++)
                {
                    nodes[i] = xdoc.GetElementFromRow(xdoc.DataSet.Tables[0].Rows[eval.EvaluatorIndexes[i]]);
                }

                return (object)nodes;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Evaluates the minimum value.
        /// </summary>
        /// <param name="t">Type to evaluate</param>
        /// <param name="dataSource">Data source</param>
        /// <param name="fieldName">Field name foe which the evaluation is made</param>
        /// <returns>XmlElement which contains the resulting field</returns>
        public object EvaluateMin<T>(T t, object dataSource, string fieldName)
        {
            DataTableQueryEvaluator eval = null;
            XmlElement element = null;

            try
            {
                eval = new DataTableQueryEvaluator();

                XmlDataDocument xdoc = (XmlDataDocument)dataSource;

                DataRow row = (DataRow)eval.EvaluateMin<DataTable>(xdoc.DataSet.Tables[0], xdoc.DataSet.Tables[0], fieldName);

                element = xdoc.GetElementFromRow(xdoc.DataSet.Tables[0].Rows[eval.EvaluatorIndexes[0]]);

                return element;
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Evaluates the minimum value
        /// </summary>
        /// <param name="t">Type to evaluate</param>
        /// <param name="dataSource">The data source to be evaluated</param>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="query">Query</param>
        /// <returns>XmlElement which contains the resulting field </returns>
        public object EvaluateMin<T>(T t, object dataSource, string fieldName, string query)
        {
            DataTableQueryEvaluator eval = null;
            XmlElement element = null;

            try
            {
                eval = new DataTableQueryEvaluator();

                XmlDataDocument xdoc = (XmlDataDocument)dataSource;

                DataRow row = (DataRow)eval.EvaluateMin<DataTable>(xdoc.DataSet.Tables[0], xdoc.DataSet.Tables[0], fieldName, query);

                element = xdoc.GetElementFromRow(xdoc.DataSet.Tables[0].Rows[eval.EvaluatorIndexes[0]]);

                return element;
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Evaluates the maximum value
        /// </summary>
        /// <param name="t">Type to be evaluated</param>
        /// <param name="dataSource">Data source to be evaluated</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>XmlElement which contains the resulting field</returns>
        public object EvaluateMax<T>(T t, object dataSource, string fieldName)
        {
            DataTableQueryEvaluator eval = null;
            XmlElement element = null;

            try
            {
                eval = new DataTableQueryEvaluator();

                XmlDataDocument xdoc = (XmlDataDocument)dataSource;

                DataRow row = (DataRow)eval.EvaluateMax<DataTable>(xdoc.DataSet.Tables[0], xdoc.DataSet.Tables[0], fieldName);

                element = xdoc.GetElementFromRow(xdoc.DataSet.Tables[0].Rows[eval.EvaluatorIndexes[0]]);

                return element;
            }
            catch
            {
                throw;
            }
        }


        
	/// <summary>
        /// Evaluates the maximum value
        /// </summary>
        /// <param name="t">Type to be evaluated</param>
        /// <param name="dataSource">The data source to be evaluated</param>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="query">Query</param>
        /// <returns>XmlElement which contains the resulting field</returns>
        public object EvaluateMax<T>(T t, object dataSource, string fieldName, string query)
        {
            DataTableQueryEvaluator eval = null;
            XmlElement element = null;

            try
            {
                eval = new DataTableQueryEvaluator();

                XmlDataDocument xdoc = (XmlDataDocument)dataSource;

                DataRow row = (DataRow)eval.EvaluateMax<DataTable>(xdoc.DataSet.Tables[0], xdoc.DataSet.Tables[0], fieldName, query);

                element = xdoc.GetElementFromRow(xdoc.DataSet.Tables[0].Rows[eval.EvaluatorIndexes[0]]);

                return element;
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Calculated the average value
        /// </summary>
        /// <param name="dataSource">The data source</param>
        /// <param name="fieldName">Name of the field</param>
        /// <returns>Returs the average value of the specified field</returns>
        public decimal Avg<T>(object dataSource, string fieldName)
        {
            DataTableQueryEvaluator eval = null;

            try
            {
                eval = new DataTableQueryEvaluator();

                XmlDataDocument xdoc = (XmlDataDocument)dataSource;

                decimal result = Convert.ToDecimal( eval.Avg<DataTable>(xdoc.DataSet.Tables[0], fieldName));

                return result;
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Gets the sum of the field 
        /// </summary>
        /// <param name="dataSource">The data source</param>
        /// <param name="fieldName">Name of the field</param>
        /// <returns>Returs the sum of the specified field</returns>
        public decimal Sum<T>(object dataSource, string fieldName)
        {
            DataTableQueryEvaluator eval = null;

            try
            {
                eval = new DataTableQueryEvaluator();

                XmlDataDocument xdoc = (XmlDataDocument)dataSource;

                decimal result = Convert.ToDecimal(eval.Sum<DataTable>(xdoc.DataSet.Tables[0], fieldName));

                return result;
            }
            catch
            {
                throw;
            }
	}

	
        /// <summary>
        /// Gets the sum of the field 
        /// </summary>
        /// <param name="dataSource">The data source</param>
        /// <param name="fieldName">Name of the field</param>
        /// <returns>Returs the sum of the specified field</returns>
        public decimal Sum<T>(object dataSource, string fieldName, string query)
        {
	    DataTableQueryEvaluator eval = null;

            try
            {
                eval = new DataTableQueryEvaluator();

                XmlDataDocument xdoc = (XmlDataDocument)dataSource;

                decimal result = Convert.ToDecimal(eval.Sum<DataTable>(xdoc.DataSet.Tables[0], fieldName, query));

                return result;
            }
            catch
            {
                throw;
            }
        }
        #endregion
	
    }
}
