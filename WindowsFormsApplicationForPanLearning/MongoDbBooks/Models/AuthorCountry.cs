using System;
using System.Collections.Generic;
using System.Linq;


namespace MongoDbBooks.Models
{
    public class AuthorCountry
    {
        public string Country { get; set; }

        public UInt32 TotalPagesReadFromCountry
        {
            get
            {
                UInt32 total = 0;
                foreach (var author in AuthorsFromCountry) total += author.TotalPages;
                return total;
            }
        }
        public int TotalBooksReadFromCountry
        {
            get
            {
                int total = 0;
                foreach (var author in AuthorsFromCountry) total += author.TotalBooksReadBy;
                return total;
            }
        }

        public List<BookAuthor> AuthorsFromCountry { get; set; }

        public UInt32 TotalPagesWorldWide { get; set; }
        public int TotalBooksWorldWide { get; set; }

        public double PercentageOfBooksRead
        {
            get
            {
                if (TotalBooksWorldWide < 1) return 0.0;
                return 100.0 * ((double)TotalBooksReadFromCountry / (double)TotalBooksWorldWide);
            }
        }

        public double PercentageOfPagesRead
        {
            get
            {
                if (TotalPagesWorldWide < 1) return 0.0;
                return 100.0 * ((double)TotalPagesReadFromCountry / (double)TotalPagesWorldWide);
            }
        }

        public AuthorCountry()
        {
            Country = "";
            AuthorsFromCountry = new List<BookAuthor>();
            TotalPagesWorldWide = 1;
            TotalBooksWorldWide = 1;
        }
    }
}
