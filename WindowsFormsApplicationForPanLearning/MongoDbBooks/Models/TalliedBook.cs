using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDbBooks.Models
{
    public class TalliedBook
    {
        public BookRead Book { get; private set; }

        public string DateString { get { return Book.DateString; } }

        public DateTime Date { get { return Book.Date; } }

        public string Author { get { return Book.Author; } }
        public string Title { get { return Book.Title; } }
        public UInt32 Pages { get { return Book.Pages; } }

        public UInt32 TotalBooks { get; set; }

        public UInt32 TotalBookFormat { get; set; }
        public UInt32 TotalComicFormat { get; set; }
        public UInt32 TotalAudioFormat { get; set; }

        public UInt32 TotalPagesRead { get; set; }

        public TalliedBook(BookRead book)
        {
            Book = book;
        }
    }
}
