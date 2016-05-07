using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace Lithnet.Transforms.UnitTests
{
    [TestClass()]
    public class PowerShellScriptTransformTest
    {
        public PowerShellScriptTransformTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestStringSV()
        {
            PowerShellScriptTransform transform = new PowerShellScriptTransform();
            transform.ScriptPath = @"..\..\TestData\Transform-String.ps1";

            this.ExecuteTest(transform, "Things & stuff", "THINGS & STUFF");
        }

        [TestMethod()]
        public void TestStringSVBulk()
        {
            PowerShellScriptTransform transform = new PowerShellScriptTransform();
            transform.ScriptPath = @"..\..\TestData\Transform-String.ps1";

            for (int i = 0; i < 1000; i++)
            {
                this.ExecuteTest(transform, "Things & stuff", "THINGS & STUFF");
            }
        }


        [TestMethod()]
        public void PerformanceTest()
        {
            PowerShellScriptTransform transform = new PowerShellScriptTransform();
            transform.ScriptPath = @"..\..\TestData\Transform-String.ps1";

            UnitTestControl.PerformanceTest(() =>
            {
                Assert.AreEqual("THINGS & STUFF", transform.TransformValue("Things & stuff").First());
            }, 100000, 30000);
        }

        [TestMethod()]
        public void TestStringMV()
        {
            PowerShellScriptTransform transform = new PowerShellScriptTransform();
            transform.ScriptPath = @"..\..\TestData\Transform-String.ps1";

            this.ExecuteTest(transform, new List<object>() { "Things & stuff", "test", "test2" }, new List<object>() { "THINGS & STUFF", "TEST", "TEST2" });
        }

        [TestMethod()]
        public void TestIntegerSV()
        {
            PowerShellScriptTransform transform = new PowerShellScriptTransform();
            transform.ScriptPath = @"..\..\TestData\Transform-Integer.ps1";

            this.ExecuteTest(transform, 1, 2);
        }

        [TestMethod()]
        public void TestIntegerMV()
        {
            PowerShellScriptTransform transform = new PowerShellScriptTransform();
            transform.ScriptPath = @"..\..\TestData\Transform-Integer.ps1";

            this.ExecuteTest(transform, new List<object>() { 1, 3, 5 }, new List<object>() { 2, 4, 6 });
        }

        [TestMethod()]
        public void TestBooleanSV()
        {
            PowerShellScriptTransform transform = new PowerShellScriptTransform();
            transform.ScriptPath = @"..\..\TestData\Transform-Boolean.ps1";

            this.ExecuteTest(transform, true, false);
        }

        [TestMethod()]
        public void TestBooleanMV()
        {
            PowerShellScriptTransform transform = new PowerShellScriptTransform();
            transform.ScriptPath = @"..\..\TestData\Transform-Boolean.ps1";

            this.ExecuteTest(transform, new List<object>() { true, false, true }, new List<object>() { false, true, false });
        }

        [TestMethod()]
        public void TestBinarySV()
        {
            PowerShellScriptTransform transform = new PowerShellScriptTransform();
            transform.ScriptPath = @"..\..\TestData\Transform-Binary.ps1";

            this.ExecuteTest(transform, new byte[] { 0, 1, 2, 3 }, new byte[] { 3, 2, 1, 0 });
        }

        [TestMethod()]
        public void TestBinaryMV()
        {
            PowerShellScriptTransform transform = new PowerShellScriptTransform();
            transform.ScriptPath = @"..\..\TestData\Transform-Binary.ps1";

            this.ExecuteTest(transform, new List<object>() { new byte[] { 0, 1, 2, 3 }, new byte[] { 4, 5, 6, 7 } }, new List<object>() { new byte[] { 3, 2, 1, 0 }, new byte[] { 7, 6, 5, 4 } });
        }

        private void ExecuteTest(PowerShellScriptTransform transform, object sourceValue, object expectedValue)
        {
            object outValue = transform.TransformValue(sourceValue).FirstOrDefault();

            if (sourceValue is byte[])
            {
                CollectionAssert.AreEqual((byte[])expectedValue, (byte[])outValue);
            }
            else
            {
                Assert.AreEqual(expectedValue, outValue);
            }
        }

        private void ExecuteTest(PowerShellScriptTransform transform, IList<object> sourceValue, IList<object> expectedValue)
        {
            IList<object> outValue = transform.TransformValue(sourceValue);

            if (sourceValue.First() is byte[])
            {
                CollectionAssert.AreEqual(expectedValue.ToArray(), outValue.ToArray(), new ByteArrayComparer());
            }
            else
            {
                CollectionAssert.AreEqual(expectedValue.ToArray(), outValue.ToArray());
            }
        }
    }
}
