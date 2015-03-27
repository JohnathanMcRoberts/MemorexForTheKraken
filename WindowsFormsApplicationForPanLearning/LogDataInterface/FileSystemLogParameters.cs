using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LogDataInterface
{
    [Serializable]
    public class FileSystemLogParameters
    {
        private readonly static XmlSerializer Serializer = new XmlSerializer(typeof(FileSystemLogParameters));

        public FileSystemLogParameters()
        { }

        public FileSystemLogParameters(FileSystemDataLog log)
        {
            WellName = string.IsNullOrEmpty(log.WellName) ? "NoName" : log.WellName;
            WellId = log.WellId;
            WellboreName = string.IsNullOrEmpty(log.WellboreName) ? "NoName" : log.WellboreName;
            WellboreId = log.WellboreId;
            LogName = string.IsNullOrEmpty(log.Name) ? "NoName" : log.Name;
            LogId = log.Id;
            IndexCurve = log.IndexCurve;
            IndexType = log.IndexType;

            NullValue = string.IsNullOrEmpty(log.NullValue) ? "-999.25" : log.NullValue;
        }

        public string WellName { get; set; }
        public string WellId { get; set; }
        public string WellboreName { get; set; }
        public string WellboreId { get; set; }
        public string LogName { get; set; }
        public string LogId { get; set; }

        public IndexCurve IndexCurve { get; set; }
        public string IndexType { get; set; }
        public string NullValue { get; set; }

        public static XDocument ToXml(FileSystemLogParameters parameters)
        {
            XDocument document = new XDocument();
            using (XmlWriter writer = document.CreateWriter())
            {
                Serializer.Serialize(writer, parameters);
            }

            return document;
        }

        public static FileSystemLogParameters FromXml(XDocument paramsXml)
        {
            using (XmlReader reader = paramsXml.CreateReader())
            {
                return (FileSystemLogParameters)Serializer.Deserialize(reader);
            }
        }
    }
}
