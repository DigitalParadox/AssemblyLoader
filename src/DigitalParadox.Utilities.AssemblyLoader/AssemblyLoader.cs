using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Glob;


namespace DigitalParadox.Utilities.AssemblyLoader
{
    public static class AssemblyLoader
    {
        //TODO: Implement Option for Interfaces to be allowed 
        public static bool AllowInterfaces { get; set; }
        //TODO: Implement Option for Abstract Classes to be allowed 
        public static bool AllowAbstract { get; set; }

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

            var assemblyTypes = assembly.GetTypes().ToList();

            var assignableTypes = assemblyTypes.Where(q => typeof(T).GetTypeInfo().IsAssignableFrom(q));

            if (!AllowInterfaces)
            {
                assignableTypes = assemblyTypes.Where(q => !q.GetTypeInfo().IsInterface);
            }

            if (!AllowAbstract)
            {
                assignableTypes = assemblyTypes.Where(q => !q.GetTypeInfo().IsAbstract);
            }
            
            return assignableTypes;
        }

        /// <summary>
        ///     Get collection of assembly that contain types derived from <see cref="T" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAppDomainAssemblies<T>()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var filtered = assemblies.Where(q => FindDerivedTypes<T>(q).Any());
            return filtered;
        }

        public static Assembly GetAssembly<T>(FileInfo fi)
        {
            var assemblies = new List<Assembly> {Assembly.LoadFrom(fi.FullName)};
            var filtered = assemblies.FirstOrDefault(q => FindDerivedTypes<T>(q).Any());
            return filtered;
        }

        public static IEnumerable<Assembly> GetAssemblies<T>(DirectoryInfo di, string globFilter = "*.dll")
        {
            var fsis = di.GlobFileSystemInfos(globFilter);
            
            foreach (var fsi in fsis)
            {
                yield return GetAssembly<T>(fsi as FileInfo);
            }
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

        public static IEnumerable<Type> GetTypes<T>(string filePath = null)
        {
            if (filePath == null)
                return GetAppDomainAssemblies<T>().GetTypes<T>();

            var assembly = GetAssembly<T>(new FileInfo(filePath));

            var types = FindDerivedTypes<T>(assembly);
            return types.Where(q => !q.IsAbstract && !q.IsInterface);
        }
    }





}