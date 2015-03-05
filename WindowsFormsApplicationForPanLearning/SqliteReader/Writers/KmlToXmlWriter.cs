using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace SqliteReader.Writers
{
    public class KmlToXmlWriter
    {
        public KmlToXmlWriter()
        {
            CreateNewFileForKmlDoc();

        }

        private void CreateNewFileForKmlDoc()
        {
            _currentKmlDocName = "C:\\Users\\E142890\\Downloads\\" + "Constuencies"+
                DateTime.Now.Ticks +".xml";

            using(var outputXmlFilestream = File.CreateText(_currentKmlDocName))
            {
                outputXmlFilestream.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                outputXmlFilestream.WriteLine("<Document>");
            }


        }

        private string _currentKmlDocName;

        public void AddKmlBlock(string kmlBlock)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(kmlBlock);
            XmlElement root = doc.DocumentElement;
            var kmlString = root.InnerXml;
            using (var outputXmlFilestream = new StreamWriter(_currentKmlDocName,true))
            {
                outputXmlFilestream.Write(kmlString);
            }
             
        }

        public void CompleteDocument()
        {
            using (var outputXmlFilestream = new StreamWriter(_currentKmlDocName,true))
            {
                outputXmlFilestream.Write("</Document>");
            }

        }

    }
}
