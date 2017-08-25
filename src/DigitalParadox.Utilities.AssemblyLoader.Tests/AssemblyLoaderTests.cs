using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TestInterfaces;
using Xunit;
using Xunit.Sdk;

namespace DigitalParadox.Utilities.AssemblyLoader.Tests
{

    public class AssemblyLoaderTests
    {
        //[Theory(DisplayName = "AppDomain Loaded Assemblies Resolve Assembly ")]
        //[InlineData(".\\TestAssemblies\\TestAssembly.dll")]
        //[InlineData]

        //public void GetAssembliesFromAppDomainLoadExpectedTypes(params string[] paths)
        //{
        //    var domain = AppDomain.CreateDomain($"TestDomain");
            
        //    var loader = new AssemblyLoader(domain);

        //    Assert.False(loader.AllowAbstract);
        //    Assert.False(loader.AllowInterfaces);

        //    foreach (var path in paths)
        //    {

        //        var fi = new FileInfo(path);
        //        Assert.True(fi.Exists);
        //        var bin = File.ReadAllBytes(fi.FullName);


        //        AppDomain.CurrentDomain.Load(bin);

        //    }
            
        //    var assemblies = loader.GetAppDomainAssemblies<ITestInterface>().ToList();

        //    Assert.NotNull(assemblies);
        //    Assert.NotEmpty(assemblies);
        //    Assert.DoesNotContain(assemblies, q => q == null);
        //    Assert.Equal(1 + paths.Length, assemblies.Count );

        //    AppDomain.Unload(domain);

        //}

        [Theory(DisplayName = "Load assemblies from specified directory")]
        [InlineData(@".\TestAssembliesTreeStructure", 6)]
        //[InlineData(@".\TestAssemblies", 1)]
        public void GetAssembliesLoadsExpectedAssembliesFromDirectory(string path, int expectedCount)
        {
            var di = new DirectoryInfo(path);

            Assert.True(di.Exists);
            var loader = new AssemblyLoader();
            var assemblies = loader.GetAssemblies<ITestInterface>(di).ToList();

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

            
            var loader = new AssemblyLoader();

            Assert.False(loader.AllowAbstract);
            Assert.False(loader.AllowInterfaces);

            Assert.True(fi.Exists);

            Assembly assembly;

            if (loadNonConstructableTypes)
            {
                loader.AllowInterfaces = true;
                loader.AllowAbstract = true;

                assembly = loader.GetAssembly<ITestInterface>(fi);
                var types = assembly.GetTypes<ITestInterface>().ToList();
          
                Assert.Equal(true, types.Any(q => q.IsAbstract));

            }
            else
            {
                loader.AllowInterfaces = false;
                loader.AllowAbstract = false;

                assembly =  loader.GetAssembly<ITestInterface>(fi);
                var types = assembly.GetTypes<ITestInterface>().ToList();
                Assert.Equal(false, types.Any(q => q.IsInterface));
                Assert.Equal(false, types.Any(q => q.IsAbstract));

            }

            Assert.NotNull(assembly);
            
        }
        

    }
}
