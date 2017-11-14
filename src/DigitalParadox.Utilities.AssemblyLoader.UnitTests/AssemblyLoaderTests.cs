using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xunit;
using TestInterfaces;

using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Glob;


namespace DigitalParadox.Utilities.AssemblyLoader.UnitTests
{
    public class AssemblyLoaderTests
    {
        [Fact(DisplayName = "AssemblyLoader.GetAppDomainAssemblies<T>() Loads Expected Assemblies")]
        public void GetAppDomainAssembliesLoadsAssembliesFromAllSources()
        {
            AssemblyLoader.AllowInterfaces = false;
            AssemblyLoader.AllowAbstract = false;

            var assemblies = AssemblyLoader.GetAppDomainAssemblies<ITestInterface>().ToList();

            Assert.NotNull(assemblies);
            Assert.NotEmpty(assemblies);
            Assert.DoesNotContain(assemblies, q => q == null);
            Assert.Equal(2, assemblies.Count);
            Assert.DoesNotContain(assemblies, q=>q.FullName.Contains("System.Web"));
           
        }


        [Theory(DisplayName = "AssemblyLoader.IsValidType<T>() returns True when valid")]
        [InlineData(true, true, typeof(LocalTestAbstractClass))]
        [InlineData(false, false, typeof(LocalTestAbstractClass))]
        [InlineData(true, true, typeof(ITestInterface))]
        [InlineData(false, false, typeof(ITestInterface))]
        [InlineData(true, true, typeof(LocalTestClass))]
        [InlineData(true, false, typeof(LocalTestClass))]
        [InlineData(false, true, typeof(LocalTestClassShouldFail))]
        [InlineData(false, false, typeof(LocalTestClassShouldFail))]
        public void IsValidTypeReturnsTrueWhenValid(bool expected, bool loadNonConstructableTypes, Type testType)
        {
            if (loadNonConstructableTypes)
            {
                AssemblyLoader.AllowInterfaces = true;
                AssemblyLoader.AllowAbstract = true;
            }
            else
            {
                AssemblyLoader.AllowInterfaces = false;
                AssemblyLoader.AllowAbstract = false;
            }

            var isValid = AssemblyLoader.IsValidType<ITestInterface>(testType);

            Assert.Equal(isValid, expected);

        }

        [Theory(DisplayName = "AssemblyLoader.GetAssemblies<T>()loads expected assemblies from specified directory")]
        [InlineData(@".\TestAssembliesTreeStructure", 6)]
        //[InlineData(@".\TestAssemblies", 1)]
        public void GetAssembliesLoadsExpectedAssembliesFromDirectory(string path, int expectedCount)
        {
            var di = new DirectoryInfo(path);

            Assert.True(di.Exists);

            var assemblies = AssemblyLoader.GetAssemblies<ITestInterface>(di).ToList();

            Assert.NotNull(assemblies);
            Assert.NotEmpty(assemblies);
            Assert.DoesNotContain(assemblies, q=>q == null);
            Assert.Equal(expectedCount, assemblies.Count);
            Assert.DoesNotContain(assemblies, q => q.FullName.Contains("System.Web"));

        }

        [Theory(DisplayName = "AssemblyLoader.GetAssemblies<T>()loads expected assemblies from specified file")]
        [InlineData(true)]
        [InlineData(false)]
        public void GetAssembliesLoadsExpectedAssembliesFromFile(bool loadNonConstructableTypes)
        {
            var fi = new FileInfo(@".\TestAssemblies\TestAssembly.dll");

            Assert.True(fi.Exists);

            Assembly assembly;

            if (loadNonConstructableTypes)
            {
                AssemblyLoader.AllowInterfaces = true;
                AssemblyLoader.AllowAbstract = true;

                assembly = AssemblyLoader.GetAssembly<ITestInterface>(fi);
                var types = assembly.GetTypes<ITestInterface>().ToList();
          
                Assert.Contains(types, q => q.IsAbstract);
                Assert.Contains(types, q => q.Name.Contains("FailTestIfLoaded"));

            }
            else
            {
                AssemblyLoader.AllowInterfaces = false;
                AssemblyLoader.AllowAbstract = false;

                assembly =  AssemblyLoader.GetAssembly<ITestInterface>(fi);
                var types = assembly.GetTypes<ITestInterface>().ToList();
                Assert.DoesNotContain(types, q => q.IsInterface);
                Assert.DoesNotContain(types, q => q.IsAbstract);
                Assert.DoesNotContain(types, q=>q.Name.Contains("FailTestIfLoaded"));
            }

        }
        [Theory(DisplayName = "AssemblyLoader.FindDerivedTypes<T>() loads expected types ")]
        [InlineData(true)]
        [InlineData(false)]
        public void FindDerivedTypesReturnsExpectedTypes(bool loadNonConstructableTypes)
        {
            var fi = new FileInfo(@".\TestAssemblies\TestAssembly.dll");
            Assert.True(fi.Exists);
            if (loadNonConstructableTypes)
            {
                AssemblyLoader.AllowInterfaces = true;
                AssemblyLoader.AllowAbstract = true;
            }
            else
            {
                AssemblyLoader.AllowInterfaces = false;
                AssemblyLoader.AllowAbstract = false;
            }

            var assembly = Assembly.LoadFrom(fi.FullName);
            var types = AssemblyLoader.FindDerivedTypes<ITestInterface>(assembly).ToList();

            Assert.NotEmpty(types);

            if (loadNonConstructableTypes)
            {
                //Assert.Contains(types, q => q.IsInterface); //interface is in a separate assembly 
                Assert.Contains(types, q => q.IsAbstract);
            }
            else
            {
                Assert.DoesNotContain(types, q => q.IsInterface);
                Assert.DoesNotContain(types, q => q.IsAbstract);
            }

        }

        [Theory(DisplayName = "AssemblyLoader.GetTypes<T>() Returns Expected Types When Using FilePath")]
        [InlineData(true)]
        [InlineData(false)]
        public void GetTypesReturnsExpectedTypesFromFilePath(bool loadNonConstructableTypes)
        {
            var fi = new FileInfo(@".\TestAssemblies\TestAssembly.dll");
            Assert.True(fi.Exists);
            if (loadNonConstructableTypes)
            {
                AssemblyLoader.AllowInterfaces = true;
                AssemblyLoader.AllowAbstract = true;
            }
            else
            {
                AssemblyLoader.AllowInterfaces = false;
                AssemblyLoader.AllowAbstract = false;
            }
            
            var assembly = Assembly.LoadFrom(fi.FullName);
            Assert.NotNull(assembly);
            
            var types = AssemblyLoader.GetTypes<ITestInterface>(@".\TestAssemblies\TestAssembly.dll").ToList();

            Assert.NotEmpty(types);

            if (loadNonConstructableTypes)
            {
                Assert.Contains(types, q => q.Name.Contains("FailTestIfLoaded"));
            }
            else
            {
                Assert.DoesNotContain(types, q => q.Name.Contains("FailTestIfLoaded"));
            }

        }

        [Fact(DisplayName = "AssemblyLoader.GetTypes<T>() Returns Expected Types FromAssembly")]
        public void GetTypesReturnsExpectedTypesFromAssembly()
        {
            var fi = new FileInfo(@".\TestAssemblies\TestAssembly.dll");
            Assert.True(fi.Exists);

            AssemblyLoader.AllowInterfaces = false;
            AssemblyLoader.AllowAbstract = false;

            var assembly = Assembly.LoadFrom(fi.FullName);
            Assert.NotNull(assembly);

            var types = assembly.GetTypes<ITestInterface>().ToList();

            Assert.DoesNotContain(types, q => q.IsInterface);
            Assert.DoesNotContain(types, q => q.IsAbstract);
            Assert.DoesNotContain(types, q => q.Name.Contains("FailTestIfLoaded"));

            Assert.NotEmpty(types);
        }

        [Fact(DisplayName = "AssemblyLoader.GetTypes<T>() Returns Expected Types From Assembly Collection")]
        public void GetTypesReturnsExpectedTypesFromAssemblyCollection()
        {
            var files = new DirectoryInfo(@".\TestAssembliesTreeStructure\MultipleSubDirectory\SingleSubDirectory2\").GlobFiles(@"**\*.dll");
            
            AssemblyLoader.AllowInterfaces = false;
            AssemblyLoader.AllowAbstract = false;
            
            IEnumerable<Assembly> assemblies = files.Select(q=>Assembly.LoadFrom(q.FullName)).ToList();

            //Assert Test Code is expected 
            Assert.NotNull(assemblies);
            Assert.NotEmpty(assemblies);
            Assert.Equal(2, assemblies.Count());

            var types = assemblies.GetTypes<ITestInterface>().ToList();

            Assert.DoesNotContain(types, q => q.IsInterface);
            Assert.DoesNotContain(types, q => q.IsAbstract);
            Assert.DoesNotContain(types, q => q.Name.Contains("FailTestIfLoaded"));

            Assert.NotEmpty(types);

        }


        private abstract class LocalTestAbstractClassNotValid  {
            
        }

        private abstract class LocalTestAbstractClass : ITestInterface
        {
            public string Name { get; set; }
        }




        private class LocalTestClassShouldFail
        {

        }

        private class LocalTestClass : ITestInterface
        {
            public string Name { get; set; }
        }

        


    }
}
