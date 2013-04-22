/*

       file: CriteriaOperator.cs
description: Defines criteria operators.
     author: Marius Gheorghe


*/

using System;
using System.Text;


namespace voidsoft.DataBlock.ObjectQuery
{
    /// <summary>
    /// Criteria operators
    /// </summary>
    internal enum CriteriaOperator
    {

        /// <summary>
        /// Order By operator
        /// </summary>
        OrderBy,  //Ordery BY

        /// <summary>
        /// DISTINCT operator
        /// </summary>
        Distinct,  //DISTINCT

        /// <summary>
        /// BETWEEN operator
        /// </summary>
        Between, //BETWEEN
        
        /// <summary>
        /// NOT operator
        /// </summary>
        Not,  //NOT

        /// <summary>
        /// LIKE operator
        /// </summary>
        Like,  //LIKE

        /// <summary>
        /// Equality operator
        /// </summary>
        Equality, // =

        ///<summary>
        ///Different operator
        ///</sumarry>
        Different,  // <>

        /// <summary>
        /// IsNull operator
        /// </summary>
        IsNull,

        /// <summary>
        /// IsnotNull operator
        /// </summary>
        IsNotNull,

        /// <summary>
        /// Or operator
        /// </summary>
        Or, //OR

        /// <summary>
        /// Smaller operator 
        /// </summary>
        Smaller, // <

        /// <summary>
        /// SmallerOrEqual operator
        /// </summary>
        SmallerOrEqual, // <=

        /// <summary>
        /// Higher operator
        /// </summary>
        Higher,     //>

        /// <summary>
        /// 
        /// </summary>
        HigherOrEqual,  //>=

        /// <summary>
        /// MAX operator
        /// </summary>
        Max,        //MAX

        /// <summary>
        /// MIN operator
        /// </summary>
        Min,        //MIN

        /// <summary>
        /// Count operator
        /// </summary>
        Count       //COUNT
    }
}
