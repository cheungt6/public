using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ReflectionEmitClassGeneration.Types
{
    public class TypeGenerator : TypeGeneratorGeneric<object>
    {
        public TypeGenerator(Dictionary<string, Type> properties) : base(properties)
        {
        }
    }

    /// <summary>
    /// Generate a new type that uses T as the base class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypeGeneratorGeneric<T> where T : class
    {
        private readonly Dictionary<string, MethodInfo> _setMethods;

        public TypeGeneratorGeneric(Dictionary<string, Type> properties)
        {
            Properties = properties;
            _setMethods = new Dictionary<string, MethodInfo>();
            Initialise();
        }

        private void Initialise()
        {
            var newTypeName = Guid.NewGuid().ToString();
            var assemblyName = new AssemblyName(newTypeName);
            var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var dynamicModule = dynamicAssembly.DefineDynamicModule("Main");
            var dynamicType = dynamicModule.DefineType(newTypeName,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    typeof(T));
            dynamicType.DefineDefaultConstructor(MethodAttributes.Public |
                                                MethodAttributes.SpecialName |
                                                MethodAttributes.RTSpecialName);

            foreach (var property in Properties)
                AddProperty(dynamicType, property.Key, property.Value);

            GeneratedType = dynamicType.CreateType();

            foreach (var property in Properties)
            {
                var propertyInfo = GeneratedType.GetProperty(property.Key);

                var setMethod = propertyInfo.GetSetMethod();
                if (setMethod == null) continue;
                _setMethods.Add(property.Key, setMethod);
            }
        }

        public Type GeneratedType { private set; get; }

        public Dictionary<string, Type> Properties { get; }

        private static void AddProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            
            var getMethod = typeBuilder.DefineMethod("get_" + propertyName,
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            var getMethodIL = getMethod.GetILGenerator();
            getMethodIL.Emit(OpCodes.Ldarg_0);
            getMethodIL.Emit(OpCodes.Ldfld, fieldBuilder);
            getMethodIL.Emit(OpCodes.Ret);

            var setMethod = typeBuilder.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });
            var setMethodIL = setMethod.GetILGenerator();
            Label modifyProperty = setMethodIL.DefineLabel();
            Label exitSet = setMethodIL.DefineLabel();

            setMethodIL.MarkLabel(modifyProperty);
            setMethodIL.Emit(OpCodes.Ldarg_0);
            setMethodIL.Emit(OpCodes.Ldarg_1);
            setMethodIL.Emit(OpCodes.Stfld, fieldBuilder);

            setMethodIL.Emit(OpCodes.Nop);
            setMethodIL.MarkLabel(exitSet);
            setMethodIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethod);
            propertyBuilder.SetSetMethod(setMethod);
        }

        /// <summary>
        /// Create a new instance of your generated type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public T CreateInstance(Dictionary<string, object> values = null)
        {
            var instance = (T)Activator.CreateInstance(GeneratedType);

            if (values != null)
                SetValues(instance, values);

            return instance;
        }

        /// <summary>
        /// Update the property values on an instance of your generated type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="values"></param>
        public void SetValues(T instance, Dictionary<string, object> values)
        {
            foreach (var value in values)
                SetValue(instance, value.Key, value.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void SetValue(T instance, string propertyName, object propertyValue)
        {
            if (!_setMethods.TryGetValue(propertyName, out var setter))
                throw new ArgumentException($"Type does not contain settter for property {propertyName}", nameof(propertyName));
            setter.Invoke(instance, new[] { propertyValue });
        }

        /// <summary>
        /// Create a new list of your new type and populate on initialisation
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public IList CreateList(T [] values = null)
        {
            var listGenericType = typeof(List<>);
            var list = listGenericType.MakeGenericType(GeneratedType);
            var constructor = list.GetConstructor(new Type[] { });
            var newList = (IList)constructor.Invoke(new object[] { });
            foreach (var value in values)
                newList.Add(value);
            return newList;
        }
    }
}
