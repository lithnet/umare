using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;
using System.Linq;
using System.Diagnostics;

namespace Lithnet.Transforms.UnitTests
{
    /// <summary>
    ///This is a test class for TrimStringTransformTest and is intended to contain all TrimStringTransformTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TrimStringTransformTest
    {
        private TestContext testContextInstance;
       
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        
        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            TrimStringTransform transformToSeralize = new TrimStringTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.TrimType = TrimType.Both;
            UniqueIDCache.ClearIdCache();

            TrimStringTransform deserializedTransform = (TrimStringTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.TrimType, deserializedTransform.TrimType);
        }

        [TestMethod()]
        public void PerformanceTest()
        {
            TrimStringTransform transform = new TrimStringTransform();
            transform.TrimType = TrimType.Left;

            int cycles = 200000;

            Stopwatch t = new Stopwatch();
            t.Start();

            for (int i = 0; i < cycles; i++)
            {
                Assert.AreEqual("1234", transform.TransformValue("    1234").FirstOrDefault());
            }

            t.Stop();
            int objSec = (int)(cycles / t.Elapsed.TotalSeconds);

            if (objSec < 500000)
            {
                Assert.Fail("Perf test failed: {0} obj/sec", objSec);
            }
        }

        [TestMethod()]
        public void TrimStringTransformTrimLeft()
        {
            TrimStringTransform transform = new TrimStringTransform();
            transform.TrimType = TrimType.Left;

            this.ExecuteTestTrimString(transform, " Ryan ", "Ryan ");
        }

        [TestMethod()]
        public void TrimStringTransformTrimRight()
        {
            TrimStringTransform transform = new TrimStringTransform();
            transform.TrimType = TrimType.Right;

            this.ExecuteTestTrimString(transform, " Ryan ", " Ryan");
        }

        [TestMethod()]
        public void TrimStringTransformTrimBoth()
        {
            TrimStringTransform transform = new TrimStringTransform();
            transform.TrimType = TrimType.Both;

            this.ExecuteTestTrimString(transform, " Ryan ", "Ryan");
        }

        [TestMethod()]
        public void TrimStringTransformTrimNone()
        {
            TrimStringTransform transform = new TrimStringTransform();
            transform.TrimType = TrimType.None;

            this.ExecuteTestTrimString(transform, " Ryan ", " Ryan ");
        }

        private void ExecuteTestTrimString(TrimStringTransform transform, string sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
