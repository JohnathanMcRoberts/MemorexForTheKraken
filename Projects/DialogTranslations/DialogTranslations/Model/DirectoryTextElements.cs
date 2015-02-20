using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using log4net;

namespace DialogTranslations.Model
{
    public class DirectoryTextElements
    {
        #region Constructors
        public DirectoryTextElements(string directoryName, ILog log)
        {
            Log = log;
            DirectoryName = directoryName;
            string[] files = Directory.GetFiles(directoryName);
            TextElementFiles = new Dictionary<string, FileTextElements>();
            foreach (var fileName in files)
            {
                if (fileName.Contains(".txt"))
                {
                    string shortName = Path.GetFileName( fileName );
                    Log.Debug("Loading elements for  = " + fileName);
                    TextElementFiles.Add(shortName, new FileTextElements(fileName, log));
                }
            }
        }
        #endregion

        #region Properties
        public Dictionary<string, FileTextElements> TextElementFiles { get; set; }
        public string DirectoryName { get; set; }
        public ILog Log { get; set; }
        #endregion
    }
}
