/*

           file : DataTableQueryEvaluator.cs
description : Evaluator implementation for DataTable
      author : Marius Gheorghe
  
 */ 

using System;
using System.Collections.Generic;
using System.Text;
using voidsoft.DataBlock.ObjectQuery;
using System.Data;


namespace voidsoft.DataBlock.ObjectQuery
{

    /// <summary>
    /// DataTable query evaluator
    /// </summary>
    internal class DataTableQueryEvaluator : IEvaluator
    {
        
        //list which contains the indexes of the selected rows
        private List<int> listIndexes = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="DataTableQueryEvaluator"/> class.
        /// </summary>
        public DataTableQueryEvaluator()
        {
            listIndexes = new List<int>();
        }


        #region IEvaluator Members

        /// <summary>
        /// Evaluates the maximum value
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public object EvaluateMax<T>(T t, object dataSource, string fieldName)
        {
            ObjectQueryEvaluator evaluator;
            QueryParser parser = null;

            try
            {
                evaluator = new ObjectQueryEvaluator();
                parser = new QueryParser();

                DataTable table = (DataTable)dataSource;

                //look for the column name
                int index = table.Columns.IndexOf(fieldName);

                if (index == -1)
                {
                    throw new ArgumentException("Invalid field name");
                }

                //clear the index fields
                this.listIndexes.Clear();


                if (parser.IsNumeric(table.Columns[index].DataType))
                {
                    return (object)this.EvaluateNumeric(ref table, index, false);
                }
                else if (parser.IsDateTime(table.Columns[index].DataType))
                {
                    return (object)this.EvaluateDateTime(ref table, index, false);
                }

                //since the DataTable types can't support IComparable it means we have a invalid column type
                throw new ArgumentException("Invalid column type");
            }
            catch
            {
                throw;
            }
        }



        /// <summary>
        /// Evaluates the max.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public object EvaluateMax<T>(T t, object dataSource, string fieldName, string query)
        {
            QueryParser parser = null;

            try
            {
                parser = new QueryParser();

                DataTable table = (DataTable)dataSource;

                List<QueryCondition> conditions = parser.Parse(query);

                //get the selected rows
                DataRow[] rows = (DataRow[])this.Evaluate<DataTable>(table, table, query);

                //build a new DataTable which contains the new rows
                DataTable newTable = table.Clone();
                for (int i = 0; i < rows.Length; i++)
                {
                    newTable.ImportRow(rows[i]);    
                }

                //evaluate 
                return this.EvaluateMax<DataTable>(newTable, newTable, fieldName);
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Evaluates the minimum value
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public object EvaluateMin<T>(T t, object dataSource, string fieldName)
        {
            ObjectQueryEvaluator evaluator;
            QueryParser parser = null;

            try
            {
                evaluator = new ObjectQueryEvaluator();
                parser = new QueryParser();

                DataTable table = (DataTable)dataSource;

                int index = table.Columns.IndexOf(fieldName);

                if (index == -1)
                {
                    throw new ArgumentException("Invalid field name");
                }

                //clear the indexes
                this.listIndexes.Clear();


                if (parser.IsNumeric(table.Columns[index].DataType))
                {
                    return this.EvaluateNumeric(ref table, index, true);
                }
                else if (parser.IsDateTime(table.Columns[index].DataType))
                {
                    return this.EvaluateDateTime(ref table, index, true);
                }

                //since the DataTable types can't support IComparable it means we have a invalid column type
                throw new ArgumentException("Invalid column type"); 
            }
            catch
            {
                throw;
            }
        }



        /// <summary>
        /// Evaluates the minimum value
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="datasource">The datasource.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public object EvaluateMin<T>(T t, object datasource, string fieldName, string query)
        {
            QueryParser parser = null;

            try
            {
                parser = new QueryParser();

                DataTable table = (DataTable) datasource;

                List<QueryCondition> conditions = parser.Parse(query);

                DataRow[] rows = (DataRow[]) this.Evaluate<DataTable>(table, table, query);

                DataTable newTable = table.Clone();
                newTable.Rows.Add(rows);

                return this.EvaluateMin<DataTable>(newTable, newTable, fieldName);
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// Returns the average value of the specified field
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public decimal Avg<T>(object dataSource, string propertyName)
        {
            try
            {
                decimal result = this.Sum<T>(dataSource, propertyName);

                DataTable table = (DataTable)dataSource;  

                return result / table.Rows.Count;
            }
            catch 
            {
                throw;
            }
        }


        /// <summary>
        /// Sums the specified data source.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public decimal Sum<T>(object dataSource, string fieldName)
        {
            try
            {
                DataTable table = (DataTable)dataSource;

                int index = table.Columns.IndexOf(fieldName);

                decimal result = 0;

                for (int i = 0; i < table.Rows.Count; i++)
                {
                    result += Convert.ToDecimal(table.Rows[i][index]);
                }

                return result;
            }
            catch 
            {
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="fieldName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public decimal Sum<T>(object dataSource, string fieldName, string query)
        {
            QueryParser parser = null;

            try
            {
                parser = new QueryParser();

                DataTable table = (DataTable)dataSource;

                List<QueryCondition> conditions = parser.Parse(query);

                DataRow[] rows = (DataRow[])this.Evaluate<DataTable>(table, table, query);

                DataTable newTable = table.Clone();

                for (int i = 0; i < rows.Length; i++)
                {
                    DataRow newRow = newTable.NewRow();

                    for (int k = 0; k < newTable.Columns.Count; k++)
                    {
                        newRow[k] = rows[i][k];
                    }

                    newTable.Rows.Add(newRow);
                }

                return this.Sum<DataTable>(newTable, fieldName);
            }
            catch
            {
                throw;
            }
        }



        /// <summary>
        /// Gets the list of evaluated indexes.
        /// </summary>
        public List<int> EvaluatorIndexes
        {
            get
            {
                return this.listIndexes;
            }
        }




        /// <summary>
        /// Evaluates the specified query
        /// </summary>
        /// <param name="t"></param>
        /// <param name="dataSource"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public object Evaluate<T>(T t, object dataSource, string query)
        {
            //query parser
            QueryParser parser = null;

            //list which holds the results
            List<DataRow> listResults = null;

            //list with query conditions
            List<QueryCondition> listConditions = null;

            try
            {
                listResults = new List<DataRow>();
                parser = new QueryParser();


                //clear the indexes
                this.listIndexes.Clear();


                //parse the condition and get the list of query conditions
                listConditions = parser.Parse(query);

                //flag used to know the operation result
                bool result = false;

                DataTable table = (DataTable)dataSource;


                for (int i = 0; i < table.Rows.Count; i++)
                {
                    //reset the flag
                    result = false;

                    //loop thru the queries and check the fields with its associated query
                    foreach (QueryCondition currentCondition in listConditions)
                    {
                        //check the fieldName
                        if (! table.Columns.Contains(currentCondition.FieldName))
                        {
                            throw new ArgumentException("Invalid field name " + currentCondition.FieldName);
                        }

                        int columnIndex = table.Columns.IndexOf(currentCondition.FieldName);

                        //evaluate the specific type
                        result = EvaluateType(ref parser, table.Rows[i][columnIndex], table.Columns[columnIndex].DataType , currentCondition);

                        //if it fails there's no point on evaluation the other conditions so break
                        if (result == false)
                        {
                            //but before breaking check for conditional OR queries
                            if (currentCondition.ListConditions != null)
                            {
                                foreach (QueryCondition var in currentCondition.ListConditions)
                                {
                                    bool innerResult = false;

                                    if (! table.Columns.Contains(var.FieldName))
                                    {
                                        throw new ArgumentException("Invalid field name " + currentCondition.FieldName);
                                    }

                                    int columnPosition = table.Columns.IndexOf(var.FieldName);

                                    //evaluate the specific type
                                    innerResult = EvaluateType(ref parser, table.Rows[i][columnPosition], table.Columns[columnPosition].DataType, var);

                                    if (innerResult == true)
                                    {
                                        //found a match so it passes
                                        result = true;
                                        this.listIndexes.Add(i);
                                        break;
                                    }
                                }
                            }

                            break;
                        }
                    }

                    if (result == true)
                    {
                        listResults.Add(table.Rows[i]);

                        this.listIndexes.Add(i);
                    }
                }

                DataRow[] results = new DataRow[listResults.Count];
                listResults.CopyTo(results);

                return (object) results;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (listConditions != null)
                {
                    listConditions.Clear();
                    listConditions = null;
                }
                if (listResults != null)
                {
                    listResults.Clear();
                    listResults = null;
                }
            }
        }
        #endregion



        #region internal implementation
        /// <summary>
        /// Evaluates the type
        /// </summary>
        /// <param name="parser">QueryParser which evlauates the type</param>
        /// <param name="currentValue">The current value.</param>
        /// <param name="type">The type.</param>
        /// <param name="currentCondition">The current condition.</param>
        /// <returns>Boolean flag which indicates if the evaluation succedded</returns>
        private bool EvaluateType(ref QueryParser parser,
                                                  object currentValue,
                                                  Type type,
                                                  QueryCondition currentCondition)
        {
            ObjectQueryEvaluator queryEvaluator = new ObjectQueryEvaluator();
            
            bool result = false;

            //check the field's type and call the evaluator.
            if (parser.IsNumeric(type))
            {
                result = queryEvaluator.EvaluateNumeric(currentValue, currentCondition);
            }
            else if (parser.IsString(type))
            {
                result = queryEvaluator.EvaluateString(currentValue, currentCondition);
            }
            else if (parser.IsDateTime(type))
            {
                result = queryEvaluator.EvaluateDateTime(currentValue, currentCondition);
            }
            else if (parser.IsBool(type))
            {
                result = queryEvaluator.EvaluateBool(currentValue, currentCondition);
            }
            else
            {
                //try to evaluate a IComparable type
                result = queryEvaluator.EvaluateIComparable(currentValue, type, currentCondition);
            }

            return result;
        }



        /// <summary>
        /// Evaluates a numeric type
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columnIndex">Index of the column.</param>
        private object EvaluateNumeric(ref DataTable table,
                                        int columnIndex,
                                        bool evaluateMinimum)
        {
            decimal initialValue = 0;
            int rowIndex = -1;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (evaluateMinimum)
                {
                    #region evaluate minimum
                    if(i == 0)
                    {
                        initialValue = Convert.ToDecimal(table.Rows[i][columnIndex]);
                        rowIndex = i;
                    }
                    else
                	{
                        if(Convert.ToDecimal(table.Rows[i][columnIndex]) < initialValue)
                        {
                            initialValue = Convert.ToDecimal(table.Rows[i][columnIndex]);
                            rowIndex = i;
                        }
	                }
                    #endregion
                }
                else
                {
                    #region evaluate maximum
                    if (i == 0)
                    {
                        initialValue = Convert.ToDecimal(table.Rows[i][columnIndex]);
                        rowIndex = i;
                    }
                    else
                    {
                        if (Convert.ToDecimal(table.Rows[i][columnIndex]) > initialValue)
                        {
                            initialValue = Convert.ToDecimal(table.Rows[i][columnIndex]);
                            rowIndex = i;
                        }
                    }
                    #endregion
                }
            }

            this.listIndexes.Add(rowIndex);
            return table.Rows[rowIndex];
        }


        /// <summary>
        /// Evaluates the date time.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="evaluateMinimum">if set to <c>true</c> [evaluate minimum].</param>
        /// <returns></returns>
        private object EvaluateDateTime(ref DataTable table,
                                          int columnIndex,
                                          bool evaluateMinimum)
        {
            DateTime initialValue = DateTime.Now;
            int rowIndex = -1;


            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (evaluateMinimum)
                {
                    #region evaluate minimum
                    if (i == 0)
                    {
                        initialValue = Convert.ToDateTime(table.Rows[i][columnIndex]);
                        rowIndex = i;
                    }
                    else
                    {
                        DateTime current = Convert.ToDateTime(table.Rows[i][columnIndex]);

                        int result = DateTime.Compare(current, initialValue);

                        if (result < 0)
                        {
                            initialValue = current;
                            rowIndex = i;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region evaluate maximum
                    if (i == 0)
                    {
                        initialValue = Convert.ToDateTime(table.Rows[i][columnIndex]);
                        rowIndex = i;
                    }
                    else
                    {
                        DateTime current = Convert.ToDateTime(table.Rows[i][columnIndex]);

                        int result = DateTime.Compare(current, initialValue);

                        if (result > 0)
                        {
                            initialValue = current;
                            rowIndex = i;
                        }
                    }
                    #endregion
                }
            }


            this.listIndexes.Add(rowIndex);
            return table.Rows[rowIndex];
        }
        #endregion

    }
}
