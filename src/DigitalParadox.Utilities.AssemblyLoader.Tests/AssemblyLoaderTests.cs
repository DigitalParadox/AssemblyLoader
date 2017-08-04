using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace DigitalParadox.Utilities.AssemblyLoader.Tests
{
    public class AssemblyLoaderTests
    {
        [Fact]
        public void GetAssembliesLoadsExpectedAssembliesFromAllSources()
        {
            var assemblies = AssemblyLoader.GetAssemblies<ITestAssembly>() as List<Assembly>;

            Assert.NotNull(assemblies);
            Assert.NotEmpty(assemblies);
            Assert.Collection(assemblies);
            Assert.Equal(1, assemblies.Count);

        }
        [Theory]
        [InlineData(@".\TestDirectory")]
        //[InlineData(@".\TestAssembliesMultiDirectory")]
        public void GetAssembliesLoadsExpectedAssembliesFromDirectoryPath(string path)
        {
            var di = new DirectoryInfo(path);

            Assert.True(di.Exists);

            var assemblies = AssemblyLoader.GetAssemblies<ITestAssembly>(di) as List<Assembly>;

            Assert.NotNull(assemblies);
            Assert.NotEmpty(assemblies);
            Assert.Collection(assemblies);
            Assert.Equal(1, assemblies.Count);

        }
    }
}
