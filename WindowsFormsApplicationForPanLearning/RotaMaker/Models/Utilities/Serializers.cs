using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace RotaMaker.Models.Utilities
{
    public static class Serializers
    {
        public static object DeserializeObject<T>(this string toDeserialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringReader textReader = new StringReader(toDeserialize);
            return xmlSerializer.Deserialize(textReader);
        }

        public static string SerializeObject<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }

        public static void SerializeToXml<T>(T obj, string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                var ser = new XmlSerializer(typeof(T));
                ser.Serialize(fileStream, obj);
            }
        }


        public static T DeserializeFromXML<T>(string path)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (FileStream fs = File.OpenRead(path))
            {
                return (T)xs.Deserialize(fs);
            }
        }

    }
}
