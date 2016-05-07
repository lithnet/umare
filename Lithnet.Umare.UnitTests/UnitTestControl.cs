using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Lithnet.Umare.UnitTests
{
    public static class UnitTestControl
    {

        public static void PerformanceTest(System.Action action, int expectedRate, int cycles = 200000)
        {
            Stopwatch t = new Stopwatch();
            t.Start();

            for (int i = 0; i < cycles; i++)
            {
                action.Invoke();
            }

            t.Stop();

            int objSec = (int)(cycles / t.Elapsed.TotalSeconds);

            if (objSec < expectedRate)
            {
                Assert.Fail("Perf test failed: {0} obj/sec", objSec);
            }
        }
    }
}
