using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogTranslations.Model
{
    public class FileMatch
    {
        string EnglishFileName { get; set; }
        string TranslationFileName { get; set; }
        FileTextElements EnglishTextElements { get; set; }
        FileTextElements TranslationTextElements { get; set; }

        public FileMatch(string englishFileName, string translationFileName,
            FileTextElements englishTextElements, FileTextElements translationTextElements)
        {
            EnglishFileName = englishFileName;
            TranslationFileName = translationFileName;
            EnglishTextElements = englishTextElements;
            TranslationTextElements = translationTextElements;
        }

        public List<string> GetTextElementMatchStrings()
        {
            List<string> outputLines = new List<string>();

            var englishTextElementKeys = EnglishTextElements.TextElements.Keys.ToList();

            // add lines for all the english tags and string
            List<string> matchedTags = new List<string>();
            foreach (var englishFileTag in englishTextElementKeys)
            {
                string outputLine = "\"" + EnglishFileName + "\","; // filename
                outputLine += "\"" + englishFileTag + "\","; // tag
                outputLine += "\"" + EnglishTextElements.TextElements[englishFileTag] + "\","; // English
                if (TranslationTextElements.TextElements.ContainsKey(englishFileTag))
                {
                    outputLine += "\"" + TranslationTextElements.TextElements[englishFileTag] + "\""; // Translation
                    matchedTags.Add(englishFileTag);
                }
                else
                    outputLine += "\"\""; // Translation

                outputLines.Add(outputLine);
            }
            // add lines for any missed translation elements
            List<string> missedTags = new List<string>();
            var translationTextElementKeys = TranslationTextElements.TextElements.Keys.ToList();
            foreach (var translationKey in translationTextElementKeys)
                if (!matchedTags.Contains(translationKey))
                    missedTags.Add(translationKey);

            foreach (var missedTag in missedTags)
            {
                string outputLine = "\"" + EnglishFileName + "\","; // filename
                outputLine += "\"" + missedTag + "\","; // tag
                outputLine += "\"\","; // English
                outputLine += "\"" + TranslationTextElements.TextElements[missedTag] + "\""; // Translation

                outputLines.Add(outputLine);
            }

            return outputLines;
        }
    }
}
