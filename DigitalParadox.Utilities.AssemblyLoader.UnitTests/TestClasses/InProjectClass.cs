using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestInterfaces;

namespace DigitalParadox.Utilities.AssemblyLoader.UnitTests.TestClasses
{
    public class InProjectClass : ITestInterface
    {
        public string Name { get; set; }
    }
}
