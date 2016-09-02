using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDbBooks.Models
{
    public class BookAuthor
    {
        public string Author { get; set; }

        public string Nationality { get; set; }
        public string Language { get; set; }

        public UInt16 TotalPages
        {
            get
            {
                UInt16 total = 0;
                foreach (var book in BooksReadBy) total += book.Pages;
                return total;
            }
        }
        public int TotalBooksReadBy { get { return BooksReadBy.Count; } }

        public List<BookRead> BooksReadBy { get; set; }

        public BookAuthor()
        {
            Author = "";
            Nationality = "";
            Language = "";
            BooksReadBy = new List<BookRead>();
        }
    }
}
