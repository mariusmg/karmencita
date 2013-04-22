/*
       file : ObjectReflection.cs
description : Implements reflection queries for fields/properties. It also
                  supports caching to skip multiple reflection queries.   
                  Also please note that Nullable<> types are NOT cached.
     author : Marius Gheorghe

    TODO:// type caching is broken now
  
*/ 

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;


namespace voidsoft.DataBlock.ObjectQuery
{

    /// <summary>
    /// Implements reflection searching for an object of specified type
    /// </summary>
    internal class ObjectReflection
    {

        #region fields
        //data caches
        private Dictionary<string, int> cachePropertyTypes = null;
        private Dictionary<string, int> cacheFieldsTypes = null; 
        #endregion
        

        #region ctor
        /// <summary>
        /// Creates a new instance of ObjectReflection
        /// </summary>
        /// <param name="cachePropertyTypes"></param>
        /// <param name="cacheFieldTypes"></param>
        public ObjectReflection(ref Dictionary<string, int> cachePropertyTypes,
                                ref Dictionary<string, int> cacheFieldTypes)
        {
            this.cacheFieldsTypes = cacheFieldTypes;
            this.cachePropertyTypes = cachePropertyTypes;
        }


        public ObjectReflection()
        {
        }

        #endregion
        
        
        #region Object reflection
        /// <summary>
        /// Gets the type and value of the specified field
        /// </summary>
        /// <typeparam name="T">Type who's fields are searched</typeparam>
        /// <param name="t"></param>
        /// <param name="fieldName">Name of the field</param>
        /// <param name="tp">The returned field's type</param>
        /// <param name="value">Value of the field</param>
        internal void GetTypeOfField<T>(T t, string fieldName, out Type tp, out object value)
        {
            //set it to null as a double check
            value = null;

            //search in properties
            this.SearchInProperties<T>(t, fieldName, out tp, out value);

            if (tp == null && value == null)
            {
                this.SearchInFields<T>(t, fieldName, out tp, out value);
            }

            if (tp == null && value == null)
            {
                //not found so the field name must be wrong.
                throw new ArgumentException("Invalid field with name : " + fieldName);
            }
        }


        /// <summary>
        /// Search for specified name in the fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="fieldName"></param>
        /// <param name="tp"></param>
        /// <param name="value"></param>
        private void SearchInFields<T>(T t, string fieldName, out Type tp, out object value)
        {
            FieldInfo[] fields = t.GetType().GetFields();

            int indexFields = -1;

            if (this.cacheFieldsTypes != null)
            {
                if (this.cacheFieldsTypes.ContainsKey(fieldName))
                {
                    foreach (KeyValuePair<string, int> var in this.cacheFieldsTypes)
                    {
                        ++indexFields;

                        if (var.Key == fieldName)
                        {
                            break;
                        }
                    }

                    //check the index.
                    if (indexFields > -1)
                    {
                        tp = fields[indexFields].FieldType;
                        value = fields[indexFields].GetValue(t);

                        return;
                    }
                }
            }
            
            
            //loop thru the fields 
            for (int i = 0; i < fields.Length; i++)
            {
                //check the name
                if (fields[i].Name == fieldName)
                {
                    #region nullable
                    //to check if it's nullable we first check if it's generic
                    if (fields[i].FieldType.IsGenericType)
                    {
                        //check if it's a nullable type
                        bool isNullable = (fields[i].FieldType.GetGenericTypeDefinition() == typeof(Nullable<>));

                        if (isNullable)
                        {
                            //check the underlying type for nullable
                            Type[] tps = fields[i].FieldType.GetGenericArguments();

                            tp = tps[0];
                            value = fields[i].GetValue(t);

                            return;
                        }
                    }
                    
                    #endregion

                    //normal type
                    tp = fields[i].FieldType;
                    value = fields[i].GetValue(t);

                    ////cache them
                    //if (cacheFieldsTypes != null)
                    //{
                    //    this.cacheFieldsTypes.Add(fieldName, i);
                    //}
                    
                    return;
                }
            }

            tp = null;
            value = null;
        }



        /// <summary>
        /// Search for specified field in properties
        /// </summary>
        /// <param name="t">Type of the object who's fields are queried</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="tp">Field's type</param>
        /// <param name="value">Field's value</param>
        private void SearchInProperties<T>(T t, string fieldName, out Type tp, out object value)
        {
            PropertyInfo[] properties = t.GetType().GetProperties();

            //index position in propertycache
            int indexProperties = -1;

            if (this.cachePropertyTypes != null)
            {
                //check in the property cache first
                if (this.cachePropertyTypes.ContainsKey(fieldName))
                {
                    this.cachePropertyTypes.TryGetValue(fieldName, out indexProperties);
                }
            }
            
            
            //check if we found it in property cache
            if (indexProperties > -1)
            {
                tp = properties[indexProperties].PropertyType;
                value = properties[indexProperties].GetValue(t, null);
                return;
            }
            else
            {
                //get the requested values and the cache the index
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].Name == fieldName)
                    {
                        tp = properties[i].PropertyType;
                        value = properties[i].GetValue(t, null);

                        //if(cachePropertyTypes != null)
                        //{
                        //    this.cachePropertyTypes.Add(fieldName, i);
                        //}
                         return;
                    }
                }
            }


            tp = null;
            value = null;
        }
        #endregion
          
    }
}
