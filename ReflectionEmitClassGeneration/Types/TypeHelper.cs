using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReflectionEmitClassGeneration.Types
{
    public static class TypeHelper
    {
        public static Dictionary<string, Type> GetPublicProperties(System.Collections.IEnumerable itemSource)
        {
            var type = itemSource.GetType().GetTypeInfo();
            var publicProperties = type.GetProperties(BindingFlags.Public);
            var propertiesDict = publicProperties.ToDictionary(k => k.Name, v => v.PropertyType);
            return propertiesDict;
        }
    }
}
