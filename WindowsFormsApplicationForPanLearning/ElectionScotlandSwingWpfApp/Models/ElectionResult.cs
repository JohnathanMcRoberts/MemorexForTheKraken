using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

namespace ElectionScotlandSwingWpfApp.Models
{
    [XmlType("ElectionResult")] // define Type
    [XmlInclude(typeof(ElectoralRegion))] // include type class AdditionalListSeat
    public class ElectionResult
    {
        #region Public Properties
        [XmlElement]
        public string Name { get; set; }

        [XmlArray("Regions")]
        [XmlArrayItem("ElectoralRegion")]
        public List<ElectoralRegion> Regions { get; set; }

        #endregion
        
        #region Constructor

        public ElectionResult()
        {
            Regions = new List<ElectoralRegion>();
        }

        #endregion

        #region Public Methods

        public string SerializeObject()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, this);
                return textWriter.ToString();
            }
        }

        #endregion
    }
}
