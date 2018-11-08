using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QuickShare.Patch.Common
{
    public static class IocContainer
    {
        private static List<Assembly> _assemblies;

        public static List<T> Get<T>() where T : class
        {
            var types = FindTypes(x => x.IsClass && !x.IsAbstract &&
                                       (x.HasInterface<T>() || x.IsSubclassOf(typeof(T))) && x.HasDefaultConstructor());
            return types.Select(x => (T)Activator.CreateInstance(x)).ToList();
        }

        private static Type[] FindTypes(Func<Type, bool> filter)
        {
            var list = new List<Type>();
            foreach (var assembly in FideAssembly())
            {
                try
                {
                    var types = assembly.GetTypes();
                    if (filter != null)
                    {
                        list.AddRange(types.Where(filter));
                    }
                    else
                    {
                        list.AddRange(types);
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }
            }

            return list.ToArray();
        }
        private static List<Assembly> FideAssembly()
        {
            if (_assemblies != null)
            {
                return _assemblies;
            }
            var path = AppDomain.CurrentDomain.BaseDirectory;
            _assemblies = new List<Assembly>();
            if (Directory.Exists(path))
            {
                var dlls = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly).ToList();
                foreach (var dll in dlls)
                {
                    var fileName = Path.GetFileName(dll);
                    if (fileName != null)
                    {
                        try
                        {
                            _assemblies.Add(Assembly.LoadFile(dll));
                        }
                        catch (Exception)
                        {
                            //ignore 忽略非net程序
                        }
                    }
                }

                var exes = Directory.GetFiles(path, "*.exe", SearchOption.TopDirectoryOnly).ToList();
                foreach (var exe in exes)
                {
                    var fileName = Path.GetFileName(exe);
                    if (fileName != null)
                    {
                        try
                        {
                            _assemblies.Add(Assembly.LoadFile(exe));
                        }
                        catch (Exception)
                        {
                            //ignore 忽略非net程序
                        }
                    }
                }
            }

            return _assemblies;
        }
        /// <summary>
        /// 判断一个类型具备一个接口
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool HasInterface(this Type type, Type interfaceType)
        {
            if (type == null) return false;
            if (interfaceType == null) return false;
            if (!interfaceType.IsInterface) return false;

            var its = type.GetInterfaces();
            foreach (var it in its)
            {
                if (it == interfaceType)
                {
                    return true;
                }
                if (interfaceType.IsGenericTypeDefinition && it.IsGenericType && it.GetGenericTypeDefinition() == interfaceType)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasInterface<T>(this Type type)
        {
            return type.HasInterface(typeof(T));
        }


        public static bool HasDefaultConstructor(this Type type)
        {
            return null != type.GetConstructor(Type.EmptyTypes);
        }
    }
}