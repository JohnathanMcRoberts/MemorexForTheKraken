using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

using CsvHelper;

namespace ElectionScotlandSwingWpfApp.Models
{
    public class MainModel
    {
        #region Constructor

        public MainModel(log4net.ILog log)
        {
            // TODO: Complete member initialization
            this.Log = log;
        }

        #endregion

        #region Public Functions

        internal void OpenConstituenciesResults(string filename)
        {
            using (var sr = new StreamReader(filename, Encoding.Default))
            {
                var parser = new CsvParser(sr);
                bool setUpHeader = false;
                while (true)
                {
                    var row = parser.Read();
                    if (row == null)
                    {
                        break;
                    }
                    else
                    {
                        if (!setUpHeader)
                        {
                            setUpHeader = SetupPartiesFromConstituenciesHeader(row);

                            _regionToConstituencyLookup = new Dictionary<string, List<string>>();
                            _constituencyToNumCandidatesLookup = new Dictionary<string, int>();
                            _constituencyToPartyResultsLookup = new Dictionary<string, List<KeyValuePair<string, int>>>();
                            _constituencyToElectorateLookup = new Dictionary<string, int>();
                        }
                        else
                            SetupConstituencyResultsFromRow(row);
                    }
                }
            }
            BuildElectionResultFromConstituencies(filename);
            //UpdateCollections();
            //Properties.Settings.Default.InputFile = filename;
            //Properties.Settings.Default.Save();
        }

        internal void OpenRegionsResults(string filename)
        {
            using (var sr = new StreamReader(filename, Encoding.Default))
            {
                var parser = new CsvParser(sr);
                bool setUpHeader = false;
                while (true)
                {
                    var row = parser.Read();
                    if (row == null)
                    {
                        break;
                    }
                    else
                    {
                        if (!setUpHeader)
                        {
                            setUpHeader = SetupPartiesFromRegionsHeader(row);

                            _regionToConstituencyLookup = new Dictionary<string, List<string>>();
                            _constituencyToNumCandidatesLookup = new Dictionary<string, int>();
                            _constituencyToPartyResultsLookup = new Dictionary<string, List<KeyValuePair<string, int>>>();
                            _constituencyToElectorateLookup = new Dictionary<string, int>();
                        }
                        else
                            SetupConstituencyRegionResultsFromRow(row);
                    }
                }
            }
            UpdateElectionResultWithRegionalListVotes(filename);
        }

        internal void WriteElectionResultToFile(string filename)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Public Data

        public log4net.ILog Log { get; private set; }

        public ElectionResult CurrentResult { get; private set; }

        #endregion

        #region Constituency Parsing

        // data to column look ups
        private List<KeyValuePair<string, int>> _constituencyPartyToColumnLookup;
        private readonly int _constituencyRegionToColumnLookup = 0;
        private readonly int _constituencyNameToColumnLookup = 2;
        private readonly int _constituencyNumCandidatesToColumnLookup = 3;
        private int _constituencyElectorateToColumnLookup = 5;

        // region to constituency to result data
        private Dictionary<string, List<string>> _regionToConstituencyLookup;
        private Dictionary<string, int> _constituencyToNumCandidatesLookup;
        private Dictionary<string, List<KeyValuePair<string, int>>> _constituencyToPartyResultsLookup;
        private Dictionary<string, int> _constituencyToElectorateLookup;

        private bool SetupPartiesFromConstituenciesHeader(string[] columns)
        {
            // format is Region,Order,Constituency,Candidates,Labour,...,UKIP,Other,Electorate,...
            bool addingParties = false;
            _constituencyPartyToColumnLookup = new List<KeyValuePair<string, int>>(); ;

            for (int i = 0; i < columns.Length; i++ )
            {
                if (columns[i] == "Candidates")
                {
                    addingParties = true;
                    continue;
                }
                if (columns[i] == "Electorate")
                {
                    _constituencyElectorateToColumnLookup = i;
                    addingParties = false;
                }
                if (!addingParties)
                    continue;

                _constituencyPartyToColumnLookup.Add(new KeyValuePair<string, int>(columns[i], i));
            }

            return true;
        }

        private void SetupConstituencyResultsFromRow(string[] columns)
        {
            string region = "";
            string constituencyName = "";

            for (int i = 0; i < columns.Length; i++)
            {
                if (string.IsNullOrEmpty(columns[i])) continue;
                if (i == _constituencyRegionToColumnLookup)
                {
                    region = columns[i];
                    continue;
                }

                if (i == _constituencyNameToColumnLookup)
                {
                    constituencyName = columns[i];
                    if (_regionToConstituencyLookup.ContainsKey(region))
                    {
                        _regionToConstituencyLookup[region].Add(constituencyName);
                    }
                    else 
                    {
                        _regionToConstituencyLookup.Add(region, new List<string>() { constituencyName }); 
                    }
                    continue;
                }

                if (i == _constituencyNumCandidatesToColumnLookup)
                {
                    string numCandidates = columns[i];
                    if (_constituencyToNumCandidatesLookup.ContainsKey(constituencyName))
                    {
                        _constituencyToNumCandidatesLookup[constituencyName] = Int32.Parse(numCandidates);
                    }
                    else
                    {
                        _constituencyToNumCandidatesLookup.Add(constituencyName, Int32.Parse(numCandidates));
                    }
                    continue;
                }

                foreach (var partyLookup in _constituencyPartyToColumnLookup)
                {
                    if (partyLookup.Value == i)
                    {
                        int votes = GetElectorateAsNumber(columns[i]);
                        if (_constituencyToPartyResultsLookup.ContainsKey(constituencyName))
                        {
                            _constituencyToPartyResultsLookup[constituencyName].Add(
                                new KeyValuePair<string, int>(partyLookup.Key, votes)
                                );
                        }
                        else
                        {
                            _constituencyToPartyResultsLookup.Add(
                                constituencyName, new List<KeyValuePair<string, int>>() 
                                { new KeyValuePair<string, int>(partyLookup.Key, votes) });
                        }
                    }
                }


                if (i == _constituencyElectorateToColumnLookup)
                {
                    string electorate  = columns[i];
                    int electorateNumber = GetElectorateAsNumber(electorate);
                    if (_constituencyToElectorateLookup.ContainsKey(constituencyName))
                    {
                        _constituencyToElectorateLookup[constituencyName] = electorateNumber;
                    }
                    else
                    {
                        _constituencyToElectorateLookup.Add(constituencyName, electorateNumber);
                    }
                    continue;
                }
            }

        }

        private static int GetElectorateAsNumber(string electorate)
        {
            string electorateTrim = electorate.Trim();
            electorateTrim = electorateTrim.Replace(",", "");
            if (electorateTrim == "-")
                return 0;
            int electorateNumber = Int32.Parse(electorateTrim);
            return electorateNumber;
        }

        private void BuildElectionResultFromConstituencies(string filename)
        {
            CurrentResult = new ElectionResult() { Name = filename };

            foreach (var regionName in _regionToConstituencyLookup.Keys)
            {
                ElectoralRegion region = new ElectoralRegion() { Name = regionName };

                foreach (var constituencyName in _regionToConstituencyLookup[regionName])
                {
                    ConstituencySeat constituencySeat =
                        new ConstituencySeat() { Name = constituencyName };

                    if (_constituencyToElectorateLookup.ContainsKey(constituencyName))
                        constituencySeat.TotalElectorate =
                            _constituencyToElectorateLookup[constituencyName];


                    if (_constituencyToPartyResultsLookup.ContainsKey(constituencyName))
                    {
                        var partyTotals = _constituencyToPartyResultsLookup[constituencyName];

                        foreach (var party in partyTotals)
                        {
                            ConstituencyCandidate candidate =
                                new ConstituencyCandidate() { Name = party.Key, Party = party.Key, VotesFor = party.Value };

                            constituencySeat.Candidates.Add(candidate);
                        }
                    }

                    region.ConstituencySeats.Add(constituencySeat);
                }

                CurrentResult.Regions.Add(region);

            }
        }

        #endregion

        #region Region Parsing

        private bool SetupPartiesFromRegionsHeader(string[] columns)
        {
            // format is Region,Dummy1,Constituency,Next Parties,Labour,SNP,...,UKIP,Other Independent,Total Votes,...
            bool addingParties = false;
            _constituencyPartyToColumnLookup = new List<KeyValuePair<string, int>>(); ;

            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i] == "Next Parties")
                {
                    addingParties = true;
                    continue;
                }
                if (columns[i] == "Total Votes")
                {
                    addingParties = false;
                }
                if (!addingParties)
                    continue;

                _constituencyPartyToColumnLookup.Add(new KeyValuePair<string, int>(columns[i], i));
            }

            return true;
        }

        private void SetupConstituencyRegionResultsFromRow(string[] columns)
        {
            string region = "";
            string constituencyName = "";

            for (int i = 0; i < columns.Length; i++)
            {
                if (string.IsNullOrEmpty(columns[i])) continue;
                if (i == _constituencyRegionToColumnLookup)
                {
                    region = columns[i];
                    continue;
                }

                if (i == _constituencyNameToColumnLookup)
                {
                    constituencyName = columns[i];
                    if (_regionToConstituencyLookup.ContainsKey(region))
                    {
                        _regionToConstituencyLookup[region].Add(constituencyName);
                    }
                    else
                    {
                        _regionToConstituencyLookup.Add(region, new List<string>() { constituencyName });
                    }
                    continue;
                }

                foreach (var partyLookup in _constituencyPartyToColumnLookup)
                {
                    if (partyLookup.Value == i)
                    {
                        int votes = GetElectorateAsNumber(columns[i]);
                        if (_constituencyToPartyResultsLookup.ContainsKey(constituencyName))
                        {
                            _constituencyToPartyResultsLookup[constituencyName].Add(
                                new KeyValuePair<string, int>(partyLookup.Key, votes)
                                );
                        }
                        else
                        {
                            _constituencyToPartyResultsLookup.Add(
                                constituencyName, new List<KeyValuePair<string, int>>() 
                                { new KeyValuePair<string, int>(partyLookup.Key, votes) });
                        }
                    }
                }


            }

        }

        private void UpdateElectionResultWithRegionalListVotes(string fileName)
        {
            var regionFile = Path.GetFileName(fileName);

            CurrentResult.Name += ", " + regionFile;
            foreach (var region in CurrentResult.Regions)
            {
                foreach (var constituency in region.ConstituencySeats)
                {
                    if (_constituencyToPartyResultsLookup.ContainsKey(constituency.Name))
                    {
                        var partiesVotes = _constituencyToPartyResultsLookup[constituency.Name];
                        foreach (var item in partiesVotes)
                            constituency.PartyListVotes.Add(item.Key, item.Value);
                    }
                }

                region.SetupListSeats();
            }

        }

        #endregion

    }
}
