using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lithnet.Transforms.UnitTests
{
    public static class UnitTestControl
    {
        private static bool initialized = false;

        public static void Initialize()
        {
            if (!initialized)
            {
                Lithnet.MetadirectoryServices.Resolver.MmsAssemblyResolver.RegisterResolver();

                TransformGlobal.HostProcessSupportsLoopbackTransforms = true;
                TransformGlobal.HostProcessSupportsNativeDateTime = true;
                //Logger.LogPath = @"D:\MAData\Lithnet.Transforms\Lithnet.transforms.unittests.log";
               // Logger.LogLevel = LogLevel.Debug;
                initialized = true;
            }
        }

        public static T XmlSerializeRoundTrip<T>(object objectToSerialize)
        {
            string filename = Path.GetTempFileName();
            bool delete = true;

            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
                {
                    XmlWriterSettings writerSettings = new XmlWriterSettings();
                    writerSettings.Indent = true;
                    writerSettings.IndentChars = "  ";
                    writerSettings.NewLineChars = Environment.NewLine;

                    XmlWriter writer = XmlWriter.Create(stream, writerSettings);
                    writer.WriteStartDocument();
                    writer.WriteStartElement("acma-unit-tests");

                    writer.WriteStartElement("test-data");
                    writer.WriteAttributeString("id", "x" + Guid.NewGuid().ToString());

                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));

                    serializer.WriteObject(writer, objectToSerialize);

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                    stream.Position = 0;

                    XmlReaderSettings readerSettings = new XmlReaderSettings();
                    XmlReader reader = XmlReader.Create(stream, readerSettings);
                    reader.ReadStartElement();
                    reader.ReadStartElement();
                    T deserializedObject = (T)serializer.ReadObject(reader);
                    
                    reader.Close();
                    stream.Close();
                    stream.Dispose();

                    return deserializedObject;
                }
            }
            catch
            {
                delete = false;
                throw;
            }
            finally
            {
                if (delete && File.Exists(filename))
                {
                    System.Diagnostics.Debug.WriteLine(System.IO.File.ReadAllText(filename));
                    File.Delete(filename);
                }
            }
        }

        public static void PerformanceTest(Action action, int expectedRate, int cycles = 200000)
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

        static void readerSettings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            throw e.Exception;
        }
    }
}
