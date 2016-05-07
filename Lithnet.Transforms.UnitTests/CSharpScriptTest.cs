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
    public class CSharpScriptTransformTest
    {
        public CSharpScriptTransformTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod]
        public void TestSV()
        {
            CSharpScriptTransform transform = new CSharpScriptTransform();
            transform.ScriptText = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lithnet.Transforms;
using Microsoft.MetadirectoryServices;

public static class CSExtension
{
    public static IList<object> Transform(IList<object> obj)
    {
        return new List<object>() { obj.First() };        
    }
}";
            IList<object> results = transform.TransformValue(new List<object>() { "1", "2"});

            Array expected = new List<object>() { "1" }.ToArray();

            CollectionAssert.AreEqual(expected, results.ToArray());
        }

        [TestMethod()]
        public void PerformanceTest()
        {
            CSharpScriptTransform transform = new CSharpScriptTransform();
            transform.ScriptText = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lithnet.Transforms;
using Microsoft.MetadirectoryServices;

public static class CSExtension
{
    public static IList<object> Transform(IList<object> obj)
    {
        return new List<object>() { obj.First() };        
    }
}";

            UnitTestControl.PerformanceTest(() =>
            {
                Assert.AreEqual("1", transform.TransformValue("1").First());
            }, 130000);
        }
        [TestMethod]
        public void TestMV()
        {
            CSharpScriptTransform transform = new CSharpScriptTransform();
            transform.ScriptText = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lithnet.Transforms;
using Microsoft.MetadirectoryServices;

public static class CSExtension
{
    public static IList<object> Transform(IList<object> obj)
    {
        return obj;       
    }
}";
            IList<object> results = transform.TransformValue(new List<object>() { "1", "2" });

            Array expected = new List<object>() { "1", "2" }.ToArray();

            CollectionAssert.AreEqual(expected, results.ToArray());
        }

        [TestMethod]
        public void TestDeclineMapping()
        {
            CSharpScriptTransform transform = new CSharpScriptTransform();
            transform.ScriptText = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Lithnet.Transforms;
using Microsoft.MetadirectoryServices;

public static class CSExtension
{
    public static IList<object> Transform(IList<object> obj)
    {
        throw new DeclineMappingException();    
    }
}";
            try
            {
                IList<object> results = transform.TransformValue(new List<object>() { "1", "2" });
                Assert.Fail("The expected exception was not thrown");
            }
            catch (DeclineMappingException)
            {
            }
        }

    }
}
