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
    ///This is a test class for RegexReplaceTransformTest and is intended
    ///to contain all RegexReplaceTransformTest Unit Tests
    ///</summary>
    [TestClass()]
    public class XPathQueryTransformTest
    {
        public XPathQueryTransformTest()
        {
            UnitTestControl.Initialize();
        }


        private string sampleCourse = @"
<course>
    <code>3050</code>
    <studyMode>MM</studyMode>
    <startDate>2014-03-03</startDate>
    <status>INACTIVE</status>
    <shortName>SSE (ENHANCEMENT ST)</shortName>
    <name>SINGLE SUBJECT ENROLMENT (ENHANCEMENT STUDIES)</name>
    <type>UG</type>
    <location>CLAYTON</location>
    <orgLocation>IDM-AU</orgLocation>
    <ouNumber>50000253</ouNumber>
    <ouName>Office of the DVC (Global Engagement)</ouName>
</course>";

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            XPathQueryTransform transformToSeralize = new XPathQueryTransform();
            transformToSeralize.DefaultValue = "A";
            transformToSeralize.XPathQuery = "Query";
            transformToSeralize.OnMissingMatch = OnMissingMatch.UseNull;
            transformToSeralize.ID = "test001";
            transformToSeralize.UserDefinedReturnType = ExtendedAttributeType.Boolean;
            UniqueIDCache.ClearIdCache();

            XPathQueryTransform deserializedTransform = (XPathQueryTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.DefaultValue, deserializedTransform.DefaultValue);
            Assert.AreEqual(transformToSeralize.XPathQuery, deserializedTransform.XPathQuery);
            Assert.AreEqual(transformToSeralize.OnMissingMatch, deserializedTransform.OnMissingMatch);
            Assert.AreEqual(transformToSeralize.UserDefinedReturnType, deserializedTransform.UserDefinedReturnType);
        }

        [TestMethod()]
        public void PerformanceTest()
        {
            XPathQueryTransform transform = new XPathQueryTransform();
            transform.XPathQuery = @"course/code/text()";
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;

            int cycles = 200000;

            Stopwatch t = new Stopwatch();
            t.Start();

            for (int i = 0; i < cycles; i++)
            {
                Assert.AreEqual("3050", transform.TransformValue(sampleCourse).FirstOrDefault());
            }

            t.Stop();
            int objSec = (int)(cycles / t.Elapsed.TotalSeconds);

            if (objSec < 25000)
            {
                Assert.Fail("Perf test failed: {0} obj/sec", objSec);
            }

        }
    }
}
