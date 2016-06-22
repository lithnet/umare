using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Runtime.Serialization;
using Lithnet.Common.ObjectModel;
using Lithnet.Transforms;
using Lithnet.MetadirectoryServices;

namespace Lithnet.Umare
{
    public static class ConfigManager
    {
        public static XmlConfigFile LoadXml(string filename)
        {
            UniqueIDCache.ClearIdCache();

            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XmlDictionaryReader xdr = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());

                DataContractSerializer serializer = new DataContractSerializer(typeof(XmlConfigFile));
                XmlConfigFile configFile = (XmlConfigFile)serializer.ReadObject(xdr);

                configFile.FileName = filename;
                configFile.ResetChangeState();

                ConfigManager.CurrentConfig = configFile;
                return configFile;
            }
        }

        public static XmlConfigFile CurrentConfig { get; private set; }

        public static void Save(string filename, XmlConfigFile configFile)
        {
            if (configFile == null)
            {
                throw new ArgumentNullException(nameof(configFile));
            }

            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite))
            {
                XmlWriterSettings writerSettings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = Environment.NewLine,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates
                };

                using (XmlWriter writer = XmlWriter.Create(stream, writerSettings))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(XmlConfigFile));
                    serializer.WriteObject(writer, configFile);

                    writer.Flush();
                    writer.Close();
                }
            }

            configFile.FileName = filename;
            configFile.ResetChangeState();
        }
    }
}
