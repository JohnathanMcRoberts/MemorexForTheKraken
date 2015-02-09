using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using log4net;

namespace DialogTranslations.Model
{
    public class DialogTranslationsModel
    {
        public DialogTranslationsModel(ILog log)
        {
            Log = log;
            EnglishDirectoryName = Properties.Settings.Default.EnglishDirectory;
            TranslationDirectoryName = Properties.Settings.Default.TranslationDirectory;


            CompleteEnglishDirectoryName = Properties.Settings.Default.CompleteEnglishDirectory;
            CompleteTranslationDirectoryName = Properties.Settings.Default.CompleteTranslationDirectory;

            OutputFileName = Properties.Settings.Default.OutputFile;            
        }

        #region Properties
        public string EnglishDirectoryName { get; set; }
        public string TranslationDirectoryName { get; set; }
        public string OutputFileName { get; set; }

        public Boolean EnglishDirectoryNameSet
        {
            get { return EnglishDirectoryName != ""; }
        }
        public Boolean TranslationDirectoryNameSet
        {
            get { return EnglishDirectoryName != ""; }
        }

        public string CompleteEnglishDirectoryName { get; set; }
        public string CompleteTranslationDirectoryName { get; set; }

        public bool CompleteEnglishDirectoryNameSet
        {
            get { return CompleteEnglishDirectoryName != ""; }
        }
        public bool CompleteTranslationDirectoryNameSet
        {
            get { return CompleteTranslationDirectoryName != ""; }
        }


        public ILog Log { get; set; }
        #endregion

        #region Member variables

        public DirectoryTextElements _englishDirectoryElements;
        public DirectoryTextElements _translationDirectoryElements;

        #endregion
        
        #region Public Functions

        public void GenerateTranslationCsv()
        {
            Log.Debug("EnglishDirectoryName = " + EnglishDirectoryName);
            Log.Debug("TranslationDirectoryName = " + TranslationDirectoryName);
            Log.Debug("OutputFileFieldName = " + OutputFileName);

            // get the files and the text elements within
            _englishDirectoryElements = new DirectoryTextElements(EnglishDirectoryName, Log);
            _translationDirectoryElements = new DirectoryTextElements(TranslationDirectoryName, Log);

            // get the matching english and translation files
            List<FileMatch> fileMatches = GetFileMatches();

            // open the ouput file & write the header
            TextWriter tw = new StreamWriter(new FileStream(OutputFileName, FileMode.Create), Encoding.UTF8);
            tw.WriteLine("\"Filename\",\"Dialog Element Tag\",\"English\",\"Translation\"");

            // write out the matches for the files
            foreach (var match in fileMatches)
            {
                foreach (var outputLine in match.GetTextElementMatchStrings())
                {
                    tw.WriteLine(outputLine);
                }
            }
            tw.Close();
        }

        internal void GenerateCompleteTranslationCsv()
        {
            Log.Debug("CompleteEnglishDirectoryName = " + CompleteEnglishDirectoryName);
            Log.Debug("CompleteTranslationDirectoryName = " + CompleteTranslationDirectoryName);
            Log.Debug("OutputFileFieldName = " + OutputFileName);

            // open the ouput file & write the header
            TextWriter tw = new StreamWriter(new FileStream(OutputFileName, FileMode.Create), Encoding.UTF8);
            tw.WriteLine("\"Filename\",\"Dialog Element Tag\",\"English\",\"Translation\"");


            List<string> englishTextDirs = GetTextDirectories(CompleteEnglishDirectoryName);
            List<string> translationTextDirs = GetTextDirectories(CompleteTranslationDirectoryName);

            foreach (var englishDirName in englishTextDirs)
            {
                string translationDirName = GetMatchingTranslationDir(englishDirName, translationTextDirs);
                if (translationDirName == "") continue;

                // get the files and the text elements within
                _englishDirectoryElements = new DirectoryTextElements(englishDirName, Log);
                _translationDirectoryElements = new DirectoryTextElements(translationDirName, Log);

                // get the matching english and translation files
                List<FileMatch> fileMatches = GetFileMatches();

                // write out the matches for the files
                foreach (var match in fileMatches)
                {
                    foreach (var outputLine in match.GetTextElementMatchStrings())
                    {
                        tw.WriteLine(outputLine);
                    }
                }
            }
            tw.Close();

        }

        #endregion

        #region Utility Functions

        private List<FileMatch> GetFileMatches()
        {
            List<FileMatch> fileMatches = new List<FileMatch>();
            var englishFileNames = _englishDirectoryElements.TextElementFiles.Keys.ToList();
            var translationFileNames = _translationDirectoryElements.TextElementFiles.Keys.ToList();
            foreach (var englishFileName in englishFileNames)
            {
                string[] parts = englishFileName.Split('.');
                if (!(parts.Length > 1)) continue;
                string englishWithoutExtension = parts[0];

                foreach (var translationFileName in translationFileNames)
                {
                    parts = translationFileName.Split('.');
                    if (!(parts.Length > 1)) continue;
                    string translationWithoutExtension = parts[0];

                    if (translationWithoutExtension.ToUpper() == englishWithoutExtension.ToUpper())
                    {
                        FileMatch match = new FileMatch(englishFileName, translationFileName,
                            _englishDirectoryElements.TextElementFiles[englishFileName],
                            _translationDirectoryElements.TextElementFiles[translationFileName]);
                        fileMatches.Add(match);
                        break;
                    }
                }
            }
            return fileMatches;
        }

        private string GetShortParent(string textDir)
        {
            string parent = Directory.GetParent(textDir).ToString();
            string grandParent = Directory.GetParent(parent).ToString();
            string shortParent = parent.Substring(grandParent.Length + 1);

            return shortParent;
        }

        private string GetMatchingTranslationDir(string englishDirName, List<string> translationTextDirs)
        {
            string shortEnglishParent =GetShortParent(englishDirName);

            foreach (var translationDir in translationTextDirs)
            {
                string shortTranslationParent = GetShortParent(translationDir);
                if (shortTranslationParent == shortEnglishParent)
                    return translationDir;
            }        

            return "";
        }

        private List<string> GetTextDirectories(string directoryName)
        {
            List<string> textDirectories = new List<string>();

            // Recurse into subdirectories of this directory. 
            string[] subdirectoryEntries = Directory.GetDirectories(directoryName);

            foreach (var subdirectory in subdirectoryEntries)
            {
                string[] textSubdirectoryEntries = Directory.GetDirectories(subdirectory,"Text*");
                foreach (var textDir in textSubdirectoryEntries)
                    textDirectories.Add(textDir);
            }
            return textDirectories;
        }

        #endregion



    }
}
