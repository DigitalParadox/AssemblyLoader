using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;


namespace DigitalParadox.Utilities.AssemblyLoader
{
    public static class AssemblyLoader
    {
        /// <summary>
        ///     Search Assembly and return specified types the deririve from given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly">Assembly to search</param>
        /// <returns>Collection of derived types of <see cref="T" /></returns>
        public static IEnumerable<Type> FindDerivedTypes<T>(Assembly assembly)
        {
            //if (typeof(T).IsInterface)
            //{
            //    return assembly.GetTypes().Where(q => q.GetInterface(typeof(T).FullName) != null);
            //}
            return assembly.GetTypes<T>();
        }

        /// <summary>
        ///     Get collection of assembly that contain types derived from <see cref="T" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssemblies<T>()
        {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var filtered = assemblies.Where(q => FindDerivedTypes<T>(q).Any());
            return filtered;
        }

        public static Assembly GetAssembly<T>(FileInfo fi)
        {
            var assemblies = new List<Assembly> {Assembly.Load(fi.FullName)};
            var filtered = assemblies.FirstOrDefault(q => FindDerivedTypes<T>(q).Any());
            return filtered;
        }

        public static IEnumerable<Assembly> GetAssemblies<T>(DirectoryInfo di)
        {
            var fsis = di.GetFileSystemInfos();
            
            foreach (var fsi in fsis)
            {
                if (fsi is DirectoryInfo)
                {
                    Trace.Write($"Directory {fsi.Name} Found digging deeper..");

                    GetAssemblies<T>(fsi as DirectoryInfo);
                }
                yield return GetAssembly<T>(fsi as FileInfo);
            }
        }
        public static IEnumerable<Assembly> GetAssemblies<T>(string path)
        {

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var filtered = assemblies.Where(q => FindDerivedTypes<T>(q).Any());
            return filtered;
        }
        /// <summary>
        ///     Find All Derived Types of <see cref="T" />in a collection of assemblies
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypes<T>(this Assembly assembly)
        {
            return GetTypes<T>(new List<Assembly> {assembly});
        }

        public static IEnumerable<Type> GetTypes<T>(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(FindDerivedTypes<T>);
        }

        public static IEnumerable<Type> GetTypes<T>(string nameOrFile = null)
        {
            if (nameOrFile == null)
                return GetAssemblies<T>().GetTypes<T>();
            var assembly = Assembly.Load(nameOrFile);

            var types = FindDerivedTypes<T>(assembly);
            return types.Where(q => !q.IsAbstract && !q.IsInterface);
        }
    }





}