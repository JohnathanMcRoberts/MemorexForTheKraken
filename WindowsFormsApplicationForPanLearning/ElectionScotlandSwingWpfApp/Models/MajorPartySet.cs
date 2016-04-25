using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectionScotlandSwingWpfApp.Models
{
    public class MajorPartySet
    {
        public List<Party> Parties { get; set; }

        public MajorPartySet()
        {
            Parties = new List<Party>()
            {
                new Party(){ Name = "Scottish National Party", Percentage = 0},
                new Party(){ Name = "Scottish Labour Party", Percentage = 0},
                new Party(){ Name = "Scottish Conservative Party", Percentage = 0},
                new Party(){ Name = "Scottish Liberal Democrats", Percentage = 0},
                new Party(){ Name = "Scottish Green Party", Percentage = 0},
                new Party(){ Name = "UKIP Scotland", Percentage = 0}
            };
        }

        public Party Victor 
        { 
            get 
            {
                Party victor = new Party(){Name= "None found", Percentage =0};

                foreach (var party in Parties)
                    if (party.Percentage > victor.Percentage)
                        victor = party;

                return victor;
            }
        }

        public MajorPartySet ApplySwing(MajorPartySet swing)
        {
            Dictionary<string, Party> swungParties = new Dictionary<string,Party>();
            foreach(var party in Parties)
                swungParties.Add(party.Name, party);
            foreach (var party in swing.Parties)
            {
                if (swungParties.ContainsKey(party.Name))
                    swungParties[party.Name].Percentage += party.Percentage;
                else
                    swungParties.Add(party.Name, party);
            }

            var swung = new MajorPartySet() { Parties = swungParties.Values.ToList() };
            return swung;
        }
    }
}
