using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace DialogTranslations.Model
{
    public class FileTextElements
    {
        #region Constructors
        public FileTextElements(string fileName, ILog log)
        {
            FileName = fileName;
            Log = log;

            TextElements = new Dictionary<string, string>();

            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains("="))
                {
                    string[] words = line.Split('=');
                    if (words.Length > 1)
                    {
                        string tag = words[0];
                        int tagLength = tag.Length;
                        string text = line.Substring(tagLength+1);

                        if (!TextElements.ContainsKey(tag))
                            TextElements.Add(tag, text);
                        else
                            TextElements[tag] = text;

                        //Log.Debug("adding element -> tag  = " + tag + " , text = " + text);
                    }
                }
                Console.WriteLine(line);
            }

            file.Close();
            Log.Debug("Loaded " + TextElements.Count + " elements for  = " + fileName);

        }
        #endregion

        #region Properties
        public string FileName { get; set; }
        public ILog Log { get; set; }
        public Dictionary<string, string> TextElements { get; set; }
        #endregion
    }
}
