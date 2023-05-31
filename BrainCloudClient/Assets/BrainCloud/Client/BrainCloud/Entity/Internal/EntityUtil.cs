//----------------------------------------------------
// brainCloud client source code
// Copyright 2016 bitHeads, inc.
//----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BrainCloud.Entity.Internal
{
    internal class EntityUtil
    {
        /** Method returns object as the asked for type. In the case of IList<T> and IDictionary<K,V>,
         * this method will return wrapper objects that masquerade as the asked for type.
         * For instance the underlying List we in entities is always a List<object>. But
         * we allow the user to pretend it's a List<T> where T is the type they wish to use.
         *
         * IList<int> li = myEntity.Get<IList<int>>("numbers");
         *
         * More complicated scenarioes arise when the user passes in nested type structures like:
         *
         * IDictionary<string, IDictionary<string, IList<int>>> ddli;
         *
         * @see ListWrapper and DictionaryWrapper classes for more info on how this is accomplished.
         */
        public static T GetObjectAsType<T>(object value)
        {
            Type type = typeof(T);

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    // the generic types T & S for our ListWrapper<T,S>
                    Type[] listGenericTypes = new Type[2];
                    listGenericTypes[0] = type.GetGenericArguments()[0];
                    listGenericTypes[1] = value.GetType().GetGenericArguments()[0];

                    // the parameters to the ListWrapper constructor
                    object[] parameters = new object[1];
                    parameters[0] = value;

                    // now call the ListWrapper constructor through reflection
                    Type genericTypeOpen = typeof(ListWrapper<,>);
                    Type genericTypeClosed = genericTypeOpen.MakeGenericType(listGenericTypes);
                    Type[] constructorTypes = new Type[1];
                    constructorTypes[0] = value.GetType();
                    ConstructorInfo ci = genericTypeClosed.GetConstructor(constructorTypes);

                    return (T)ci.Invoke(parameters);
                }
                if (type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    // the generic types TValue & SValue for our DictionaryWrapper<TValue, SValue>
                    Type[] dictGenericTypes = new Type[2];
                    dictGenericTypes[0] = type.GetGenericArguments()[1];
                    dictGenericTypes[1] = value.GetType().GetGenericArguments()[1];

                    // the parameters to the DictionaryWrapper constructor
                    object[] parameters = new object[1];
                    parameters[0] = value;

                    // now call the ListWrapper constructor through reflection
                    Type genericTypeOpen = typeof(DictionaryWrapper<,>);
                    Type genericTypeClosed = genericTypeOpen.MakeGenericType(dictGenericTypes);
                    Type[] constructorTypes = new Type[1];
                    constructorTypes[0] = value.GetType();
                    ConstructorInfo ci = genericTypeClosed.GetConstructor(constructorTypes);
                    return (T)ci.Invoke(parameters);
                }
            }

            T castedValue;
            try
            { 
                castedValue = (T) value;
            }
            catch(InvalidCastException)
            {
                castedValue = (T) Convert.ChangeType(value, typeof(T));
            }
            return castedValue;
        }
    }
}
