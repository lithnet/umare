﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lithnet.Transforms.UnitTests
{
    /// <summary>
    ///This is a test class for StringSplitTransformTest and is intended to contain all StringSplitTransformTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StringSplitTransformTest
    {
        public StringSplitTransformTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            StringSplitTransform transformToSeralize = new StringSplitTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.SplitRegex = ",";
            UniqueIDCache.ClearIdCache();

            StringSplitTransform deserializedTransform = (StringSplitTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.SplitRegex, deserializedTransform.SplitRegex);
        }

        [TestMethod()]
        public void TestSimpleTransform1()
        {
            StringSplitTransform transform = new StringSplitTransform();
            transform.SplitRegex = ",";

            this.ExecuteTest(transform, new List<object>() { "1,2,3" }, new List<string>() { "1", "2", "3" });
        }


        [TestMethod()]
        public void PerformanceTest()
        {
            StringSplitTransform transform = new StringSplitTransform();
            transform.SplitRegex = ",";

            int cycles = 200000;

            Stopwatch t = new Stopwatch();
            t.Start();

            for (int i = 0; i < cycles; i++)
            {
                CollectionAssert.AreEqual(new string[] { "1", "2", "3" }, transform.TransformValue("1,2,3").ToArray());
            }

            t.Stop();
            int objSec = (int)(cycles / t.Elapsed.TotalSeconds);

            if (objSec < 150000)
            {
                Assert.Fail("Perf test failed: {0} obj/sec", objSec);
            }
        }

        [TestMethod()]
        public void TestSimpleTransform2()
        {
            StringSplitTransform transform = new StringSplitTransform();
            transform.SplitRegex = ",";

            this.ExecuteTest(transform, new List<object>() { "1,2,3", "4,5" }, new List<string>() { "1", "2", "3", "4", "5" });
        }

        [TestMethod()]
        public void TestSimpleTransform3()
        {
            StringSplitTransform transform = new StringSplitTransform();
            transform.SplitRegex = ",";

            this.ExecuteTest(transform, new List<object>() { null }, new List<string>());
        }

        [TestMethod()]
        public void TestSimpleTransform4()
        {
            StringSplitTransform transform = new StringSplitTransform();
            transform.SplitRegex = ",";

            this.ExecuteTest(transform, new List<object>() { "1" }, new List<string>() { "1" });
        }

        private void ExecuteTest(StringSplitTransform transform, IList<object> sourceValues, IList<string> expectedValues)
        {
            IList<object> outValues = transform.TransformValue(sourceValues);

            CollectionAssert.AreEqual(expectedValues.ToArray(), outValues.ToArray());
        }
    }
}
