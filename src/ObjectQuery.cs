/*

      file : ObjectQuery.cs
description: ObjectQuery implementation.
     author: Marius Gheorghe
 
 
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using voidsoft.DataBlock;
using voidsoft.DataBlock.ObjectQuery;


namespace voidsoft.DataBlock.ObjectQuery
{
    /// <summary>
    /// ObjectQuery  
    /// </summary>
    /// <typeparam name="T">Generic type for ObjectQuery</typeparam>
    public class ObjectQuery<T>  
    {
        // Current IEvaluator
        private IEvaluator eval = null;

        // Current Type
        private T currentType = default(T);


        #region Ctors
        /// <summary>
        /// Creates a new instance of ObjectQuery
        /// </summary>
        public ObjectQuery()
        {
            this.CreateEvaluator<T>(currentType, ref this.eval);
        } 
        #endregion


        #region Select

        /// <summary>
        /// Select data based on a specific condition
        /// </summary>
        /// <param name="data">Data source</param>
        /// <param name="condition">ObjectQuery condition</param>
        /// <returns>Array which contains the selected data</returns>
        public object Select(object dataSource,
                             string condition)
        {
            if(condition == null || condition.Trim().Length == 0)
            {
                throw new ArgumentException("Invalid criteria condition");
            }

            object result = eval.Evaluate<T>(this.currentType, dataSource, condition);
            return result;
        }
        #endregion

        
        #region Other operations
        
        //NOTE: supported only for DateTime and numeric fields.

        /// <summary>
        /// Returns the smallest value from the specified data source
        /// </summary>
        /// <param name="data">Data source</param>
        /// <param name="propertyName">The field who's value we return</param>
        /// <returns>The value of the selected object</returns>
        public object Min(object dataSource,
                          string propertyName)
        {
            return (this.eval.EvaluateMin<T>(currentType, dataSource, propertyName));
        }


        /// <summary>
        /// Returns the smallest value from the specified data source
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public object Min(object dataSource,
                          string propertyName,
                          string query)
        {
            return (this.eval.EvaluateMin<T>(currentType, dataSource, propertyName, query));
        }


	
        /// <summary>
        /// Gets the max value of the selected data
        /// </summary>
        /// <param name="data">Data source</param>
        /// <param name="propertyName">The processing field</param>
        /// <returns>The value of the selected object</returns>
        public object Max(object dataSource,
                          string propertyName)
        {
           return (this.eval.EvaluateMax<T>(currentType, dataSource, propertyName));
        }



        /// <summary>
        /// Maxes the specified data source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public object Max(object dataSource,
                          string propertyName,
                          string query)
        {
            return (this.eval.EvaluateMax<T>(currentType, dataSource, propertyName, query));
        }


        /// <summary>
        /// Gets the average value of the selected data
        /// </summary>
        /// <param name="data">Data source</param>
        /// <param name="propertyName">The processing field</param>
        /// <returns>The value of the selected object</returns>
        public decimal Avg(object dataSource,
                           string propertyName)
        {
           return (this.eval.Avg<T>(dataSource, propertyName));
        }    


        /// <summary>
        /// Gets the sum of the selected data
        /// </summary>
        /// <param name="data">Data source</param>
        ///<param name="propertyName"></param>
        /// <param name="condition">Query condition</param>
        /// <returns>The sum of selected objects</returns>
        public decimal Sum(object dataSource,
                           string  propertyName,
                           string query)
        {
            return (this.eval.Sum<T>(dataSource, propertyName, query));
        }            


        /// <summary>
        /// Gets the sum of the selected data
        /// </summary>
        /// <param name="data">Data source</param>
        /// <param name="propertyName">The processing field</param>
        /// <returns>The sum of the selected objects</returns>
        public decimal Sum(object dataSource,
                           string propertyName)
        {
            return (this.eval.Sum<T>(dataSource, propertyName));
        }
        #endregion


        #region internal implementation
        /// <summary>
        /// Creates a IEvaluator based on the object's type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="eval"></param>
        private void CreateEvaluator<K>(K tp, ref  IEvaluator eval)
        {
            Type type = typeof(K);

            if (type.FullName == "System.Data.DataTable")
            {
                eval = new DataTableQueryEvaluator();
                return;
            }
            else if (type.FullName == "voidsoft.DataBlock.TableMetadata")
            {
                //eval = new T 
                return;
            }
            else if (type.FullName ==  "System.Xml.XmlDataDocument")
            {
                eval = new XmlQueryEvaluator();
                return;
            }

            eval = new ObjectQueryEvaluator();
        }
        #endregion

    }
}
