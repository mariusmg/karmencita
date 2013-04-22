/*
 
           file : IEvaluator.cs
description : Interface which defines the evaluators implementation.
      author : Marius Gheorghe 
  
*/ 


using System;
using System.Collections.Generic;
using System.Text;

namespace voidsoft.DataBlock.ObjectQuery
{

    /// <summary>
    /// Interface which defines the evaluation implementation.
    /// </summary>
    internal interface IEvaluator
    {
        object Evaluate<T>(T t,object dataSource, string condition);

        object EvaluateMin<T>(T t, object dataSource, string fieldName);

        object EvaluateMin<T>(T t, object dataSource, string fieldName, string query);

        object EvaluateMax<T>(T t, object dataSource, string fieldName);

        object EvaluateMax<T>(T t, object dataSource, string fieldName, string query);

        decimal Avg<T>(object dataSource, string fieldName);

        decimal Sum<T>(object dataSource, string fieldName);

        decimal Sum<T>(object dataSource, string fieldName, string query);
    }
}
