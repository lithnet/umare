using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Transforms.UnitTests
{
    /// <summary>
    ///This is a test class for TrimStringTransformTest and is intended to contain all TrimStringTransformTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConditionalStringFlowTransformTest
    {
        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            ConditionalStringFlowTransform transformToSeralize = new ConditionalStringFlowTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.ComparisonType = StringComparison.InvariantCulture;
            UniqueIDCache.ClearIdCache();

            ConditionalStringFlowTransform deserializedTransform = (ConditionalStringFlowTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.ComparisonType, deserializedTransform.ComparisonType);
        }

        [TestMethod()]
        public void SVLoopBackTestNullTarget()
        {
            ConditionalStringFlowTransform transform = new ConditionalStringFlowTransform();
            transform.ComparisonType = StringComparison.OrdinalIgnoreCase;

            this.ExecuteConditionalStringFlowTransform(transform, new List<object>(), new List<object>() { "Bob" }, new List<object>() { "Bob" });
        }

        [TestMethod()]
        public void SVLoopBackTestSVTargetMatch()
        {
            ConditionalStringFlowTransform transform = new ConditionalStringFlowTransform();
            transform.ComparisonType = StringComparison.OrdinalIgnoreCase;

            this.ExecuteConditionalStringFlowTransform(transform, new List<object>() { "bob" }, new List<object>() { "Bob" }, new List<object>() { "bob" });
        }

        [TestMethod()]
        public void SVLoopBackTestMVTargetNoMatch()
        {
            ConditionalStringFlowTransform transform = new ConditionalStringFlowTransform();
            transform.ComparisonType = StringComparison.OrdinalIgnoreCase;

            this.ExecuteConditionalStringFlowTransform(transform, new List<object>() { "jim", "bob" }, new List<object>() { "bill" }, new List<object>() { "bill" });
        }

        [TestMethod()]
        public void SVLoopBackTestMVTargetOneMatch()
        {
            ConditionalStringFlowTransform transform = new ConditionalStringFlowTransform();
            transform.ComparisonType = StringComparison.OrdinalIgnoreCase;

            this.ExecuteConditionalStringFlowTransform(transform, new List<object>() { "bob", "jim" }, new List<object>() { "Bob" }, new List<object>() { "bob" });
        }


        [TestMethod()]
        public void MVLoopBackTestMVTargetAllMatch()
        {
            ConditionalStringFlowTransform transform = new ConditionalStringFlowTransform();
            transform.ComparisonType = StringComparison.OrdinalIgnoreCase;

            this.ExecuteConditionalStringFlowTransform(transform,new List<object>() { "bob", "jim" },new List<object>() { "Bob", "Jim" }, new List<object>() { "bob", "jim" });
        }

        [TestMethod()]
        public void PerformanceTest()
        {
            ConditionalStringFlowTransform transform = new ConditionalStringFlowTransform();
            transform.ComparisonType = StringComparison.OrdinalIgnoreCase;

            UnitTestControl.PerformanceTest(() =>
            {
                CollectionAssert.AreEqual(new List<object>() { "bob", "jim" },
                    transform.TransformValuesWithLoopback(
                        new List<object>() { "Bob", "Jim" },
                        new List<object>() { "bob", "jim" }
                        ).ToArray());
            }, 150000);
        }

        [TestMethod()]
        public void MVLoopBackTestMVTargetOneMatch()
        {
            ConditionalStringFlowTransform transform = new ConditionalStringFlowTransform();
            transform.ComparisonType = StringComparison.OrdinalIgnoreCase;

            this.ExecuteConditionalStringFlowTransform(transform, new List<object>() { "bob", "jim" }, new List<object>() { "Bob" }, new List<object>() { "bob" });
        }


        [TestMethod()]
        public void MVLoopBackTestMVTargetNoMatch()
        {
            ConditionalStringFlowTransform transform = new ConditionalStringFlowTransform();
            transform.ComparisonType = StringComparison.OrdinalIgnoreCase;

            this.ExecuteConditionalStringFlowTransform(transform, new List<object>() { "bob" }, new List<object>() { "Jim" }, new List<object>() { "Jim" });
        }

        [TestMethod()]
        public void MVLoopBackTestNullTarget()
        {
            ConditionalStringFlowTransform transform = new ConditionalStringFlowTransform();
            transform.ComparisonType = StringComparison.OrdinalIgnoreCase;

            this.ExecuteConditionalStringFlowTransform(transform, new List<object>(), new List<object>() { "Bob", "Jim" }, new List<object>() { "Bob", "Jim" });
        }


        private void ExecuteConditionalStringFlowTransform(ConditionalStringFlowTransform transform, IList<object> targetValues, IList<object> inputValues, IList<object> expectedValues)
        {
            IList<object> result = transform.TransformValuesWithLoopback(inputValues, targetValues);
            
            if (result.Count == 0)
            {
                Assert.Fail("Not results were returned");
            }
           
            if (result.Count != expectedValues.Count)
            {
                Assert.Fail("Incorrect number of results returned");
            }

            CollectionAssert.AreEquivalent(expectedValues.Cast<string>().ToArray(), result.Cast<string>().ToArray());
        }
    }
}
