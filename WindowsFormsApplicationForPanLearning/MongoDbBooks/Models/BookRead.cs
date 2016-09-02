using System;
using System.Collections.Generic;
using System.Linq;


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoDbBooks.Models
{
    public enum BookFormat
    {
        Book = 1,
        Comic = 2,
        Audio = 3
    };

    public class MongoEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class BookRead : MongoEntity
    {
        // Date,DD/MM/YYYY,Author,Title,Pages,Note,Nationality,Original Language,Book,Comic,Audio

        #region Public Data

        [BsonElement("dateString")]
        public string DateString { get; set; }
        [BsonElement("date")]
        public DateTime Date { get; set; }
        [BsonElement("author")]
        public string Author { get; set; }
        [BsonElement("title")]
        public string Title { get; set; }
        [BsonElement("pages")]
        public UInt16 Pages { get; set; }
        [BsonElement("note")]
        public string Note { get; set; }
        [BsonElement("nationality")]
        public string Nationality { get; set; }
        [BsonElement("originalLanguage")]
        public string OriginalLanguage { get; set; }

        public string Book
        {
            get { return _isBook; }
            set
            {
                _isBook = value;
                if (!string.IsNullOrEmpty(_isBook)) Format = BookFormat.Book;
            }
        }
        public string Comic
        {
            get { return _isComic; }
            set
            {
                _isComic = value;
                if (!string.IsNullOrEmpty(_isComic)) Format = BookFormat.Comic;
            }
        }
        public string Audio
        {
            get { return _isAudio; }
            set
            {
                _isAudio = value;
                if (!string.IsNullOrEmpty(_isAudio)) Format = BookFormat.Audio;
            }
        }

        [BsonElement("format")]
        public BookFormat Format
        {
            get { return _bookFormat; }
            set
            {
                _bookFormat = value;
                switch (_bookFormat)
                {
                    case BookFormat.Book: _isBook = "x"; _isComic = _isAudio = ""; break;
                    case BookFormat.Audio: _isAudio = "x"; _isBook = _isComic = ""; break;
                    case BookFormat.Comic: _isComic = "x"; _isBook = _isAudio = ""; break;
                }
            }
        }

        #endregion

        #region Constructor

        public BookRead()
        {
            Format = BookFormat.Book;
        }

        #endregion

        #region Private Data

        private string _isBook;
        private string _isComic;
        private string _isAudio;

        private BookFormat _bookFormat;

        #endregion

    }
}
