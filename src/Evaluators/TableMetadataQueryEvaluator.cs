///*

//      file : TableMetadataQueryEvaluator.cs
//description: TableMetdata evaluator
//    author : Marius Gheorghe 
  
//*/


//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using System.Text.RegularExpressions;
//using voidsoft.DataBlock;


//namespace voidsoft.DataBlock.ObjectQuery
//{
//    /// <summary>
//    /// TableMetadata query evaluator
//    /// </summary>
//    internal class TableMetadataQueryEvaluator : IEvaluator
//    {
//        #region constructor
//        /// <summary>
//        /// Creates a new instance of TableMetadataQueryEvaluator
//        /// </summary>
//        public TableMetadataQueryEvaluator()
//        {
//        }
//        #endregion

//        #region IEvaluator Members

//        /// <summary>
//        /// Evaluates the specified 
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="data"></param>
//        /// <param name="condition"></param>
//        /// <returns></returns>
//        public T[] Evaluate<T>(IList<T> data, string condition) 
//        {
//            QueryParser parser = null;
//            ObjectQueryEvaluator eval = null;

//            //list which holds the results
//            List<T> listResults = null;

//            //list with query conditions
//            List<QueryCondition> listConditions = null;

//            try
//            {
//                listResults = new List<T>();
//                parser = new QueryParser();
//                eval = new ObjectQueryEvaluator();


//                //parse the condition and get the list of query conditions
//                listConditions = parser.Parse(condition);


//                //flag used to know the operation result
//                bool result = false;


//                for (int i = 0; i < data.Count; i++)
//                {
//                    //reset the flag
//                    result = false;

//                    //loop thru the queries and check the fields with its associated query
//                    foreach (QueryCondition currentCondition in listConditions)
//                    {
//                        DatabaseField currentField = new DatabaseField();

//                        //get the field associated with the query
//                        try
//                        {
//                            TableMetadata mainTable = data[i] as TableMetadata;

//                            currentField = mainTable.GetField(currentCondition.FieldName);
//                        }
//                        catch
//                        {
//                            //we have a invalid field
//                            throw new ArgumentException("Invalid field name " + currentCondition.FieldName);
//                        }



//                        //check the field's type and call the evaluator.
//                        if (parser.IsNumeric(currentField.fieldType))
//                        {
//                            result = eval.EvaluateNumeric(currentField.fieldValue, currentCondition);
//                        }
//                        else if (parser.IsString(currentField.fieldType))
//                        {
//                            result = eval.EvaluateString(currentField.fieldValue, currentCondition);
//                        }
//                        else if (parser.IsDateTime(currentField.fieldType))
//                        {
//                            result = eval.EvaluateDateTime(currentField.fieldValue, currentCondition);
//                        }
//                        else if (parser.IsBool(currentField.fieldType))
//                        {
//                            result = eval.EvaluateBool(currentField.fieldValue, currentCondition);
//                        }
//                        else
//                        {
//                            //unsupported type
//                            throw new ArgumentException("The field " + currentField.fieldName + " has a unsupported type");
//                        }


//                        //if it fails there's no point on evaluation the other conditions so break
//                        if (result == false)
//                        {
//                            break;
//                        }
//                    }

//                    if (result == true)
//                    {
//                        listResults.Add(data[i]);
//                    }
//                }

//                //return result
//                T[] results = new T[listResults.Count];
//                listResults.CopyTo(results);


//                return results;
//            }
//            catch
//            {
//                throw;
//            }
//            finally
//            {
//                if (listConditions != null)
//                {
//                    listConditions.Clear();
//                }
//                if (listResults != null)
//                {
//                    listResults.Clear();
//                }
//            }

//        }
//        #endregion

//        #region Operation evaluators

//        /// <summary>
//        /// Evaluates the specified DateTime expression
//        /// </summary>
//        /// <param name="data">Data source</param>
//        /// <param name="field">DataField upon which the evaluation is being done</param>
//        /// <param name="evaluateMinimum">Flag used to know if we evaluate the min value.If false the MAX value is evaluated</param>
//        /// <returns>Selected data</returns>
//        public DateTime EvaluateDateTime<T>(IList<T> data,
//                                            DatabaseField field,
//                                            bool evaluateMinimum) where T : TableMetadata 
//        {
//            DateTime result = new DateTime(1,1,1);
//            int fieldIndex = -1;
           
//            //first get the fields index
//            for(int j = 0 ; j < data[0].TableFields.Length; j++)
//            {
//                if(data[0].TableFields[j].fieldName == field.fieldName)
//                {
//                    fieldIndex = j;
//                    break;                    
//                }
//            }
            
//            if(fieldIndex == -1)
//            {
//                throw new ArgumentException("Invalid field specified");   
//            }
            
//            DateTime value = new DateTime();
            
//            for(int i = 0 ; i <  data.Count; i++)
//            {
//                //reset
//                value = new DateTime(1, 1, 1);

//                if(! data[i].IsNull(fieldIndex))
//                {
//                    value = Convert.ToDateTime(data[i].TableFields[fieldIndex].fieldValue);
                    
//                    int comparasionResult = DateTime.Compare(value, result);
                    
//                    //check if this is the first index.
//                    if(i ==  0)
//                    {
//                        result = value;   
//                    }
//                    else
//                    {
//                        //check if we evaluate min || max
                        
//                        if(evaluateMinimum == true)
//                        {
//                            if(comparasionResult < 0)
//                            {
//                                result = value;   
//                            }
//                        }
//                        else
//                        {
//                            //evaluate maximum
                            
//                            if(comparasionResult > 0)
//                            {
//                                result = value;
//                            }                                
                        
//                        }
//                    }
//                }
                
//            }                
            
//            return result;    
//        }

        
        
        
//        /// <summary>
//        /// Evaluates the specified numeric expression
//        /// </summary>
//        /// <typeparam name="T">Type of parameter for which the evaluation is being done</typeparam>
//        /// <param name="data">Data source</param>
//        /// <param name="field">DatabaseField for which evaluation is done</param>
//        /// <param name="evaluateMinimum">Flag used to know if we evaluate the MIN version. If false the MAX version is evaluated</param>
//        /// <returns>Selected data</returns>
//        public decimal EvaluateNumeric<T>(IList<T> data,
//                                          DatabaseField field,
//                                          bool evaluateMinimum) where T : TableMetadata
//        {
//            decimal result = 0;
//            int fieldIndex = -1;
            
//            //first get the fields index
//            for(int j = 0 ; j < data[0].TableFields.Length; j++)
//            {
//                if(data[0].TableFields[j].fieldName == field.fieldName)
//                {
//                    fieldIndex = j;
//                    break;                    
//                }
//            }
            
            
//            if(fieldIndex == -1)
//            {
//                throw new ArgumentException("Invalid field specified");   
//            }
            
            
//            decimal value = 0;
            
//            for(int i = 0 ; i <  data.Count; i++)
//            {
//                if(! data[i].IsNull(fieldIndex))
//                {
//                    value = Convert.ToDecimal(data[i].TableFields[fieldIndex].fieldValue);
                    
//                    if(i == 0)
//                    {
//                        result = value;   
//                    }
//                    else
//                    {
//                        //check if we evaluate min || max
//                        if(evaluateMinimum == true)
//                        {
//                            if(value < result)
//                            {
//                                result = value;   
//                            }
//                        }
//                        else
//                        {
//                            //evaluate maximum
//                            if(value > result)
//                            {
//                                result = value;
//                            }                                
                        
//                        }
//                    }
//                }
                
//            }                
            
        
//            return result;    
//        }
//        #endregion
//    }
//}
