using System.IO;
using Xunit;
using TestInterfaces;

using System.Linq;


namespace DigitalParadox.Utilities.AssemblyLoader.UnitTests
{
    public class AssemblyLoaderTests
    {
        [Fact]
        public void GetAssembliesLoadsExpectedAssembliesFromAllSources()
        {
            var assemblies = AssemblyLoader.GetAppDomainAssemblies<ITestInterface>().ToList();

            Assert.NotNull(assemblies);
            Assert.NotEmpty(assemblies);
            Assert.DoesNotContain(assemblies, q => q == null);
            Assert.Equal(2, assemblies.Count);
           
        }
        [Theory(DisplayName = "Load assemblies from specified directory")]
        [InlineData(@".\TestAssembliesTreeStructure", 7)]
        [InlineData(@".\TestAssemblies", 1)]
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

        [Fact(DisplayName = "Load Assemblies from specified file")]
        public void GetAssembliesLoadsExpectedAssembliesFromFile()
        {
            var fi = new FileInfo(@".\TestAssemblies\TestAssembly.dll");

            Assert.True(fi.Exists);
            
            var assemblies = AssemblyLoader.GetAssembly<ITestInterface>(fi);
            var types = assemblies.GetTypes<ITestInterface>().ToList();
            Assert.False(types.Any(q=>q.IsInterface), "Types collection contains interfaces");
            Assert.False(types.Any(q => q.IsAbstract), "Types collection contains abstract types");
            Assert.NotNull(assemblies);

        }

    }
}
