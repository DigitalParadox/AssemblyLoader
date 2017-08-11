using System;
using System.Collections.Generic;
using System.Text;

namespace TestAssembly
{
    public class FailTestIfLoaded1
    {

    }

    public class FailTestIfLoaded2 : IComparable
    {
        public int CompareTo(object obj)
        {
            return 0;
        }
    }

    public class FailTestIfLoaded3 : EventArgs
    {

    }

    public abstract class FailTestIfLoaded4 : TestInterfaces.ITestInterface
    {
        public string Name { get; set; } = "Fail: Abstract classes should not be loaded";
    }
}
