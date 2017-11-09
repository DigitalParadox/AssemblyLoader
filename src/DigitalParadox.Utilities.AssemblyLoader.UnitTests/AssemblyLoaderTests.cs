using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xunit;
using TestInterfaces;

using System.Linq;
using System.Reflection;
using Glob;


namespace DigitalParadox.Utilities.AssemblyLoader.UnitTests
{
    public class AssemblyLoaderTests
    {
        [Fact]
        public void GetAssembliesLoadsExpectedAssembliesFromAllSources()
        {
            AssemblyLoader.AllowInterfaces = false;
            AssemblyLoader.AllowAbstract = false;

            var assemblies = AssemblyLoader.GetAppDomainAssemblies<ITestInterface>().ToList();

            Assert.NotNull(assemblies);
            Assert.NotEmpty(assemblies);
            Assert.DoesNotContain(assemblies, q => q == null);
            Assert.Equal(10, assemblies.Count);
           
        }

        [Theory(DisplayName = "Load assemblies from specified directory")]
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

        }

        [Theory(DisplayName = "Load Assemblies from specified file")]
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

            }
            else
            {
                AssemblyLoader.AllowInterfaces = false;
                AssemblyLoader.AllowAbstract = false;

                assembly =  AssemblyLoader.GetAssembly<ITestInterface>(fi);
                var types = assembly.GetTypes<ITestInterface>().ToList();
                Assert.DoesNotContain(types, q => q.IsInterface);
                Assert.DoesNotContain(types, q => q.IsAbstract);
            }

            Assert.NotNull(assembly);
        }
        [Fact]
        public void FindDerivedTypesReturnsExpectedTypes()
        {
            var fi = new FileInfo(".\\TestAssemblies\\TestAssembly.dll");
            Assert.True(fi.Exists);
            AssemblyLoader.AllowInterfaces = false;
            AssemblyLoader.AllowAbstract = false;
            var assembly = Assembly.LoadFrom(fi.FullName);
            var types = AssemblyLoader.FindDerivedTypes<ITestInterface>(assembly).ToList();

            Assert.DoesNotContain(types, q=>q.IsInterface);
            Assert.DoesNotContain(types, q => q.IsAbstract);
            Assert.DoesNotContain(types, q=>q.Name.Contains("FailTestIfLoaded"));

            Assert.NotEmpty(types);


        }

        [Fact]
        public void GetTypesReturnsExpectedTypesFromFilePath()
        {
            var fi = new FileInfo(".\\TestAssemblies\\TestAssembly.dll");
            Assert.True(fi.Exists);

            AssemblyLoader.AllowInterfaces = false;
            AssemblyLoader.AllowAbstract = false;

            var assembly = Assembly.LoadFrom(fi.FullName);
            Assert.NotNull(assembly);

            var types = AssemblyLoader.GetTypes<ITestInterface>(".\\TestAssemblies\\TestAssembly.dll").ToList();

            Assert.DoesNotContain(types, q => q.IsInterface);
            Assert.DoesNotContain(types, q => q.IsAbstract);
            Assert.DoesNotContain(types, q => q.Name.Contains("FailTestIfLoaded"));

            Assert.NotEmpty(types);

        }
        [Fact]
        public void GetTypesReturnsExpectedTypesFromAssembly()
        {
            var fi = new FileInfo(".\\TestAssemblies\\TestAssembly.dll");
            Assert.True(fi.Exists);

            AssemblyLoader.AllowInterfaces = false;
            AssemblyLoader.AllowAbstract = false;

            var assembly = Assembly.LoadFrom(fi.FullName);
            Assert.NotNull(assembly);

            var types = AssemblyLoader.GetTypes<ITestInterface>(assembly).ToList();

            Assert.DoesNotContain(types, q => q.IsInterface);
            Assert.DoesNotContain(types, q => q.IsAbstract);
            Assert.DoesNotContain(types, q => q.Name.Contains("FailTestIfLoaded"));

            Assert.NotEmpty(types);

        }

        [Fact]
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

            var types = AssemblyLoader.GetTypes<ITestInterface>(assemblies).ToList();

            Assert.DoesNotContain(types, q => q.IsInterface);
            Assert.DoesNotContain(types, q => q.IsAbstract);
            Assert.DoesNotContain(types, q => q.Name.Contains("FailTestIfLoaded"));

            Assert.NotEmpty(types);

        }

    }
}
