using Jareds.ServiceModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;

namespace Jareds.ServiceProxy
{
    public class DynamicProxy
    {

        private static ConcurrentDictionary<Type, Type> Cache = new ConcurrentDictionary<Type, Type>();
        private AssemblyBuilder _asmBuilder;
        private ModuleBuilder _moduleBuilder;
        private readonly string _asmName;
        private readonly string host;
        private DynamicProxy(string asmName, string host)
        {
            this.host = host;
            this._asmName = asmName;
            this._asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName { Name = asmName }, AssemblyBuilderAccess.Run);
        }
        private ModuleBuilder CreateModulerBuilder(string moduleName)
        {
            if (_moduleBuilder == null)
            {
                _moduleBuilder = _asmBuilder.DefineDynamicModule(moduleName);
            }
            return _moduleBuilder;
        }
        public Type CreateProxyType(Type typeOfT)
        {
            return Cache.GetOrAdd(typeOfT, t =>
            {
                var moduleBuilder = CreateModulerBuilder($"DynamicProxy.{typeOfT.Name}");
                var typeBuilder = moduleBuilder.DefineType($"{typeOfT.Name.TrimStart('I')}_{Guid.NewGuid():N}", TypeAttributes.Public | TypeAttributes.Class);
                typeBuilder.AddInterfaceImplementation(typeOfT);

                MethodInfo[] methods = typeOfT.GetMethods().Where(m => !m.IsSpecialName).ToArray();
                foreach (var m in methods)
                {
                    CreateMethod(typeBuilder, m);
                }
                PropertyInfo[] properties = typeOfT.GetProperties();
                foreach (var p in properties)
                {
                    CreateProperty(typeOfT, typeBuilder, p);
                }
                var dyType = typeBuilder.CreateTypeInfo();

                return dyType;
            });
        }
        public Type CreateProxyType<T>()
        {
            Type typeOfT = typeof(T);
            return CreateProxyType(typeOfT);
        }
        public T CreateProxyInstance<T>()
        {
            Type implType = CreateProxyType<T>();
            return (T)Activator.CreateInstance(implType);
        }

        private void CreateProperty(Type sourceType, TypeBuilder typeBuilder, PropertyInfo p)
        {
            string propertyName = p.Name;
            Type propType = p.PropertyType;
            var field = typeBuilder.DefineField("_" + propertyName, propType, FieldAttributes.Private);
            var property = typeBuilder.DefineProperty(propertyName,
                                           System.Reflection.PropertyAttributes.None,
                                           propType,
                                           new[] { propType });

            const MethodAttributes getSetAttr = MethodAttributes.Public
                                                | MethodAttributes.Virtual
                                                | MethodAttributes.HideBySig;

            var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName,
                                         getSetAttr,
                                         propType,
                                         Type.EmptyTypes);

            var currGetIl = currGetPropMthdBldr.GetILGenerator();
            currGetIl.Emit(OpCodes.Ldarg_0);
            currGetIl.Emit(OpCodes.Ldfld, field);
            currGetIl.Emit(OpCodes.Ret);

            var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                                         getSetAttr,
                                         null,
                                         new[] { propType });

            var currSetIl = currSetPropMthdBldr.GetILGenerator();
            currSetIl.Emit(OpCodes.Ldarg_0);
            currSetIl.Emit(OpCodes.Ldarg_1);
            currSetIl.Emit(OpCodes.Stfld, field);
            currSetIl.Emit(OpCodes.Ret);


            property.SetGetMethod(currGetPropMthdBldr);
            property.SetSetMethod(currSetPropMthdBldr);
            var getMethod = sourceType.GetMethod("get_" + propertyName);
            var setMethod = sourceType.GetMethod("set_" + propertyName);
            typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
            typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);
        }

        private void CreateMethod(TypeBuilder typeBuilder, MethodInfo method)
        {
            Type returnType = method.ReturnType;
            string methodName = method.Name;
            var methodParams = method.GetParameters();
            var parameterTypes = methodParams.Select(p => p.ParameterType).ToArray();
            var parameterNames = methodParams.Select(p => p.Name).ToArray();
            var genericType = returnType;
            if (returnType.IsInterface)
            {
                genericType = CreateProxyType(returnType);
            }
            else if (returnType.IsGenericType)
            {
                var types = returnType.GetGenericArguments();
                var impl = new Type[types.Length];
                for (int i = 0; i < impl.Length; i++)
                {
                    if (types[i].IsInterface)
                    {
                        impl[i] = CreateProxyType(types[i]);
                    }
                    else
                    {
                        impl[i] = types[i];
                    }
                }
                genericType = typeof(List<>).MakeGenericType(impl);
            }

            var proxyAttr = (method.GetCustomAttribute(typeof(HttpProxyAttribute)) as HttpProxyAttribute);
            if (proxyAttr == null)
            {
                throw new Exception("服务未标注HttpProxy");
            }
            string path = proxyAttr.Path;
            string httpMethod = proxyAttr.HttpMethod;
            var mtdBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes.Public | MethodAttributes.Virtual, returnType, parameterTypes);
            var il = mtdBuilder.GetILGenerator();

            var obj = il.DeclareLocal(typeof(object));
            if (parameterTypes == null || parameterTypes.Length <= 0)
            {
                il.Emit(OpCodes.Ldnull);
                il.Emit(OpCodes.Stloc, obj);
            }
            else if (parameterTypes.Length > 1 || (parameterTypes.Length == 1 && (!parameterTypes[0].IsClass || parameterTypes[0] != typeof(string))))
            {
                var dict = il.DeclareLocal(typeof(Dictionary<string, object>));
                il.Emit(OpCodes.Newobj, typeof(Dictionary<string, object>).GetConstructor(new Type[0]));
                il.Emit(OpCodes.Stloc, dict);
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    il.Emit(OpCodes.Ldloc, dict);
                    il.Emit(OpCodes.Ldstr, parameterNames[i]);
                    if (i == 0)
                        il.Emit(OpCodes.Ldarg_1);
                    else if (i == 1)
                        il.Emit(OpCodes.Ldarg_2);
                    else if (i == 2)
                        il.Emit(OpCodes.Ldarg_3);
                    else
                        il.Emit(OpCodes.Ldarg_S, i + 1);
                    if (parameterTypes[i].IsValueType)
                    {
                        il.Emit(OpCodes.Box, parameterTypes[i]);
                    }
                    il.Emit(OpCodes.Callvirt, typeof(Dictionary<string, object>).GetMethod("Add"));
                }
                il.Emit(OpCodes.Ldloc, dict);

                il.Emit(OpCodes.Stloc, obj);

            }
            else
            {
                il.Emit(OpCodes.Ldarg_1);

                il.Emit(OpCodes.Stloc, obj);

            }
            il.Emit(OpCodes.Nop);

            var apiRequest = il.DeclareLocal(typeof(RestServiceBaseAccessor));
            var ins = RestServiceBaseAccessor.Resolver();
            Type typeofInterceptor = ins.GetType();
            il.Emit(OpCodes.Newobj, typeofInterceptor.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc, apiRequest);
            il.Emit(OpCodes.Ldloc, apiRequest);

            il.Emit(OpCodes.Ldstr, path);//加载参数
            il.Emit(OpCodes.Ldstr, httpMethod);
            il.Emit(OpCodes.Ldloc, obj);
            il.Emit(OpCodes.Ldstr, this.host);
            il.Emit(OpCodes.Callvirt, typeofInterceptor.GetMethod(nameof(RestServiceBaseAccessor.Handle)).MakeGenericMethod(genericType));

            var result = il.DeclareLocal(returnType);
            il.Emit(OpCodes.Stloc, result);
            il.Emit(OpCodes.Ldloc, result);

            il.Emit(OpCodes.Ret);
        }
        private static ConcurrentDictionary<string, DynamicProxy> proxys = new ConcurrentDictionary<string, DynamicProxy>();
        public static T CreateInstance<T>(string host)
        {
            Type typeOfT = typeof(T);
            string hash = Md5(typeOfT.Namespace + host);
            var dp = proxys.GetOrAdd(hash, ns =>
            {
                return new DynamicProxy(ns, host);
            });
            return dp.CreateProxyInstance<T>();
        }

        private static string Md5(string source)
        {
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(source));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.AppendFormat("{0:x2}", buffer[i]);
            }
            return builder.ToString();
        }
    }
}
