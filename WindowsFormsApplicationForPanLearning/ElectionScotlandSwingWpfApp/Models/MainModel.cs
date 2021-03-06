﻿using System;
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
            this.Log = log;

            PopulatePartyForecasts();

            CurrentResult = new ElectionResult();
            _predictedResult = new ElectionResult();

            _partyListSwings = new Dictionary<string,double>();
            _partyConstituencySwings = new Dictionary<string, double>();

            foreach (var party in MajorPartyNames)
            {
                _partyListSwings.Add(party, 0);
                _partyConstituencySwings.Add(party, 0);
            }

            FirstPartyVotesAndPairedPredictionTotals = 
                new List<KeyValuePair<double, List<PartyForecast>>>();
        }

        #endregion

        #region Public Functions

        public void OpenConstituenciesResults(string filename)
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

        public void OpenRegionsResults(string filename)
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
            PopulatePartyForecasts();
            ResetPredictedResultToCurrent();
        }

        public void WriteElectionResultToFile(string filename)
        {
            var asXML = CurrentResult.SerializeObject();

            StreamWriter sw = new StreamWriter(filename, false, Encoding.Default); //overwrite original file

            // write the XML
            sw.WriteLine(asXML);

            // tidy up
            sw.Close();
        }

        public void OpenElectoralResults(string filePath)
        {
            try
            {
                string xml;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    xml = reader.ReadToEnd();
                }

                var textReader = new StringReader(xml);
                var serializer = new XmlSerializer(typeof(ElectionResult));

                CurrentResult =
                    (ElectionResult)serializer.Deserialize(textReader);
            }
            catch (Exception e)
            {
                Log.Error("error reading xml file " + filePath);
                Log.Error(e.ToString());
                throw e;
            }

            PopulatePartyForecasts();

            ResetPredictedResultToCurrent();
        }

        public void PredictUsingUniformNationalSwing(
            Dictionary<string, double> partyListSwings,
            Dictionary<string, double> partyConstituencySwings)
        {
            _partyListSwings = new Dictionary<string, double> ( partyListSwings);
            _partyConstituencySwings = new Dictionary<string, double>(partyConstituencySwings);

            // reset the prediction to the previous
            ResetPredictedResultToCurrent();

            // update the swings to account for the removed parties
            Dictionary<string, double> updatedListSwings;
            Dictionary<string, double> updatedConstituencySwings;
            UpdateSwingsForRemovedParties(
                partyListSwings, partyConstituencySwings, 
                out updatedListSwings, out updatedConstituencySwings);

            // apply the swings & recalculate the regions
            foreach (var region in _predictedResult.Regions)
            {
                region.ApplyPartySwings(updatedListSwings, updatedConstituencySwings);
            }
        }

        public static void PopulatePartyForecasts(
            Dictionary<string, int> currentResultSeats,
            Dictionary<string, int> predictedResultSeats,
            ref List<PartyForecast> partyForecasts)
        {

            foreach (var party in partyForecasts)
            {
                party.PreviousSeats =
                    currentResultSeats.ContainsKey(party.Name) ?
                    currentResultSeats[party.Name] : 0;
                party.PredictedSeats =
                    predictedResultSeats.ContainsKey(party.Name) ?
                    predictedResultSeats[party.Name] : 0;
            }
        }

        #endregion

        #region Private Fields

        private ElectionResult _predictedResult;

        private Dictionary<string, double> _partyListSwings;
        private Dictionary<string, double> _partyConstituencySwings;

        #endregion

        #region Public Properties

        public log4net.ILog Log { get; private set; }

        public ElectionResult CurrentResult { get; private set; }

        public ElectionResult PredictedResult { get { return _predictedResult; } }


        public List<PartyForecast> PartyConstituencyForecasts { get; private set; }

        public List<PartyForecast> PartyListForecasts { get; private set; }

        public List<PartyForecast> PartyOverallForecasts 
        { 
            get
            {
                var listForecasts = PartyListForecasts;
                var constituencyForecasts = PartyConstituencyForecasts;
                List<PartyForecast> overallForecasts = 
                    GetOverallPartyForecastFromListAndConstituencies(listForecasts, constituencyForecasts);
                return overallForecasts;
            }
        }

        private static List<PartyForecast> GetOverallPartyForecastFromListAndConstituencies(
                        List<PartyForecast> listForecasts, List<PartyForecast> constituencyForecasts)
        {

            List<PartyForecast> overallForecasts = new List<PartyForecast>();
            if (listForecasts != null && constituencyForecasts != null)
            {
                for (int i = 0; i < listForecasts.Count; i++)
                {
                    string name = listForecasts[i].Name;
                    int prevSeats =
                        listForecasts[i].PreviousSeats + constituencyForecasts[i].PreviousSeats;
                    int predictedSeats =
                        listForecasts[i].PredictedSeats + constituencyForecasts[i].PredictedSeats;

                    overallForecasts.Add(
                        new PartyForecast(name, 0.0)
                        {
                            PreviousSeats = prevSeats,
                            PredictedSeats = predictedSeats
                        });
                }
            }
            return overallForecasts;
        }

        #endregion

        #region Constant Data

        public enum MajorNamePartyLookup
        {
            SNP = 0,
            Labour = 1,
            Conservative=2,
            LiberalDemocrat=3,
            Green=4,
            UKIP=5
        }

        public readonly static string[] MajorPartyNames = new string[] 
        {        
            "SNP", 
            "Labour", 
            "Conservative", 
            "Liberal Democrat", 
            "Green", 
            "UK Independence Party"
        };

        public readonly static string[] RemovedPartyNames = new string[] 
        {        
            "Margo MacDonald", 
            "British National Party"
        };

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

            ResetPredictedResultToCurrent();
        }


        #endregion

        #region Predicted Result Setup


        private static void UpdateSwingsForRemovedParties(
            Dictionary<string, double> partyListSwings,
            Dictionary<string, double> partyConstituencySwings,
            out Dictionary<string, double> updatedListSwings,
            out Dictionary<string, double> updatedConstituencySwings)
        {
            updatedListSwings = new Dictionary<string, double>(partyListSwings);
            updatedConstituencySwings = new Dictionary<string, double>(partyConstituencySwings);
            foreach (var removedParty in RemovedPartyNames)
            {
                updatedListSwings.Add(removedParty, -99.99);
                updatedConstituencySwings.Add(removedParty, -99.99);
            }
        }

        private void ResetPredictedResultToCurrent()
        {
            _predictedResult = (ElectionResult)CurrentResult.Clone();
            _predictedResult.Name = "Predicted:" + CurrentResult.Name;
        }
        
        #endregion

        #region Party Forecasts

        private void PopulatePartyForecasts()
        {
            List<PartyForecast> partyConstituencyForecasts;
            List<PartyForecast> partyListForecasts;

            PopulatePartyForecastsFromCurrentResult(out partyConstituencyForecasts, out partyListForecasts);


            PartyConstituencyForecasts = partyConstituencyForecasts;
            PartyListForecasts = partyListForecasts;
        }

        private void PopulatePartyForecastsFromCurrentResult(
            out List<PartyForecast> partyConstituencyForecasts, out List<PartyForecast> partyListForecasts)
        {

            partyConstituencyForecasts = new List<PartyForecast>();
            partyListForecasts = new List<PartyForecast>();

            if (CurrentResult != null && CurrentResult.Regions.Count > 0)
            {
                foreach (var name in MajorPartyNames)
                {
                    double listPercentage =
                        CurrentResult.ListPercentagesByParty.ContainsKey(name) ?
                        CurrentResult.ListPercentagesByParty[name] : 0.0;

                    PartyForecast listForecast =
                        new PartyForecast(name, listPercentage);
                    partyListForecasts.Add(listForecast);

                    double constituencyPercentage =
                        CurrentResult.ConstituencyPercentagesByParty.ContainsKey(name) ?
                        CurrentResult.ConstituencyPercentagesByParty[name] : 0.0;

                    PartyForecast constituencyForecast =
                        new PartyForecast(name, constituencyPercentage);
                    partyConstituencyForecasts.Add(constituencyForecast);
                }
            }
            else
            {
                foreach (var name in MajorPartyNames)
                {
                    PartyForecast forecast = new PartyForecast(name, 0.0);
                    partyConstituencyForecasts.Add(forecast);
                    partyListForecasts.Add(forecast);
                }
            }
        }

        #endregion

        #region Paired Party Predictions

        public List<KeyValuePair<double, List<PartyForecast>>> FirstPartyVotesAndPairedPredictionTotals { get; private set; }

        public void GeneratePairPrediction(string firstPartyName, string secondPartyName)
        {
            FirstPartyVotesAndPairedPredictionTotals.Clear();

            double totalPairedListVote =
                PredictedResult.ListPercentagesByParty[firstPartyName] +
                PredictedResult.ListPercentagesByParty[secondPartyName];

            double increment = totalPairedListVote / 100.0;

            double firstPartyVote = totalPairedListVote;
            double secondPartyVote = 0.0;

            do
            {
                var pairedPredictedResult = (ElectionResult)_predictedResult.Clone();

                // update the swings to account for the removed parties
                Dictionary<string, double> updatedListSwings;
                Dictionary<string, double> updatedConstituencySwings;
                UpdateSwingsForRemovedParties(
                    _partyListSwings, _partyConstituencySwings,
                    out updatedListSwings, out updatedConstituencySwings);

                if (updatedListSwings.ContainsKey(firstPartyName))
                    updatedListSwings[firstPartyName] = firstPartyVote - _predictedResult.ListPercentagesByParty[firstPartyName];

                if (updatedListSwings.ContainsKey(secondPartyName))
                    updatedListSwings[secondPartyName] = secondPartyVote - _predictedResult.ListPercentagesByParty[secondPartyName];

                // apply the swings & recalculate the regions
                foreach (var region in pairedPredictedResult.Regions)
                {
                    region.ApplyPartySwings(updatedListSwings, updatedConstituencySwings);
                }

                List<PartyForecast> overallPairedResults =
                    GetOverallFromResultsFromPrediction(pairedPredictedResult);

                KeyValuePair<double, List<PartyForecast>> pairedResult =
                    new KeyValuePair<double, List<PartyForecast>>(firstPartyVote, overallPairedResults);
                FirstPartyVotesAndPairedPredictionTotals.Add(pairedResult);

                firstPartyVote -= increment;
                secondPartyVote += increment;
            }
            while (firstPartyVote >= -1e-7);
        }

        private List<PartyForecast> GetOverallFromResultsFromPrediction(ElectionResult pairedPredictedResult)
        {
            List<PartyForecast> overallPairedResults = new List<PartyForecast>();

            for(int i = 0; i < PartyOverallForecasts.Count; i++)
            {
                string name = PartyOverallForecasts[i].Name;
                PartyForecast forecast = 
                    new PartyForecast(name, 0.0);
                forecast.PreviousSeats = PartyOverallForecasts[i].PreviousSeats;

                int predictedList =
                    pairedPredictedResult.ListSeatsByParty.ContainsKey(name) ?
                    pairedPredictedResult.ListSeatsByParty[name] : 0;
                int predictedConstituency =
                    pairedPredictedResult.ConstituencySeatsByParty.ContainsKey(name) ?
                    pairedPredictedResult.ConstituencySeatsByParty[name] : 0;

                forecast.PredictedSeats = predictedConstituency + predictedList;

                overallPairedResults.Add(forecast);
            }
            return overallPairedResults;
        }

        #endregion

    }
}
