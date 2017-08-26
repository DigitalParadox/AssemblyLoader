using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using FasterReflection;

using Glob;


namespace DigitalParadox.Utilities.AssemblyLoader
{
    public interface IAssemblyLoaderOptions
    {
        //TODO: Implement Option for Interfaces to be allowed 
        bool AllowInterfaces { get; set; } 
        //TODO: Implement Option for Abstract Classes to be allowed 
        bool AllowAbstract { get; set; } 
    }

    public class AssemblyLoader
    {
        //TODO: Implement Option for Interfaces to be allowed 
        public bool AllowInterfaces { get; set; } = false;
        //TODO: Implement Option for Abstract Classes to be allowed 
        public bool AllowAbstract { get; set; } = false;

        ReflectionMetadataBuilder _metadataBuilder = new ReflectionMetadataBuilder();

        IEnumerable<AssemblyDefinition> _assemblyDefinitions = new List<AssemblyDefinition>();


        public AssemblyLoader AddAssembly(string path)
        {
            _metadataBuilder.AddAssembly(path);
            return this;
        }
        public AssemblyLoader AddAssembly(Assembly assembly)
        {
            _metadataBuilder.AddAssembly(assembly.Location);
            return this;
        }

        public AssemblyLoader AddDirectory(DirectoryInfo directory, bool recurse)
        {

            return this;
        }

        public AssemblyLoader GetDirectoryAssemblies(DirectoryInfo directory, bool recurse)
        {
            var pattern = recurse ? "**\\*.(exe|dll)" : "*.(exe|dll)";

            var files = directory.GlobFiles(pattern);
            
            foreach (var file in files)
            {
                _metadataBuilder.AddAssembly(file.FullName);
            }

            return this;
        }
        
        public AssemblyLoader AddAppDomainAssemblies(AppDomain appDomain)
        {
            foreach (Assembly assembly in appDomain.GetAssemblies())
            {
                _metadataBuilder.AddAssembly(assembly.Location);
            }
            return this;
        }
        
        /// <summary>
        ///     Search Assembly and return specified types the deririve from given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assembly">Assembly to search</param>
        /// <returns>Collection of derived types of <see cref="T" /></returns>
        /// 


        /// <summary>
        ///     Get collection of assembly that contain types derived from <see cref="T" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<Assembly> GetAppDomainAssemblies<T>()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var filtered = assemblies.Where(q => FindDerivedTypes<T>(q).Any()).ToList();
            return filtered;
        }

        public  Assembly GetAssembly<T>(FileInfo fi)
        {
            var builder = new ReflectionMetadataBuilder();
            builder.AddAssembly(fi.FullName);
            builder.AddReferenceOnlyAssemblyByType<T>();
            
            var result = builder.Build();

            var referenceType = result.TypeDefinitions.Find(q => q.FullName == typeof(T).FullName);
            var types = result.TypeDefinitions.Where(q => q.IsAssignableFrom(referenceType));
            
         
            var filtered = result.;
            return filtered;
        }

        public  IEnumerable<Assembly> GetAssemblies<T>(DirectoryInfo di, string globFilter = "*.dll")
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


        public  IEnumerable<Type> GetTypes<T>(string filePath = null)
        {
            if (filePath == null)
                return GetAppDomainAssemblies<T>().GetTypes<T>();

            var assembly = GetAssembly<T>(new FileInfo(filePath));

            var types = FindDerivedTypes<T>(assembly);
            return types.Where(q => !q.IsAbstract && !q.IsInterface).Select(q=>q.GetType());
        }

        private IEnumerable<TypeDefinition> FindDerivedTypes<T>(Assembly assembly)
        {
            throw new NotImplementedException();
        }
    }

    public static class AssemblyLoaderExtensions
    {
        public static IEnumerable<Type> GetTypes<T>(this Assembly assembly)
        {
            return GetTypes<T>(new List<Assembly> { assembly });
        }

        public static IEnumerable<Type> GetTypes<T>(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(new AssemblyLoader().FindDerivedTypes<T>);
        }
    }



}