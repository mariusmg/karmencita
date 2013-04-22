/*
      file : ObjectQueryEvaluator.cs
description: Evaluator implementation for generic/OBJECT type
     author: Marius Gheorghe

 
 Notes :
   - it supports NULL comparation (with the equality operator)
   - Karmencita is CASE SENSITIVE
  
*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Reflection;
using voidsoft.DataBlock.ObjectQuery;
using voidsoft.DataBlock;


namespace voidsoft.DataBlock.ObjectQuery
{

    /// <summary>
    /// Evaluator implementation for generic types
    /// </summary>
    internal class ObjectQueryEvaluator : IEvaluator
    {

        #region constants

        internal const int SIZE_FIXED_CONDITION_ARGUMENTS = 3;
        internal const string NULL_KEYWORD = "null";
        internal const int FIXED_SIZE_ARGUMENTS = 2;


        //cache for already parsed property types. The key is the field's name
        //and the value it's his index
        private Dictionary<string, int> cachePropertyTypes = null;
        private Dictionary<string, int> cacheFieldsTypes = null;

        private ObjectReflection reflector = null;

        #endregion

        #region constructor
        /// <summary>
        /// Creates a new instance of ObjectQueryEvaluator
        /// </summary>
        public ObjectQueryEvaluator()
        {
            cachePropertyTypes = new Dictionary<string, int>();
            cacheFieldsTypes = new Dictionary<string, int>();
            reflector = new ObjectReflection(ref cachePropertyTypes, ref cacheFieldsTypes);
        }
        #endregion

        #region Evaluator implementation
        /// <summary>
        /// Evaluate the specified condition
        /// </summary>
        /// <typeparam name="T">Array of data structure which will be evaluated</typeparam>
        /// <param name="data">List of data to evaluate</param>
        /// <param name="condition">Condition for which data is evaluated</param>
        /// <returns></returns>
        public object Evaluate<T>(T t, object dataSource, string condition) 
        {
            //query parser
            QueryParser parser = null;

            //list which holds the results
            List<T> listResults = null;

            //list with query conditions
            List<QueryCondition> listConditions = null;

            try
            {
                listResults = new List<T>();
                parser = new QueryParser();

                //parse the condition and get the list of query conditions
                listConditions = parser.Parse(condition);

                //flag used to know the operation result
                bool result = false;

                IEnumerable ienum = (IEnumerable)dataSource;
                IEnumerator enumerator = ienum.GetEnumerator();


                //internal vars for each parsed line
                object currentValue = null;
                Type type = null;


                while (enumerator.MoveNext())
                {
                    //reset the flag
                    result = false;

                    //loop thru the queries and check the fields with its associated query
                    foreach (QueryCondition currentCondition in listConditions)
                    {
                        currentValue = null;
                        type = null;

                        this.reflector.GetTypeOfField<T>( (T) enumerator.Current , currentCondition.FieldName, out type, out currentValue);

                        if (type == null)
                        {
                            throw new ArgumentException("Invalid field name " + currentCondition.FieldName);
                        }

                        //evaluate the specific type
                        result = EvaluateType(parser, currentValue, type, currentCondition);

                        //if it fails there's no point on evaluation the other conditions so break
                        if (result == false)
                        {
                            //but before breaking check for conditional OR queries
                            if (currentCondition.ListConditions != null)
                            {
                                foreach (QueryCondition var in currentCondition.ListConditions)
                                {
                                    bool innerResult = false;
                                    currentValue = null;
                                    type = null;

                                    this.reflector.GetTypeOfField<T>((T)enumerator.Current, var.FieldName, out type, out currentValue);

                                    if (type == null)
                                    {
                                        throw new ArgumentException("Invalid field name " + currentCondition.FieldName);
                                    }

                                    //evaluate the specific type
                                    innerResult = EvaluateType(parser, currentValue, type, var);

                                    if (innerResult == true)
                                    {
                                        //found a match so it passes
                                        result = true;
                                        break;
                                    }
                                }
                            }

                            break;
                        }
                    }

                    if (result == true)
                    {
                        listResults.Add( (T) enumerator.Current);
                    }
                }

                T[] results = new T[listResults.Count];
                listResults.CopyTo(results);

                return results;
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


        /// <summary>
        /// Evaluates the minimum value
        /// </summary>
        /// <param name="t">Generic type</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public object EvaluateMin<T>(T t, object dataSource, string propertyName)
        {
            QueryParser parser = null;
            ObjectReflection reflector = null;

            //check the subtype now
            object value;
            Type tp;

            List<object> list = null;

            try
            {
                parser = new QueryParser();
                reflector = new ObjectReflection();
                list = new List<object>();

                T typeInstance = this.GetTypeInstance<T>(dataSource);

                //get the type of the field
                reflector.GetTypeOfField(typeInstance, propertyName, out tp, out value);

                if (parser.IsNumeric(tp))
                {
                    decimal resultValue = this.EvaluateNumeric<T>(dataSource, propertyName, true);
                    return resultValue;
                }
                else if (parser.IsDateTime(tp))
                {
                    DateTime resultDate = this.EvaluateDateTime<T>(dataSource, propertyName, true);
                    return resultDate;
                }
                else
                {
                    //quick check to see if out type supports IComparable.
                    //Because we have a RuntimeType we cannot cast it to Icomparable
                    //so we check it with reflection

                    Type icTest = tp.GetInterface("System.IComparable");

                    if (icTest == null)
                    {
                        throw new ArgumentException("Invalid type. Must implement IComparable to be evaluated");
                    }

                    return this.EvaluateGeneric<T>(dataSource, propertyName, true);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (list != null)
                {
                    list.Clear();
                    list = null;
                }
            }
        }



        /// <summary>
        /// Evaluates the minimum value
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="properyName">Name of the propery.</param>
        /// <returns></returns>
        public object EvaluateMin<T>(T t, object dataSource, string properyName, string condition)
        {
            try
            {
                object source = this.Evaluate<T>(t, dataSource, condition);
                return this.EvaluateMin<T>(t, source, properyName);
            }
            catch
            {
                throw;
            }
        }

        
        /// <summary>
        /// Evaluates the minimum value
        /// </summary>
        /// <param name="t">Generic type</param>
        /// <param name="dataSource">The data source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public object EvaluateMax<T>(T t, object dataSource, string propertyName)
        {
            QueryParser parser = null;
            ObjectReflection reflector = null;

            //check the subtype now
            object value;
            Type tp;

            try
            {
                parser = new QueryParser();
                reflector = new ObjectReflection();

                T typeInstance = this.GetTypeInstance<T>(dataSource);

                //get the type of the field
                reflector.GetTypeOfField(typeInstance, propertyName, out tp, out value);

                if (parser.IsNumeric(tp))
                {
                    decimal resultValue = this.EvaluateNumeric<T>(dataSource, propertyName, false);
                    return resultValue;
                }
                else if (parser.IsDateTime(tp))
                {
                    DateTime resultDate = this.EvaluateDateTime<T>(dataSource, propertyName, false);
                    return resultDate;
                }
                else
                {
                    //quick check to see if out type supports IComparable.
                    //Because we have a RuntimeType we cannot cast it to Icomparable
                    //so we check it with reflection

                    Type icTest = tp.GetInterface("System.IComparable");

                    if (icTest == null)
                    {
                        throw new ArgumentException("Invalid type. Must implement IComparable to be evaluated");
                    }

                    return this.EvaluateGeneric<T>(dataSource, propertyName, false);
                }
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
        /// <param name="dataSource">The data source.</param>
        /// <param name="properyName">Name of the propery.</param>
        /// <param name="condition">Query</param>
        /// <returns></returns>
        public object EvaluateMax<T>(T t, object dataSource, string properyName, string condition)
        {
            try
            {
                object source = this.Evaluate<T>(t, dataSource, condition);
                return this.EvaluateMax<T>(t, source, properyName);
            }
            catch
            {
                throw;
            }
        }



        /// <summary>
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public decimal Avg<T>(object dataSource, string fieldName)
        {
            decimal result = this.Sum<T>(dataSource, fieldName);

            decimal index = -1;

            IEnumerator ienum = ((IEnumerable)dataSource).GetEnumerator();

            while (ienum.MoveNext())
            {
                ++index;
            }

            return result / index;
        }


        /// <summary>
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public decimal Sum<T>(object dataSource, string fieldName)
        {
            ObjectQueryEvaluator objectEvaluator = null;
            QueryParser parser = null;
            ObjectReflection reflector = null;

            decimal result = 0;

            try
            {
                //check the subtype now
                object value;
                Type tp;

                //it's generic object
                objectEvaluator = new ObjectQueryEvaluator();
                parser = new QueryParser();
                reflector = new ObjectReflection();

                T typeInstance = this.GetTypeInstance<T>(dataSource);

                reflector.GetTypeOfField(typeInstance, fieldName, out tp, out value);

                if (!parser.IsNumeric(tp))
                {
                    throw new ArgumentException("Invalid type for property. Only numeric fields are accepted.");
                }
                else
                {
                    IEnumerator ienum = ((IEnumerable)dataSource).GetEnumerator();

                    while (ienum.MoveNext())
                    {
                        reflector.GetTypeOfField((T)ienum.Current, fieldName, out tp, out value);

                        //add it to the result
                        result += Convert.ToDecimal(value);
                    }
                }

                return result;
            }
            catch
            {
                throw;
            }
        }



        /// <summary>
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="fieldName"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public decimal Sum<T>(object dataSource, string fieldName, string query)
        {
            try
            {
                T defaultType = default(T);

                object source  = this.Evaluate<T>(defaultType, dataSource, query);

                return this.Sum<T>(source, fieldName);
            }
            catch
            {
                throw;
            }
        }
        #endregion



        #region Type evaluator
	
	    /// <summary>
        /// Evaluates the specific type
        /// </summary>
        /// <param name="parser">QueyParser</param>
        /// <param name="currentValue">Value of type to be evaluated</param>
        /// <param name="type">Type</param>
        /// <param name="currentCondition">QueryCondition</param>
        /// <returns>Flag </returns>
        private bool EvaluateType(QueryParser parser,
                                  object currentValue, 
                                  Type type,
                                  QueryCondition currentCondition)
        {
            bool result = false;

            //check the field's type and call the evaluator.
            if (parser.IsNumeric(type))
            {
                result = this.EvaluateNumeric(currentValue, currentCondition);
            }
            else if (parser.IsString(type))
            {
                result = this.EvaluateString(currentValue, currentCondition);
            }
            else if (parser.IsDateTime(type))
            {
                result = this.EvaluateDateTime(currentValue, currentCondition);
            }
            else if (parser.IsBool(type))
            {
                result = this.EvaluateBool(currentValue, currentCondition);
            }
            else
            {
                //try to evaluate a IComparable type
                result = this.EvaluateIComparable(currentValue, type, currentCondition);
            }

            return result;
        }

        #endregion


        #region ObjectQuery evaluators for supported types


        /// <summary>
        /// Evaluates the specified IComparable object
        /// </summary>
        /// <param name="value">Value of the object to be evaluated</param>
        /// <param name="t">Type of object</param>
        /// <param name="queryCondition">QueryCondition upon which the object is evaluated</param>
        /// <returns>Evaluation result</returns>
        internal bool EvaluateIComparable(object value,
                                          Type t,  
                                          QueryCondition queryCondition)
        {
            bool passed = false;


            object conditionInstance = null;

            if (queryCondition.Value != NULL_KEYWORD)
            {
                conditionInstance = this.CreateTypeInstance(queryCondition.Value, t);
            }

            //check if the type implements IComparable
            //TODO: add support for IComparable<>
            IComparable ic = null;
            try
            {
                ic = (IComparable)value;
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid type " + t.Name + " . Must implement IComparable to be evaluated");
            }


            //holds the comparations result
            int result;

            switch (queryCondition.Operator)
            {
                case CriteriaOperator.Between:
                    object conditionSecondInstance = this.CreateTypeInstance(queryCondition.SecondaryValue, t);

                    int firstComparation = ic.CompareTo(conditionInstance);
                    int secondComparation = ic.CompareTo(conditionSecondInstance);

                    if (firstComparation >= 0 && secondComparation <= 0)
                    {
                        passed = true;
                    }
                    break;

                case CriteriaOperator.Equality:
                    //check for null comparation
                    if (queryCondition.Value == NULL_KEYWORD)
                    {
                        if (value == null)
                        {
                            passed = true;
                            break;
                        }
                    }

                    result = ic.CompareTo(conditionInstance);

                    if (result == 0)
                    {
                        passed = true;
                    }
                    break;

                    
                case CriteriaOperator.Different:
                    result = ic.CompareTo(conditionInstance);

                    if (result != 0)
                    {
                        passed = true;
                    }
                    break;


                case CriteriaOperator.Smaller:
                    result = ic.CompareTo(conditionInstance);

                    if (result == -1)
                    {
                        passed = true;
                    }
                    break;
                

                case CriteriaOperator.SmallerOrEqual:
                    result = ic.CompareTo(conditionInstance);

                    if (result <= 0)
                    {
                        passed = true;
                    }
                    break;

                case CriteriaOperator.Higher:
                    result = ic.CompareTo(conditionInstance);

                    if (result > 0)
                    {
                        passed = true;
                    }
                    break;

                case CriteriaOperator.HigherOrEqual:
                    result = ic.CompareTo(conditionInstance);

                    if (result >= 0)
                    {
                        passed = true;
                    }
                    break;

                default:
                    break;
            }

            return passed;
        }


        /// <summary>
        /// Evaluates the specified numeric expression
        /// </summary>
        /// <param name="fieldValue">Value to be evaluated</param>
        /// <param name="condition">QueryCondition for which the evaluation is done</param>
        /// <returns>Returns true if evaluation succedes</returns>
        internal bool EvaluateNumeric(object fieldValue,
                                     QueryCondition condition)
        {

            bool passed = false;

            switch (condition.Operator)
            {
                case CriteriaOperator.Like:
                    //invalid operator for numeric 
                    throw new ArgumentException("Like keyword cannot be used for numeric comparations");

                case CriteriaOperator.Equality:

                    //first check for null
                    if (condition.Value == NULL_KEYWORD)
                    {
                        //check for nulls
                        if (fieldValue == null || fieldValue == DBNull.Value)
                        {
                            passed = true;
                            break;
                        }
                        else
                        {
                            passed = false;
                            break;
                        }
                    }

                    if (Convert.ToDecimal(fieldValue) == Convert.ToDecimal(condition.Value))
                    {
                        passed = true;
                    }
                    break;

                case CriteriaOperator.Different:
                    //check for null
                    if (condition.Value == NULL_KEYWORD)
                    {
                        if (fieldValue != null && fieldValue != DBNull.Value)
                        {
                            passed = true;
                            break;
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(fieldValue) != Convert.ToDecimal(condition.Value))
                        {
                            passed = true;
                        }

                    }
                    break;

                case CriteriaOperator.Smaller:
                    if (Convert.ToDecimal(fieldValue) < Convert.ToDecimal(condition.Value))
                    {
                        passed = true;
                    }
                    break;

                case CriteriaOperator.SmallerOrEqual:
                    if (Convert.ToDecimal(fieldValue) <= Convert.ToDecimal(condition.Value))
                    {
                        passed = true;
                    }
                    break;

                case CriteriaOperator.Higher:
                    if (Convert.ToDecimal(fieldValue) > Convert.ToDecimal(condition.Value))
                    {
                        passed = true;
                    }
                    break;

                case CriteriaOperator.HigherOrEqual:
                    if (Convert.ToDecimal(fieldValue) >= Convert.ToDecimal(condition.Value))
                    {
                        passed = true;
                    }
                    break;

                case CriteriaOperator.Between:
                    decimal var = Convert.ToDecimal(fieldValue);

                    if (var >= Convert.ToDecimal(condition.Value) && var <= Convert.ToDecimal(condition.SecondaryValue))
                    {
                        passed = true;
                    } 
                    break;


                default:
                    break;
            }

            return passed;
        }



        //NOTE: string comparation is case sensitive

        /// <summary>
        /// Evaluates the specified string.
        /// </summary>
        /// <param name="fieldValue">Value to be evaluated</param>
        /// <param name="condition">QueryCondition upon which the evaluation is made</param>
        /// <returns>Returns true if the evaluation succedes</returns>
        internal bool EvaluateString(object fieldValue,
                                     QueryCondition condition)
        {

            bool passed = false;


            switch (condition.Operator)
            {

                case CriteriaOperator.Like:
                    //evaluate like. The operator is "%". If the condition starts with the operator the string is
                    //evaluated with EndsWith. If the string ends with the operator the string is evaluated with 
                    //StartWith. If the % is in the middle the string is evaluated with IndexOf.

                    int operatorIndex = condition.Value.IndexOf("%");

                    if (operatorIndex == -1)
                    {
                        throw new ArgumentException("Operator % not found in string for LIKE comparation");
                    }


                    if (condition.Value.StartsWith("%"))
                    {
                        //get a string which does NOT contains the "%" operator
                        string tempValue = condition.Value.Substring(1);

                        //evaluate with ends
                        if (fieldValue.ToString().EndsWith(tempValue))
                        {
                            passed = true;
                            break;
                        }
                    }
                    else if (condition.Value.EndsWith("%"))
                    {
                        //get a string which does NOT contains the "%" operator
                        string tempValue = condition.Value.Substring(0, condition.Value.Length - 1);

                        //evaluate with ends
                        if (fieldValue.ToString().StartsWith(tempValue))
                        {
                            passed = true;
                            break;
                        }
                    }
                    else
                    {
                        //split the string.
                        string[] args = Regex.Split(condition.Value, "%");

                        //flag used to know if it's ok
                        bool containsEverything = true;

                        if (args.Length == FIXED_SIZE_ARGUMENTS)
                        {
                            //we check if the string starts and ends with the specified data
                            if (fieldValue.ToString().StartsWith(args[0]) && fieldValue.ToString().EndsWith(args[1]))
                            {
                                passed = true;
                            }
                            else
                            {
                                passed = false;
                            }
                        }
                        else
                        {
                            //we have more than one LIKE operators so we must check if
                            //the string starts and ends with the specific data but also
                            //if the string contains the additional data.

                            //first check the start and end
                            if (!fieldValue.ToString().StartsWith(args[0]) && fieldValue.ToString().EndsWith(args[args.Length - 1]))
                            {
                                containsEverything = false;
                            }

                            if (containsEverything)
                            {
                                //check the intermediary values
                                for (int i = 1; i < args.Length - 1; i++)
                                {
                                    int index = condition.Value.IndexOf(args[i]);

                                    if (index == -1)
                                    {
                                        containsEverything = false;
                                        break;
                                    }
                                }
                            }

                            passed = containsEverything;
                        }
                    }
                    break;


                case CriteriaOperator.Equality:
                    //check for null
                    if (condition.Value == NULL_KEYWORD)
                    {
                        //is null
                        if (fieldValue == null)
                        {
                            passed = true;
                            break;
                        }
                    }
                    else
                    {
                        if (fieldValue.ToString() == condition.Value)
                        {
                            passed = true;
                        }
                    }
                    break;

                case CriteriaOperator.Different:
                    //check for null
                    if (condition.Value == NULL_KEYWORD)
                    {
                        //is null
                        if (fieldValue != null || fieldValue != DBNull.Value)
                        {
                            passed = true;
                            break;
                        }
                    }
                    else
                    {
                        if (fieldValue.ToString() != condition.Value)
                        {
                            passed = true;
                        }
                    }
                    break;


                case CriteriaOperator.Smaller:
                case CriteriaOperator.SmallerOrEqual:
                case CriteriaOperator.Higher:
                case CriteriaOperator.HigherOrEqual:
                case CriteriaOperator.Between:
                    throw new ArgumentException("Invalid operator for string comparation");

                default:
                    passed = false;
                    break;
            }

            return passed;
        }
        /// <summary>
        /// Evaluates the specified DateTime fieldValue
        /// </summary>
        /// <param name="fieldValue">Value to be evaluated</param>
        /// <param name="condition">QueryCondition upon which the evaluation is made</param>
        /// <returns>Returns true if the evaluation succedes</returns>
        internal bool EvaluateDateTime(object fieldValue,
                                       QueryCondition condition)
        {
            bool passed = false;

            DateTime? dtFieldValue = Convert.ToDateTime(fieldValue);
            DateTime? dateToBeCompared = Convert.ToDateTime(condition.Value);

            //holds the comparation's result
            int result = -2;

            switch (condition.Operator)
            {
                case CriteriaOperator.Like:
                    throw new ArgumentException("Invalid operator for a DateTime type");

                case CriteriaOperator.Equality:

                    //check for null
                    if (condition.Value == NULL_KEYWORD)
                    {
                        if (fieldValue == null || fieldValue == DBNull.Value)
                        {
                            //the fieldValue is null
                            passed = true;
                        }
                    }
                    else
                    {
                        DateTime dateToCompare = Convert.ToDateTime(condition.Value);
                        result = DateTime.Compare((DateTime)dtFieldValue, dateToCompare);
                        passed = result == 0 ? true : false;
                    }
                    break;

                case CriteriaOperator.Different:
                    //check for null
                    if (condition.Value == NULL_KEYWORD)
                    {
                        if (fieldValue != null || fieldValue != DBNull.Value)
                        {
                            passed = true;
                        }
                    }
                    else
                    {
                        DateTime dateToCompare = Convert.ToDateTime(condition.Value);
                        result = DateTime.Compare((DateTime)dtFieldValue, dateToCompare);
                        passed = result != 0 ? true : false;
                    }
                    break;


                case CriteriaOperator.Smaller:
                    result = DateTime.Compare((DateTime)dtFieldValue, (DateTime)dateToBeCompared);
                    passed = result < 0 ? true : false;

                    break;

                case CriteriaOperator.SmallerOrEqual:
                    result = DateTime.Compare((DateTime)fieldValue, (DateTime)dateToBeCompared);
                    passed = result <= 0 ? false : true;

                    break;

                case CriteriaOperator.Higher:
                    result = DateTime.Compare((DateTime)fieldValue, (DateTime)dateToBeCompared);
                    passed = result > 0 ? true : false;

                    break;

                case CriteriaOperator.HigherOrEqual:
                    result = DateTime.Compare((DateTime)fieldValue, (DateTime)dateToBeCompared);

                    passed = result >= 0 ? false : true;
                    break;


                case CriteriaOperator.Between:
                    DateTime data = Convert.ToDateTime(fieldValue);

                    int firstResult = DateTime.Compare(data, Convert.ToDateTime(condition.Value));
                    int lastResult = DateTime.Compare(data, Convert.ToDateTime(condition.SecondaryValue));

                    if (firstResult >= 0 && lastResult <= 0)
                    {
                        passed = true;
                    }
                    break;



                default:
                    passed = false;
                    break;
            }

            return passed;
        }

        /// <summary>
        /// Evaluates the specified boolean expression
        /// </summary>
        /// <param name="fieldValue">Value to be evaluated</param>
        /// <param name="condition">QueryCondition upon which the evaluation is made</param>
        /// <returns>Returns true if the evaluation succedes</returns>
        internal bool EvaluateBool(object fieldValue,
                                   QueryCondition condition)
        {
            bool passed = false;

            switch (condition.Operator)
            {
                case CriteriaOperator.Equality:
                    //check for null
                    if (condition.Value == NULL_KEYWORD)
                    {
                        //is null
                        if (fieldValue == null || fieldValue == DBNull.Value)
                        {
                            passed = true;
                        }
                    }
                    else
                    {
                        if (Convert.ToBoolean(fieldValue) == Convert.ToBoolean(condition.Value))
                        {
                            passed = true;
                        }
                    }
                    break;


                case CriteriaOperator.Different:

                    //check for null
                    if (condition.Value == NULL_KEYWORD)
                    {
                        //is null
                        if (fieldValue != null || fieldValue != DBNull.Value)
                        {
                            passed = true;
                        }
                    }
                    else
                    {
                        if (Convert.ToBoolean(fieldValue) != Convert.ToBoolean(condition.Value))
                        {
                            passed = true;
                        }
                    }
                    break;


                case CriteriaOperator.Like:
                case CriteriaOperator.Smaller:
                case CriteriaOperator.SmallerOrEqual:
                case CriteriaOperator.Higher:
                case CriteriaOperator.HigherOrEqual:
                case CriteriaOperator.Between:
                    throw new ArgumentException("Invalid operator for boolean comparation");

                default:
                    passed = false;
                    break;
            }

            return passed;
        }
        #endregion


        #region ObjectQuery evaluators for list of objects. Mostly IComparable operations

        /// <summary>
        /// Evaluates the specified DateTime expression
        /// </summary>
        /// <param name="data">Data source</param>
        /// <param name="propertyName">DataField upon which the evaluation is being done</param>
        /// <param name="evaluateMinimum">Flag used to know if we evaluate the min value.If false the MAX value is evaluated</param>
        /// <returns>Selected data</returns>
        public DateTime EvaluateDateTime<T>(object dataSource,
                                            string propertyName,
                                            bool evaluateMinimum)
        {
            DateTime result = new DateTime(1, 1, 1);

            DateTime currentValue = new DateTime();

            Type tp = null;
            object value = null;
            bool isFirstIndex = true;

            IEnumerator ienum = ((IEnumerable)dataSource).GetEnumerator();

            while(ienum.MoveNext())
            {
                //reset the current value
                currentValue = new DateTime(1, 1, 1);

                reflector.GetTypeOfField( (T) ienum.Current, propertyName, out tp, out value);

                if (value != null)
                {
                    currentValue = Convert.ToDateTime(value);
                }

                int comparasionResult = DateTime.Compare(currentValue, result);

                //check if this is the first index.
                if (isFirstIndex)
                {
                    result = Convert.ToDateTime(value);
                    isFirstIndex = false;
                }
                else
                {
                    //check if we evaluate min || max
                    if (evaluateMinimum == true)
                    {
                        if (comparasionResult < 0)
                        {
                            result = Convert.ToDateTime(value);
                        }
                    }
                    else
                    {
                        //evaluate maximum
                        if (comparasionResult > 0)
                        {
                            result = Convert.ToDateTime(value);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Evaluates the specified numeric expression
        /// </summary>
        /// <typeparam name="T">Type of parameter for which the evaluation is being done</typeparam>
        /// <param name="data">Data source</param>
        /// <param name="propertyName">DatabaseField for which evaluation is done</param>
        /// <param name="evaluateMinimum">Flag used to know if we evaluate the MIN version. If false the MAX version is evaluated</param>
        /// <returns>Selected data</returns>
        public decimal EvaluateNumeric<T>(object dataSource,
                                          string propertyName,
                                          bool evaluateMinimum)
        {
            decimal result = 0;
            decimal value = 0;

            Type tp = null;
            object returnedValue = null;

            bool isFirstIndex = true;


            IEnumerator ienum = ((IEnumerable)dataSource).GetEnumerator();

            while (ienum.MoveNext())
            {
                reflector.GetTypeOfField((T) ienum.Current , propertyName, out tp, out returnedValue);

                if (returnedValue != null)
                {
                    value = Convert.ToDecimal(returnedValue);

                    if (isFirstIndex)
                    {
                        result = value;
                        isFirstIndex = false;
                    }
                    else
                    {
                        //check if we evaluate min || max
                        if (evaluateMinimum == true)
                        {
                            if (value < result)
                            {
                                result = value;
                            }
                        }
                        else
                        {
                            //evaluate maximum
                            if (value > result)
                            {
                                result = value;
                            }
                        }
                    }
                }
            }

            return result;
        }



        /// <summary>
        /// Evaluates a data source of generic types
        /// </summary>
        /// <param name="dataSource">The data source</param>
        /// <param name="evaluateMinimum">Flag used to know if the min is evaluated</param>
        /// <returns>Resulting Type</returns>
        /// <typeparam name="T">Type to be evaluated</typeparam>
        public T EvaluateGeneric<T>(object dataSource,
                                    bool evaluateMinimum) 
        {
            T current = default(T);
            int i = 0; //counter var

            IEnumerator ienum = ((IEnumerable)dataSource).GetEnumerator();

            while (ienum.MoveNext())
            {
                if (i == 0)
                {
                    current = (T)ienum.Current;
                    ++i;
                    continue;
                }

                int result = ((IComparable)ienum.Current).CompareTo(current);

                //check for evaluation mode
                if (evaluateMinimum)
                {
                    if (result == -1)
                    {
                        current = (T)ienum.Current;
                        ++i;
                    }
                }
                else
                {
                    if (result > 0)
                    {
                        current = (T)ienum.Current;
                        ++i;
                    }
                }
            }

            return current;
        }




        public T EvaluateGeneric<T>(object dataSource,
                                         string propertyName,
                                         bool evaluateMinimum)
        {
            ObjectReflection reflector = null;
            object current = null;
            int i = 0; //index counter
            int selectedIndex = -1; //selected index 

            try
            {
                reflector = new ObjectReflection();

                Type tp = null;
                object value = null;

                IEnumerator ienum = ((IEnumerable)dataSource).GetEnumerator();

                while (ienum.MoveNext())
                {
                    T instance = (T)ienum.Current;

                    reflector.GetTypeOfField<T>(instance, propertyName, out tp, out value);

                    if (i == 0)
                    {
                        current = value;
                        selectedIndex = i;
                        ++i;
                        continue;
                    }

                    int result = ((IComparable)value).CompareTo(current);

                    if (evaluateMinimum)
                    {
                        if (result == -1)
                        {
                            current = value;
                            selectedIndex = i;
                            ++i;
                        }
                    }
                    else
                    {
                        if (result > 0)
                        {
                            current = value;
                            selectedIndex = i;
                            ++i;
                        }
                    }
                }

                //get the item at the selected index
                ienum = ((IEnumerable)dataSource).GetEnumerator();
                i = -1;

                while (ienum.MoveNext())
                {
                    ++i;

                    if (i == selectedIndex)
                    {
                        current = ienum.Current;
                        break;
                    }
                }


                return (T) current;
            }
            catch
            {
                throw;
            }
        }
        #endregion


        #region Type instaces
        /// <summary>
        /// Creates a new instance of the specified type and also
        /// checks if the type implements IComparable.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private object CreateTypeInstance(string value, Type t)
        {
            Type icTest = t.GetInterface("System.IComparable");

            if (icTest == null)
            {
                throw new ArgumentException("Invalid type " + t.Name + " . Must implement IComparable to be evaluated");
            }

            //first parse the value which can contain multiple arguments
            string[] args = Regex.Split(value, ",");

            object[] arguments = null;

            try
            {
                arguments = this.GetConstructorParameters(t, args);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("The type " + t.Name + " does not have a constructor defined who's number of arguments correspond with the arguments defined in the criteria");
            }
            catch
            {
                throw;
            }
            object instance = Activator.CreateInstance(t, arguments);

            return instance;
        }


        /// <summary>
        /// Returns an array of objects based on the object's constructor
        /// </summary>
        /// <param name="t">Object type</param>
        /// <param name="vars">Constructor arguments</param>
        /// <returns></returns>
        private object[] GetConstructorParameters(Type t, 
                                                 string[] vars)
        {
            try
            {
                object[] result = new object[vars.Length];

                ConstructorInfo[] constr = t.GetConstructors();

                ParameterInfo[] paramsArray = null;

                ///loop and keep the first constructor who's number
                ///of parameters match our string[]
                for (int i = 0; i < constr.Length; i++)
                {
                    ParameterInfo[] param = constr[i].GetParameters();

                    if (param.Length == vars.Length)
                    {
                        paramsArray = param;
                        break;
                    }
                }

                if (paramsArray == null)
                {
                    throw new ArgumentException();
                }

                //create the object[]
                for (int i = 0; i < paramsArray.Length; i++)
	    		{
                    //get the parameter type and create a instance of that type
                    Type newType = paramsArray[i].ParameterType;
                    object res = Activator.CreateInstance(newType);

                    //change the subtype 
                    res = Convert.ChangeType(vars[i], newType);

                    //set the (string) value
                    result[i] = res;
                }

                return result;
            }
            catch 
            {
                throw;
            }
        }



        /// <summary>
        /// Gets the first instance of the type from the dataSource
        /// </summary>
        /// <param name="dataSource">The data source</param>
        /// <returns>Instance of the type</returns>
        private T GetTypeInstance<T>(object dataSource)
        {
            try
            {
                IEnumerator ienum = ((IEnumerable)dataSource).GetEnumerator();

                while (ienum.MoveNext())
                {
                    return ((T)ienum.Current);                    
                }

                return default(T);
            }
            catch
            {
                throw;
            }
        }

        #endregion

    }
}
