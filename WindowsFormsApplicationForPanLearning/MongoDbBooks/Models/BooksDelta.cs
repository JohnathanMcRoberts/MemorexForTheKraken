using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDbBooks.Models
{
    public class BooksDelta
    {
        #region Public Data

        public DateTime Date { get; private set; }

        public DateTime StartDate { get; private set; }

        public List<BookRead> BooksReadToDate { get; set; }

        public int DaysSinceStart { get; private set; }

        public DeltaTally OverallTally { get; private set; }

        public DeltaTally LastTenTally { get; private set; }

        #endregion

        #region Constructor

        public BooksDelta(DateTime date, DateTime startDate)
        {
            Date = date;
            StartDate = startDate;
            TimeSpan ts = date - startDate;
            DaysSinceStart = ts.Days;
            BooksReadToDate = new List<BookRead>();

            OverallTally = new DeltaTally();
            LastTenTally = new DeltaTally();
        }

        #endregion

        #region Nested Classes

        public class DeltaTally
        {
            public int DaysInTally { get; set; }

            public UInt32 TotalPages { get; set; }
            public UInt32 TotalBooks { get; set; }

            public UInt32 TotalBookFormat { get; set; }
            public UInt32 TotalComicFormat { get; set; }
            public UInt32 TotalAudioFormat { get; set; }

            public double PercentageInEnglish { get; set; }
            public double PercentageInTranslation { get { return 100.0 - PercentageInEnglish; } }

            public double PageRate { get { return (double)TotalPages / (double)DaysInTally; } }
            public double DaysPerBook { get { return (double)DaysInTally / (double)TotalBooks; } }
            public double PagesPerBook { get { return (double)TotalPages / (double)TotalBooks; } }
            public double BooksPerYear { get { return 365.25 / (double)DaysPerBook; } }

            public List<Tuple<string, UInt32, double, UInt32, double>> LanguageTotals { get; set; }
            public List<Tuple<string, UInt32, double, UInt32, double>> CountryTotals { get; set; }

            public DeltaTally()
            {
                LanguageTotals = new List<Tuple<string, UInt32, double, UInt32, double>>();
                CountryTotals = new List<Tuple<string, UInt32, double, UInt32, double>>();
            }
        }

        #endregion

        #region Public Functions

        public void UpdateTallies()
        {
            // start with the overall
            OverallTally = GetDeltaTallyValues(BooksReadToDate);

            // then get the last 10 books in a list
            List<BookRead> lastTenBooks = new List<BookRead>();
            if (BooksReadToDate.Count <= 10)
                lastTenBooks = BooksReadToDate;
            else
            {
                for (int i = BooksReadToDate.Count - 10; i < BooksReadToDate.Count; i++)
                    lastTenBooks.Add(BooksReadToDate[i]);
            }

            // then update the tally based on that list
            LastTenTally = GetDeltaTallyValues(lastTenBooks);
        }

        #endregion

        #region Utility Functions

        private DeltaTally GetDeltaTallyValues(List<BookRead> books)
        {
            DeltaTally tally = new DeltaTally();
            UInt32 totalBooks = 0;
            UInt32 totalPagesRead = 0;
            UInt32 totalBookFormat = 0;
            UInt32 totalComicFormat = 0;
            UInt32 totalAudioFormat = 0;
            UInt32 totalInEnglish = 0;
            int daysInTally = 0;

            daysInTally = (books.Last().Date - books.First().Date).Days;
            if (daysInTally < 1) daysInTally = 1;

            Dictionary<string, Tuple<UInt32, UInt32>> languageCounts =
                new Dictionary<string, Tuple<UInt32, UInt32>>();
            Dictionary<string, Tuple<UInt32, UInt32>> countryCounts =
                new Dictionary<string, Tuple<UInt32, UInt32>>();

            foreach (var book in books)
            {
                totalBooks++;
                totalPagesRead += book.Pages;
                if (book.Format == BookFormat.Book) totalBookFormat++;
                if (book.Format == BookFormat.Comic) totalComicFormat++;
                if (book.Format == BookFormat.Audio) totalAudioFormat++;
                if (book.OriginalLanguage == "English") totalInEnglish++;
                UpdateLanguageAndCountryCounts(languageCounts, countryCounts, book);

            }
            double percentageInEnglish = GetAsPercentage(totalBooks, totalInEnglish);
            tally.DaysInTally = daysInTally;
            tally.TotalPages = totalPagesRead;
            tally.TotalBooks = totalBooks;
            tally.TotalBookFormat = totalBookFormat;
            tally.TotalComicFormat = totalComicFormat;
            tally.TotalAudioFormat = totalAudioFormat;
            tally.PercentageInEnglish = percentageInEnglish;

            foreach (string language in languageCounts.Keys)
            {
                tally.LanguageTotals.Add(
                    new Tuple<string, UInt32, double, UInt32, double>(
                        language, languageCounts[language].Item1,
                        GetAsPercentage(totalBooks, languageCounts[language].Item1),
                        languageCounts[language].Item2,
                        GetAsPercentage(totalPagesRead, languageCounts[language].Item2))
                        );
            }

            foreach (string country in countryCounts.Keys)
            {
                tally.CountryTotals.Add(
                    new Tuple<string, UInt32, double, UInt32, double>(
                        country, countryCounts[country].Item1,
                        GetAsPercentage(totalBooks, countryCounts[country].Item1),
                        countryCounts[country].Item2,
                        GetAsPercentage(totalPagesRead, countryCounts[country].Item2))
                        );
            }

            return tally;
        }

        private static void UpdateLanguageAndCountryCounts(
            Dictionary<string, Tuple<UInt32, UInt32>> languageCounts,
            Dictionary<string, Tuple<UInt32, UInt32>> countryCounts,
            BookRead book)
        {

            if (!languageCounts.ContainsKey(book.OriginalLanguage))
                languageCounts.Add(book.OriginalLanguage, new Tuple<UInt32, UInt32>(1, book.Pages));
            else
            {
                var updatedCounts =
                    new Tuple<UInt32, UInt32>(
                            languageCounts[book.OriginalLanguage].Item1 + 1,
                            languageCounts[book.OriginalLanguage].Item2 + book.Pages);
                languageCounts[book.OriginalLanguage] = updatedCounts;
            }

            if (!countryCounts.ContainsKey(book.Nationality))
                countryCounts.Add(book.Nationality, new Tuple<UInt32, UInt32>(1, book.Pages));
            else
            {
                var updatedCounts =
                    new Tuple<UInt32, UInt32>(
                            countryCounts[book.Nationality].Item1 + 1,
                            countryCounts[book.Nationality].Item2 + book.Pages);
                countryCounts[book.Nationality] = updatedCounts;
            }
        }

        private static double GetAsPercentage(UInt32 totalBooks, UInt32 totalInEnglish)
        {
            double percentageInEnglish = 100.0 * ((double)totalInEnglish / (double)totalBooks);
            return percentageInEnglish;
        }

        #endregion
    }
}
