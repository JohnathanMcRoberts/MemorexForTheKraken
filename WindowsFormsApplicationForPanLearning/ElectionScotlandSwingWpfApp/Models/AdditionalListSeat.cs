using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

namespace ElectionScotlandSwingWpfApp.Models
{
    [XmlType("AdditionalListSeat")]
    public class AdditionalListSeat : ParliamentarySeat
    {
        #region ICloneable

        public override object Clone()
        {
            var cloned = new AdditionalListSeat()
            {
                Name = this.Name,
                TotalVotesCast = this.TotalVotesCast,
                TotalElectorate = this.TotalElectorate,
                Candidates = new List<ConstituencyCandidate>()
            };

            foreach (var candidate in Candidates)
                cloned.Candidates.Add((ConstituencyCandidate)candidate.Clone());

            return cloned;
        }

        #endregion
    }
}
