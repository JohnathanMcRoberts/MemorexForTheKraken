using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using log4net;
using SqliteReader.Writers;

namespace SqliteReader.Readers
{
    public class ConstituencyKmlReader
    {
        #region Constructors
        public ConstituencyKmlReader(ILog log)
        {
            Log = log;

            using (SQLiteConnection conread = new SQLiteConnection("Data Source=" + _fullPath))
            {
                string selectSQL = "SELECT id,kml FROM swdata";// LIMIT 5";
                conread.Open();
                KmlToXmlWriter kmlWriter = new KmlToXmlWriter();


                using (SQLiteCommand cmd = new SQLiteCommand(selectSQL, conread))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            log.Debug(rdr.GetInt32(0) + " "
                                + rdr.GetString(1).Substring(0, 500) + "...");

                            kmlWriter.AddKmlBlock(rdr.GetString(1));
                        }
                    }
                }
                kmlWriter.CompleteDocument();
                conread.Close();

            }

        }

        #endregion

        #region private data

        private log4net.ILog Log;
        private readonly string _fullPath = 
            "C:\\Users\\E142890\\Downloads\\uk_parliamentary_constituency_boundaries_kml.sqlite";

        #endregion
    }


}
