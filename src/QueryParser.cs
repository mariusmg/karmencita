/*

       file: QueryParser.cs
description: parses the query into  AST representation (QueryCondition)
     author: Marius Gheorghe
  
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;


namespace voidsoft.DataBlock.ObjectQuery
{

    /// <summary>
    /// Karmencita query parser
    /// </summary>
    internal class QueryParser
    {
        //queries separator
        private const string QUERY_SEPARATOR = " and ";
        
        //conditional separator for queries
        private const string CONDITION_SEPARATOR = " or ";
        
        //length of the component's of a normal query
        private const byte QUERY_LENGTH = 3;

        //length of the component's of a extended query (with BETWEEN operator)
        private const byte EXTENDED_QUERY_LENGTH = 4;

        //chars used for value placeholders
        private const char VALUE_START_CHAR ='[';
        private const char VALUE_END_CHAR = ']';


 
        #region parser implementation

        /// <summary>
        /// Parses the specified condition
        /// </summary>
        /// <param name="condition">object query condition</param>
        /// <returns>List of query conditions</returns>
        internal List<QueryCondition> Parse(string condition)
        {
            List<string> arguments = null;
            List<QueryCondition> listObjectQuery = null;
            List<string> listQueryComponents = null;

            try
            {
                arguments = new List<string>();
                listObjectQuery = new List<QueryCondition>();
                listQueryComponents = new List<string>();

                //split the string in multiple queries 
                string[] results = this.SliceStringByContext(condition, QUERY_SEPARATOR, VALUE_START_CHAR.ToString(), VALUE_END_CHAR.ToString());


                foreach (string var in results)
                {
                    //we have a conditional query. Now try to see if we have OR conditions inside it.
                    string[] queryParts = this.SliceStringByContext(var, CONDITION_SEPARATOR, VALUE_START_CHAR.ToString(), VALUE_END_CHAR.ToString());

                    if (queryParts.Length > 0)
                    {
                        QueryCondition oqc = new QueryCondition();

                        for (int i = 0; i < queryParts.Length; i++)
                        {
                            if (i == 0)
                            {
                                oqc = this.ParseSingleQuery(queryParts[i]);

                                //check if we have multiple OR queries
                                if (queryParts.Length > 1)
                                {
                                    //lazy initialization for "OR" list. There is no point in initializing it
                                    //for a single value
                                    oqc.ListConditions = new List<QueryCondition>();
                                }
                            }
                            else
                            {
                                oqc.ListConditions.Add(this.ParseSingleQuery(queryParts[i]));
                            }
                        }

                        listObjectQuery.Add(oqc);
                    }
                    else
                    {
                        //we have a single condition so just add it 
                        listObjectQuery.Add(this.ParseSingleQuery(var));
                    }
                }

                return listObjectQuery;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message, ex);
            }
        }

        

        /// <summary>
        /// Parses a single query  
        /// </summary>
        /// <param name="query">Query to be parsed</param>
        /// <returns></returns>
        private QueryCondition ParseSingleQuery(string query)
        {
            QueryCondition oqc;

            try
            {
                oqc = new QueryCondition();

                //split the query
                string[] parts = this.SliceStringByContext(query, " ", VALUE_START_CHAR.ToString(), VALUE_END_CHAR.ToString());

                if(parts.Length == QUERY_LENGTH)
                {
                    oqc.FieldName = parts[0];
                    oqc.Operator = GetOperator(parts[1]);
                    oqc.Value = this.Strip(parts[2]);
                }
                else if(parts.Length == EXTENDED_QUERY_LENGTH)
                {
                    oqc.FieldName = parts[0];
                    oqc.Operator = GetOperator(parts[1]);
                    oqc.Value = this.Strip(parts[2]);
                    oqc.SecondaryValue = this.Strip(parts[3]);

                    //check the operator
                    if(oqc.Operator != CriteriaOperator.Between)
                    {
                        throw new ArgumentException("Invalid operator or arguments for query " + query);
                    }
                }

                return oqc;
            }
            catch 
            {
                throw;
            }
        }



        /// <summary>
        /// Strips the placeholder's values from a querie's value
        /// </summary>
        /// <param name="queryValue">Query string to be stripped</param>
        /// <returns>Resulting string</returns>
        private string Strip(string queryValue)
        {
            if(queryValue.StartsWith(VALUE_START_CHAR.ToString()) && queryValue.EndsWith(VALUE_END_CHAR.ToString()))
            {
                queryValue = queryValue.Remove(0,1);
                queryValue = queryValue.Remove(queryValue.Length - 1);
            }

            return queryValue;
        }


      /// <summary>
      /// Splits the string in multiple pieces by a specific identifier and excludes 
      /// string between 2 known identifiers
      /// </summary>
      /// <param name="queryValue">The query value.</param>
      /// <param name="splitValue">The split value.</param>
      /// <param name="ignoredStartValue">The ignored start value.</param>
      /// <param name="ignoredEndValue"></param>
      /// <returns>Array of sliced strings</returns>
      public string[] SliceStringByContext(string queryValue,
                                           string splitValue,
                                           string ignoredStartValue,
                                           string ignoredEndValue )
      {
          //list which contains the valid parts of the query
          List<string> listParts = new List<string>();

          //holds the next index of the splitValue
          int indexOfValue = -1;

          //holds the last index of the splitValue
          int lastIndex = 0;

          //index used inside loop for parsing
          int innerIndex = -1;

          try
          {
              while (lastIndex < queryValue.Length)
              {
                  //get the index of the 
                  indexOfValue = queryValue.IndexOf(splitValue, lastIndex);

                  if (indexOfValue == -1)
                  {
                      //this is the last part so we just add it
                      listParts.Add(queryValue.Substring(lastIndex));
                      break;
                  }


                  //check now if we find the ignored value
                  string newValue = queryValue.Substring(lastIndex, indexOfValue - lastIndex);

                  //check it for ignored value
                  innerIndex = newValue.IndexOf(ignoredStartValue);

                  if (innerIndex == -1)
                  {
                      //normal string. Add it directly
                      listParts.Add(newValue);

                      lastIndex = indexOfValue + splitValue.Length;
                  }
                  else
                  {
                      //check if we have full match between the start and end splitValues
                      bool isMatch = this.IsFullMatch(newValue, ignoredStartValue, ignoredEndValue);

                      if(isMatch)
                      {
                          //we have a perfect match
                          listParts.Add(newValue);

                          //set the end index

                          //check if this is the last part of the query
                          if (lastIndex + newValue.Length + 1 <= queryValue.Length)
                          {
                              lastIndex += queryValue.Length;
                          }
                          else
                          {
                              lastIndex += newValue.Length + 1 + splitValue.Length;
                          }

                          continue;
                      }

                      //get the index of the endValue
                      int endIndex = queryValue.IndexOf(ignoredEndValue, lastIndex);

                      //rip it apart form the main string and make sure it includes the endValue (add +1) 
                      newValue = queryValue.Substring(lastIndex, endIndex - lastIndex + 1);

                      listParts.Add(newValue);

                      //set the new index

                      //check if this is the end of the string
                      if (endIndex + 1 >= queryValue.Length)
                      {
                          lastIndex = endIndex + 1;
                      }
                      else
                      {
                          //move the index to current and splitLength value
                          lastIndex = endIndex + 1 + splitValue.Length;

                      }
                  }
              }
              
              string[] vars = new string[listParts.Count];
              listParts.CopyTo(vars);

              return vars;

          }
          catch
          {
              throw;
          }
          finally
          {
              if (listParts != null)
              {
                  listParts.Clear();
                  listParts = null;
              }
          }
      }


      /// <summary>
      /// Determines whether [is full match] [the specified query].
      /// </summary>
      /// <param name="query">The query.</param>
      /// <param name="startValue">The start value.</param>
      /// <param name="endValue">The end value.</param>
      /// <returns>
      /// <c>true</c> if [is full match] [the specified query]; otherwise, <c>false</c>.
      /// </returns>
        private bool IsFullMatch(string query, string startValue, string endValue)
        {

            int start = 0;
            int end = 0;
            
            int currentPosition = -1;
            int startPosition = 0; 


            while ((currentPosition = query.IndexOf(startValue, startPosition)) != -1)
            {
                ++start;
                startPosition = currentPosition;
                ++startPosition;
            }

            currentPosition = -1;
            startPosition = 0;

            while( (currentPosition = query.IndexOf(endValue, startPosition)) != -1)
            {
                ++end;
                startPosition = currentPosition;
                ++startPosition;
            }

            return (start == end);
        }
        

        #region internal conversion functions
        /// <summary>
        /// Gets the operator from the specified string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private CriteriaOperator GetOperator(string value)
        {
            switch (value.ToLower())
            {
                case "=":
                    return CriteriaOperator.Equality;

                case "<":
                    return CriteriaOperator.Smaller;

                case "<=":
                    return CriteriaOperator.SmallerOrEqual;

                case ">":
                    return CriteriaOperator.Higher;

                case ">=":
                    return CriteriaOperator.HigherOrEqual;

                case "like":
                    return CriteriaOperator.Like;
                
                case "<>":
                    return CriteriaOperator.Different;

                case "between":
                    return CriteriaOperator.Between;


                default:
                    break;
            }

            throw new ArgumentException();
        }


        #region DBType conversions
        /// <summary>
        /// Checks if the specified type is numeric
        /// </summary>
        /// <param name="type">Type to be checked</param>
        /// <returns>True if the type is numeric</returns>
        public bool IsNumeric(System.Data.DbType type)
        {
            if (type == System.Data.DbType.Byte)
            {
                return true;
            }
            else if (type == System.Data.DbType.Currency)
            {
                return true;
            }
            else if (type == System.Data.DbType.Decimal)
            {
                return true;
            }
            else if (type == System.Data.DbType.Double)
            {
                return true;
            }
            else if (type == System.Data.DbType.Guid)
            {
                return true;
            }
            else if (type == System.Data.DbType.Int16)
            {
                return true;
            }
            else if (type == System.Data.DbType.Int32)
            {
                return true;
            }
            else if (type == System.Data.DbType.Int64)
            {
                return true;
            }
            else if (type == System.Data.DbType.SByte)
            {
                return true;
            }
            else if (type == System.Data.DbType.Single)
            {
                return true;
            }
            else if (type == System.Data.DbType.UInt16)
            {
                return true;
            }
            else if (type == System.Data.DbType.UInt32)
            {
                return true;
            }
            else if (type == System.Data.DbType.Int64)
            {
                return true;
            }
            else if (type == System.Data.DbType.VarNumeric)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Check if the specified type is a string
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsString(DbType type)
        {
            if (type == DbType.String)
            {
                return true;
            }
            else if (type == DbType.AnsiString)
            {
                return true;
            }
            else if (type == DbType.StringFixedLength)
            {
                return true;
            }


            return false;
        }


        /// <summary>
        /// Check if the specified type is DateTime
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsDateTime(DbType type)
        {
            if (type == DbType.DateTime || type == DbType.Date || type == DbType.Time)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the specified type is bool
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsBool(Type type)
        {
            if (type == typeof(System.Boolean))
            {
                return true;
            }

            return false;
        } 
        #endregion


        #region Type checks

        /// <summary>
        /// Checks if the specified type is numeric
        /// </summary>
        /// <param name="type">Type to be checked</param>
        /// <returns>True if the type is numeric</returns>
        public bool IsNumeric(Type type)
        {
            if (type ==  typeof(System.Byte) || type == typeof(System.SByte))
            {
                return true;
            }
            else if (type == typeof(System.Decimal) || type == typeof(System.Double) || type == typeof(System.Single))
            {
                return true;
            }
            else if (type == typeof( System.Int16) || type == typeof(System.Int32) || type == typeof(System.Int64))
            {
                return true;
            }
            else if(type == typeof(System.UInt16) || type == typeof(System.UInt32) || type == typeof(System.UInt64 ))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Check if the specified type is a string
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsString(Type type)
        {
            if (type == typeof(System.String))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Check if the specified type is DateTime
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsDateTime(Type type)
        {
            if (type == typeof(System.DateTime))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the specified type is bool
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsBool(DbType type)
        {
            if (type == DbType.Boolean)
            {
                return true;
            }

            return false;
        } 
        #endregion

        #endregion

        #endregion

    }
    
}