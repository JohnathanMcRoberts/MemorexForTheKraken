using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDbBooks.Models
{
    public class AuthorLanguage
    {
        public string Language { get; set; }

        public int TotalBooksReadInLanguage
        {
            get
            {
                int total = 0;
                foreach (var author in AuthorsInLanguage) total += author.TotalBooksReadBy;
                return total;
            }
        }

        public UInt32 TotalPagesReadInLanguage
        {
            get
            {
                UInt32 total = 0;
                foreach (var author in AuthorsInLanguage) total += author.TotalPages;
                return total;
            }
        }

        public List<BookAuthor> AuthorsInLanguage { get; set; }

        public int TotalBooksWorldWide { get; set; }

        public UInt32 TotalPagesWorldWide { get; set; }

        public double PercentageOfBooksRead
        {
            get
            {
                if (TotalBooksWorldWide < 1) return 0.0;
                return 100.0 * ((double)TotalBooksReadInLanguage / (double)TotalBooksWorldWide);
            }
        }

        public double PercentageOfPagesRead
        {
            get
            {
                if (TotalPagesWorldWide < 1) return 0.0;
                return 100.0 * ((double)TotalPagesReadInLanguage / (double)TotalPagesWorldWide);
            }
        }

        public AuthorLanguage()
        {
            Language = "";
            AuthorsInLanguage = new List<BookAuthor>();
            TotalPagesWorldWide = 1;
            TotalBooksWorldWide = 1;
        }
    }
}
