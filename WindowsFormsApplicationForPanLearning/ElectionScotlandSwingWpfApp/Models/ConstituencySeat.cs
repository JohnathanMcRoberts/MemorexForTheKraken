using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

using ElectionScotlandSwingWpfApp.Utilities;

namespace ElectionScotlandSwingWpfApp.Models
{

    [XmlType("ConstituencySeat")] // define Type
    public class ConstituencySeat : ParliamentarySeat
    {
        #region Public Properties

        public SerializableDictionary<string, int> PartyListVotes { get; set; }

        #endregion

        #region Constructor
        
        public ConstituencySeat()
        {
            PartyListVotes = new SerializableDictionary<string, int>();
        }
        
        #endregion


    }
}
