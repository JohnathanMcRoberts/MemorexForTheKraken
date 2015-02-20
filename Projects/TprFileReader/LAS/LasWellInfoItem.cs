using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TprFileReader.LAS
{
    public class LasWellInfoItem
    {
        public string Mnemonic { get; set; }
        public string Unit { get; set; }
        public string DataValue { get; set; }
        public string Description { get; set; }

        public LasWellInfoItem(string str)
        {
            int separatorIndex = str.IndexOf(".");

            if (separatorIndex < 1) return;
            
            Mnemonic = str.Substring(0, separatorIndex).Trim();

            string remainder = str.Substring(separatorIndex + 1, str.Length - (separatorIndex + 1));
            int firstSpaceIndex = remainder.IndexOf(" ");

            // get the unit if provided
            Unit = "";
            if (remainder[0] != ' ')
            {
                Unit = remainder.Substring(0, firstSpaceIndex).Trim();
            }
            remainder = remainder.Substring(Unit.Length, remainder.Length - Unit.Length);

            int colonIndex = remainder.IndexOf(":");
            if (colonIndex > 0)
            {
                DataValue = remainder.Substring(0, colonIndex - 1).Trim();
                if (remainder.Length > colonIndex + 1)
                    Description = 
                        remainder.Substring(colonIndex+1, remainder.Length - (colonIndex +1)).Trim();
            }
            else
                DataValue = remainder.Trim();
        }
    }
}
