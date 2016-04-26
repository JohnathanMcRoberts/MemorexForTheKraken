﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

namespace ElectionScotlandSwingWpfApp.Models
{
    [XmlType("ConstituencyCandidate")]
    public class ConstituencyCandidate
    {
        #region Public Properties

        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public int VotesFor { get; set; }
        [XmlElement]
        public double PercentageVote { get; set; }
        [XmlElement]
        public string Party { get; set; }

        #endregion

        #region Constructor

        public ConstituencyCandidate()
        {
            VotesFor = 0;
            PercentageVote = 0.0;
            Name = Party = "Not set";
        }
        
        #endregion
    }
}
