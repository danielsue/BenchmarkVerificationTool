using System;
using System.Collections.Generic;
using System.Text;

namespace MIN3PBenchmark
{
    class SpecialFileIgnoreLines
    {
        public SpecialFileIgnoreLines(string extensionName)
        {
            _extensionName = extensionName;
        }

        private string _extensionName = string.Empty;
        /// <summary>
        /// Extension name
        /// </summary>
        public string ExtensionName
        {
            get { return _extensionName; }
            set { _extensionName = value; }
        }

        private List<string> _ignoredLineStartString = new List<string>();
        /// <summary>
        /// Ignored Line Start String
        /// </summary>
        public List<string> IgnoredLineStartString
        {
            get { return _ignoredLineStartString; }
            set { _ignoredLineStartString = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strStart"></param>
        /// <returns></returns>
        public bool MatchIgnoredStart(string strLine)
        {
            for (int i = 0; i < _ignoredLineStartString.Count; i++)
            {
                if (strLine.StartsWith(_ignoredLineStartString[i]))
                    return true;
            }
            return false;
        }
    }
}
