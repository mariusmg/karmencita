/*

       file : QueryCondition.cs
description : Represents a high level query condition
     author : Marius Gheorghe 
  
*/ 

using System;
using System.Collections.Generic;
using System.Text;
using voidsoft.DataBlock.ObjectQuery;



namespace voidsoft.DataBlock.ObjectQuery
{

    /// <summary>
    /// Describes a query condition
    /// </summary>
    internal struct QueryCondition
    {
        /// <summary>
        /// Name of the field
        /// </summary>
        public string FieldName;

        /// <summary>
        /// Criteria operator
        /// </summary>
        public CriteriaOperator Operator;

        /// <summary>
        ///Query condition value
        /// </summary>
        public string Value;

        /// <summary>
        /// Query secondary value. Only used with the BETWEEN operator
        /// </summary>
        public string SecondaryValue;

        /// <summary>
        /// List used to hold the OR conditions of a query
        /// </summary>
        public List<QueryCondition> ListConditions;
    }
}
