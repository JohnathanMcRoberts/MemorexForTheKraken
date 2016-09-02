using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;

using CsvHelper;

using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDbBooks.Models
{
    public class MainBooksModel
    {
        #region Private Data

        private log4net.ILog _log;

        protected static IMongoClient _client;
        protected static IMongoDatabase _database;

        #endregion

        #region Constructor

        public MainBooksModel(log4net.ILog log)
        {
            _log = log;

            BooksRead = new ObservableCollection<BookRead>();
            AuthorsRead = new ObservableCollection<BookAuthor>();
            AuthorCountries = new ObservableCollection<AuthorCountry>();
            AuthorLanguages = new ObservableCollection<AuthorLanguage>();
            TalliedBooks = new ObservableCollection<TalliedBook>();
            BookDeltas = new ObservableCollection<BooksDelta>();
            BookPerYearDeltas = new ObservableCollection<BooksDelta>();

            InputFilePath = Properties.Settings.Default.InputFile;
            OutputFilePath = Properties.Settings.Default.OutputFile;

            InputFilePath = Properties.Settings.Default.InputFile;
            OutputFilePath = Properties.Settings.Default.OutputFile;

            string errorMsg = "test";
            if (!ConnectToDatabase(out errorMsg))
                _log.Debug("error connecting to db : " + errorMsg);

        }

        #endregion

        #region Connection strings

        public const string DatabaseConnectionString = "mongodb://localhost:27017";

        #endregion

        #region Public Data

        public ObservableCollection<BookRead> BooksRead { get; private set; }

        public ObservableCollection<BookAuthor> AuthorsRead { get; private set; }

        public ObservableCollection<AuthorCountry> AuthorCountries { get; private set; }

        public ObservableCollection<AuthorLanguage> AuthorLanguages { get; private set; }

        public ObservableCollection<TalliedBook> TalliedBooks { get; private set; }

        public ObservableCollection<BooksDelta> BookDeltas { get; private set; }

        public ObservableCollection<BooksDelta> BookPerYearDeltas { get; private set; }

        public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }

        public bool DataFromFile { get; set; }
        public bool DataFromDb { get; set; }

        #endregion

        #region Public Methods

        public void ReadBooksFromFile(string filename)
        {

            using (var sr = new StreamReader(filename, Encoding.Default))
            {
                var csv = new CsvReader(sr);

                BooksRead.Clear();

                // Date,DD/MM/YYYY,Author,Title,Pages,Note,Nationality,Original Language,Book,Comic,Audio
                while (csv.Read())
                {
                    var stringFieldDate = csv.GetField<string>(0);
                    var stringFieldDDMMYYYY = csv.GetField<string>(1);
                    var stringFieldAuthor = csv.GetField<string>(2);
                    var stringFieldTitle = csv.GetField<string>(3);
                    var stringFieldPages = csv.GetField<string>(4);
                    var stringFieldNote = csv.GetField<string>(5);
                    var stringFieldNationality = csv.GetField<string>(6);
                    var stringFieldOriginalLanguage = csv.GetField<string>(7);
                    var stringFieldBook = csv.GetField<string>(8);
                    var stringFieldComic = csv.GetField<string>(9);
                    var stringFieldAudio = csv.GetField<string>(10);

                    DateTime dateForBook;
                    if (DateTime.TryParseExact(stringFieldDDMMYYYY, "d/M/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out dateForBook))
                    {
                        UInt16 pages = 0;
                        UInt16.TryParse(stringFieldPages, out pages);
                        BookRead book = new BookRead()
                        {
                            DateString = stringFieldDate,
                            Date = dateForBook,
                            Author = stringFieldAuthor,
                            Title = stringFieldTitle,
                            Pages = pages,
                            Note = stringFieldNote,
                            Nationality = stringFieldNationality,
                            OriginalLanguage = stringFieldOriginalLanguage,
                            Audio = stringFieldAudio,
                            Book = stringFieldBook,
                            Comic = stringFieldComic,
                        };

                        BooksRead.Add(book);
                    }
                }
            }
            UpdateCollections();
            Properties.Settings.Default.InputFile = filename;
            Properties.Settings.Default.Save();
            DataFromFile = true;
        }

        public void WriteBooksToFile(string filename)
        {
            StreamWriter sw = new StreamWriter(filename, false, Encoding.Default); //overwrite original file

            // write the header
            sw.WriteLine(
                "Date,DD/MM/YYYY,Author,Title,Pages,Note,Nationality,Original Language,Book,Comic,Audio"
                );

            // write the records
            var csv = new CsvWriter(sw);
            foreach (var book in BooksRead)
            {
                csv.WriteField(book.DateString);
                csv.WriteField(book.Date.ToString("d/M/yyyy"));
                csv.WriteField(book.Author);
                csv.WriteField(book.Title);
                csv.WriteField(book.Pages > 0 ? book.Pages.ToString() : "");
                csv.WriteField(book.Note);
                csv.WriteField(book.Nationality);
                csv.WriteField(book.OriginalLanguage);
                csv.WriteField(book.Format == BookFormat.Book ? "x" : "");
                csv.WriteField(book.Format == BookFormat.Comic ? "x" : "");
                csv.WriteField(book.Format == BookFormat.Audio ? "x" : "");
                csv.NextRecord();
            }

            // tidy up
            sw.Close();

            //update the settings
            Properties.Settings.Default.OutputFile = filename;
            Properties.Settings.Default.Save();
        }

        public bool ConnectToDatabase(out string errorMsg)
        {
            errorMsg = "";
            try
            {
                _client = new MongoClient(DatabaseConnectionString);
                _database = _client.GetDatabase("books_read");

                IMongoCollection<BookRead> booksRead = _database.GetCollection<BookRead>("books");

                // this is a dummy query to get everything to date...

                var filter = Builders<BookRead>.Filter.Lte(
                    new StringFieldDefinition<BookRead, BsonDateTime>("date"), new BsonDateTime(DateTime.Now));

                long totalCount = booksRead.Count(filter);

                if (totalCount == 0 && BooksRead.Count != 0)
                {
                    booksRead.InsertMany(BooksRead);
                    totalCount = booksRead.Count(filter);

                }
                else if (totalCount != 0 && BooksRead.Count == 0)
                {

                    BooksRead.Clear();

                    using (var cursor = booksRead.FindSync(filter))
                    {
                        var booksList = cursor.ToList();
                        foreach (var book in booksList)
                        {
                            BooksRead.Add(book);
                        }
                    }
                    UpdateCollections();
                    DataFromFile = false;
                    DataFromDb = true;
                }

            }
            catch (Exception e)
            {
                errorMsg = e.ToString();
                return false;
            }

            return true;
        }

        public bool AddNewBook(BookRead newBook, out string errorMsg)
        {
            errorMsg = "";

            // for the moment insist only that the date is after the last of the existing items
            if (BooksRead.Last().Date > newBook.Date)
            {
                errorMsg = "Date must be after last date : " + BooksRead.Last().DateString;
                return false;
            }

            BooksRead.Add(newBook);

            if (DataFromDb)
                AddNewBookToDatabase(newBook);

            UpdateCollections();

            return true;
        }

        public bool UpdateBook(BookRead editBook, out string errorMsg)
        {
            errorMsg = "";

            if (DataFromDb)
                UpdateBookInDatabase(editBook);

            UpdateCollections();

            return true;
        }

        #endregion

        #region Private Methods

        private void UpdateCollections()
        {
            UpdateAuthors();
            int booksReadWorldwide = 0;
            UInt32 pagesReadWorldwide = 0;
            UpdateCountries(ref booksReadWorldwide, ref pagesReadWorldwide);
            UpdateLanguages(booksReadWorldwide, pagesReadWorldwide);
            UpdateTalliedBooks();
            UpdateBookDeltas();
            UpdateBookPerYearDeltas();
        }

        private void UpdateBookPerYearDeltas()
        {
            // clear the list and the counts
            BookPerYearDeltas.Clear();
            if (BooksRead.Count < 1) return;
            DateTime startDate = BooksRead[0].Date;
            startDate = startDate.AddYears(1);

            // get all the dates a book has been read that are at least a year since the start
            Dictionary<DateTime, DateTime> bookReadDates = new Dictionary<DateTime, DateTime>();
            foreach (var book in BooksRead)
            {
                if (startDate > book.Date) continue;
                if (!bookReadDates.ContainsKey(book.Date))
                {
                    bookReadDates.Add(book.Date, book.Date);
                }
            }

            // then add the delta made up of the books up to that date
            foreach (var date in bookReadDates.Keys.ToList())
            {
                DateTime startYearDate = date;
                startYearDate = startYearDate.AddYears(-1);
                BooksDelta delta = new BooksDelta(date, startYearDate);
                foreach (var book in BooksRead)
                {
                    if (book.Date < startYearDate)
                        continue;

                    if (book.Date <= date)
                        delta.BooksReadToDate.Add(book);
                    else
                        break;
                }
                delta.UpdateTallies();
                BookPerYearDeltas.Add(delta);
            }
        }

        private void UpdateBookDeltas()
        {
            // clear the list and the counts
            BookDeltas.Clear();
            if (BooksRead.Count < 1) return;
            DateTime startDate = BooksRead[0].Date;

            // get all the dates a book has been read (after the first quarter)
            Dictionary<DateTime, DateTime> bookReadDates = new Dictionary<DateTime, DateTime>();
            foreach (var book in BooksRead)
            {
                if (!bookReadDates.ContainsKey(book.Date))
                {
                    TimeSpan ts = book.Date - startDate;
                    if (ts.Days >= 90)
                        bookReadDates.Add(book.Date, book.Date);
                }
            }

            // then add the delta made up of the books up to that date
            foreach (var date in bookReadDates.Keys.ToList())
            {
                BooksDelta delta = new BooksDelta(date, startDate);
                foreach (var book in BooksRead)
                {
                    if (book.Date <= date)
                        delta.BooksReadToDate.Add(book);
                    else
                        break;
                }
                delta.UpdateTallies();
                BookDeltas.Add(delta);
            }
        }

        private void UpdateTalliedBooks()
        {
            // clear the list and the counts
            TalliedBooks.Clear();
            UInt32 totalBooks = 0;
            UInt32 totalPagesRead = 0;
            UInt32 totalBookFormat = 0;
            UInt32 totalComicFormat = 0;
            UInt32 totalAudioFormat = 0;

            // The assumption is the books arrive in order (as they do)
            List<TalliedBook> booksTally = new List<TalliedBook>();
            foreach (var book in BooksRead)
            {
                totalBooks++;
                totalPagesRead += book.Pages;
                if (book.Format == BookFormat.Book) totalBookFormat++;
                if (book.Format == BookFormat.Comic) totalComicFormat++;
                if (book.Format == BookFormat.Audio) totalAudioFormat++;

                TalliedBook tally = new TalliedBook(book)
                {
                    TotalBooks = totalBooks,
                    TotalAudioFormat = totalAudioFormat,
                    TotalBookFormat = totalBookFormat,
                    TotalComicFormat = totalComicFormat,
                    TotalPagesRead = totalPagesRead
                };

                booksTally.Add(tally);
            }

            // finally need to sort them into datae descending
            var sortedTallies =
                (from item in booksTally orderby item.TotalBooks descending select item);
            foreach (var tallied in sortedTallies)
                TalliedBooks.Add(tallied);
        }

        private void UpdateLanguages(int booksReadWorldwide, uint pagesReadWorldwide)
        {
            // clear the list
            AuthorLanguages.Clear();

            // get the uniquely named languages
            Dictionary<string, AuthorLanguage> languageSet = new Dictionary<string, AuthorLanguage>();
            foreach (var author in AuthorsRead)
            {
                if (languageSet.ContainsKey(author.Language))
                    languageSet[author.Language].AuthorsInLanguage.Add(author);
                else
                {
                    AuthorLanguage language =
                        new AuthorLanguage() { Language = author.Language };
                    language.AuthorsInLanguage.Add(author);
                    languageSet.Add(language.Language, language);
                }
            }

            // Update the language totals + add to the list
            foreach (var language in languageSet.Values.ToList())
            {
                language.TotalBooksWorldWide = booksReadWorldwide;
                language.TotalPagesWorldWide = pagesReadWorldwide;
                AuthorLanguages.Add(language);
            }
        }

        private void UpdateCountries(ref int booksReadWorldwide, ref UInt32 pagesReadWorldwide)
        {
            // clear the list & counts
            AuthorCountries.Clear();
            booksReadWorldwide = 0;
            pagesReadWorldwide = 0;

            // get the uniquely named countries + the counts
            Dictionary<string, AuthorCountry> countrySet = new Dictionary<string, AuthorCountry>();
            foreach (var author in AuthorsRead)
            {
                booksReadWorldwide += author.TotalBooksReadBy;
                pagesReadWorldwide += author.TotalPages;

                if (countrySet.ContainsKey(author.Nationality))
                    countrySet[author.Nationality].AuthorsFromCountry.Add(author);
                else
                {
                    AuthorCountry country =
                        new AuthorCountry() { Country = author.Nationality };
                    country.AuthorsFromCountry.Add(author);
                    countrySet.Add(country.Country, country);
                }
            }

            // Update the country totals + add to the list
            foreach (var country in countrySet.Values.ToList())
            {
                country.TotalBooksWorldWide = booksReadWorldwide;
                country.TotalPagesWorldWide = pagesReadWorldwide;
                AuthorCountries.Add(country);
            }
        }

        private void UpdateAuthors()
        {
            AuthorsRead.Clear();

            Dictionary<string, BookAuthor> authorsSet = new Dictionary<string, BookAuthor>();
            foreach (var book in BooksRead)
            {
                if (authorsSet.ContainsKey(book.Author))
                    authorsSet[book.Author].BooksReadBy.Add(book);
                else
                {
                    BookAuthor author =
                        new BookAuthor() { Author = book.Author, Language = book.OriginalLanguage, Nationality = book.Nationality };
                    author.BooksReadBy.Add(book);
                    authorsSet.Add(book.Author, author);
                }
            }
            foreach (var author in authorsSet.Values.ToList())
                AuthorsRead.Add(author);
        }

        private void AddNewBookToDatabase(BookRead newBook)
        {
            _client = new MongoClient(DatabaseConnectionString);
            _database = _client.GetDatabase("books_read");

            IMongoCollection<BookRead> booksRead = _database.GetCollection<BookRead>("books");

            booksRead.InsertOne(newBook);
        }

        private void UpdateBookInDatabase(BookRead editBook)
        {
            _client = new MongoClient(DatabaseConnectionString);
            _database = _client.GetDatabase("books_read");

            IMongoCollection<BookRead> booksRead = _database.GetCollection<BookRead>("books");

            var filterOnId = Builders<BookRead>.Filter.Eq(s => s.Id, editBook.Id);

            long totalCount = booksRead.Count(filterOnId);

            var result = booksRead.ReplaceOne(filterOnId, editBook);

        }
        #endregion

    }
}
