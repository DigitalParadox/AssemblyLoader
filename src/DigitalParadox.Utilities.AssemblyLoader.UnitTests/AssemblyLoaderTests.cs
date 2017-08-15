using System.Diagnostics;
using System.IO;
using Xunit;
using TestInterfaces;

using System.Linq;
using System.Reflection;


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
          
                Assert.Equal(true, types.Any(q => q.IsAbstract));

            }
            else
            {
                AssemblyLoader.AllowInterfaces = false;
                AssemblyLoader.AllowAbstract = false;

                assembly =  AssemblyLoader.GetAssembly<ITestInterface>(fi);
                var types = assembly.GetTypes<ITestInterface>().ToList();
                Assert.Equal(false, types.Any(q => q.IsInterface));
                Assert.Equal(false, types.Any(q => q.IsAbstract));

            }

            Assert.NotNull(assembly);
        }

    }
}
