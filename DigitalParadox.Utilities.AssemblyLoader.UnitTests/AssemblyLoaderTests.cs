using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;
using TestInterfaces;

namespace DigitalParadox.Utilities.AssemblyLoader.Tests
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
        [Theory]
        [InlineData(@".\TestAssemblies")]
        //[InlineData(@".\TestAssembliesMultiDirectory")]
        public void GetAssembliesLoadsExpectedAssembliesFromDirectoryPath(string path)
        {
            var di = new DirectoryInfo(path);

            Assert.True(di.Exists);

            var assemblies = AssemblyLoader.GetAssemblies<ITestInterface>(di).ToList();

            Assert.NotNull(assemblies);
            Assert.NotEmpty(assemblies);
            Assert.DoesNotContain(assemblies, q=>q == null);
            Assert.Equal(1, assemblies.Count);

        }


    }
}
