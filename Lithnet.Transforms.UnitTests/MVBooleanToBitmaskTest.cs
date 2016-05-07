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
    public class MVBooleanToBitmaskTransformTest
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
            MVBooleanToBitmaskTransform transformToSeralize = new MVBooleanToBitmaskTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.Flags.Add(new FlagValue() { Name = "AccountDisabled", Value  = 2});
            transformToSeralize.Flags.Add(new FlagValue() { Name = "DontExpirePassword", Value  = 65535});
            transformToSeralize.DefaultValue = 512;
            UniqueIDCache.ClearIdCache();

            MVBooleanToBitmaskTransform deserializedTransform = (MVBooleanToBitmaskTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.Flags[0].Name, deserializedTransform.Flags[0].Name);
            Assert.AreEqual(transformToSeralize.Flags[1].Name, deserializedTransform.Flags[1].Name);
            Assert.AreEqual(transformToSeralize.Flags[0].Value, deserializedTransform.Flags[0].Value);
            Assert.AreEqual(transformToSeralize.Flags[1].Value, deserializedTransform.Flags[1].Value);
            Assert.AreEqual(transformToSeralize.DefaultValue, deserializedTransform.DefaultValue);
        }
       
        [TestMethod()]
        public void BitmaskLoopbackInputTest()
        {
            MVBooleanToBitmaskTransform transform = new MVBooleanToBitmaskTransform();
            transform.Flags.Add(new FlagValue() { Value = 2 });

            this.ExecuteTestBitwiseTransformLoopbackInput(transform, 512, new List<object>() { true }, 514);
            this.ExecuteTestBitwiseTransformLoopbackInput(transform, 514, new List<object>() { false }, 512);
            this.ExecuteTestBitwiseTransformLoopbackInput(transform, 512, new List<object>() { false }, 512);
            this.ExecuteTestBitwiseTransformLoopbackInput(transform, 514, new List<object>() { true }, 514);
        }

        [TestMethod()]
        public void BitmaskLoopbackInputTestMV()
        {
            MVBooleanToBitmaskTransform transform = new MVBooleanToBitmaskTransform();
            transform.Flags.Add(new FlagValue() { Value = 2 });
            transform.Flags.Add(new FlagValue() { Value = 4 });

            this.ExecuteTestBitwiseTransformLoopbackInput(transform, 512, new List<object>() { true, true }, 518);
            this.ExecuteTestBitwiseTransformLoopbackInput(transform, 514, new List<object>() { false, true}, 516);
            this.ExecuteTestBitwiseTransformLoopbackInput(transform, 512, new List<object>() { false , false }, 512);
            this.ExecuteTestBitwiseTransformLoopbackInput(transform, 514, new List<object>() { true, false }, 514);
        }


        [TestMethod()]
        public void PerformanceTest()
        {
            MVBooleanToBitmaskTransform transform = new MVBooleanToBitmaskTransform();
            transform.Flags.Add(new FlagValue() { Value = 2 });
            transform.Flags.Add(new FlagValue() { Value = 4 });

            UnitTestControl.PerformanceTest(() =>
            {
                Assert.AreEqual(518L,
                    transform.TransformValuesWithLoopback(
                       new List<object>() { true, true },
                        new List<object>() { 512 }
                        ).First());
            }, 350000);
        }

        [TestMethod()]
        public void BitmaskLoopbackInputTestWithNullPrimaryInput()
        {
            MVBooleanToBitmaskTransform transform = new MVBooleanToBitmaskTransform();
            transform.Flags.Add(new FlagValue() { Value = 2 });

            transform.DefaultValue = 512;

            this.ExecuteTestBitwiseTransformLoopbackInput(transform, null, new List<object>() { true }, 514);
            this.ExecuteTestBitwiseTransformLoopbackInput(transform, null, new List<object>() { false }, 512);
            this.ExecuteTestBitwiseTransformLoopbackInput(transform, null, new List<object>() { false }, 512);
            this.ExecuteTestBitwiseTransformLoopbackInput(transform, null, new List<object>() { true }, 514);
        }

        private void ExecuteTestBitwiseTransformLoopbackInput(MVBooleanToBitmaskTransform transform, object targetValue, IList<object> inputValues, long expectedValue)
        {
            IList<object> result = transform.TransformValuesWithLoopback(inputValues, new List<object>() { targetValue });
            
            if (result.Count == 0)
            {
                Assert.Fail("Not results were returned");
            }
            else if (result.Count > 1)
            {
                Assert.Fail("Too many results were returned");
            }

            long outValue = (long)result.First();

            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
