using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DifferenceEngine;


namespace MIN3PBenchmark
{
    class ResultCompare
    {
        public ResultCompare()
        {}

        private FileLog _log = new FileLog();

        public FileLog Log
        {
            set { _log = value; }
        }

        private DiffEngineLevel _level = DiffEngineLevel.FastImperfect;
        private LineCompareMode _lineCompMode = LineCompareMode.SameLineCompare;
        /// <summary>
        /// set line compare mode
        /// </summary>
        public LineCompareMode LineCompMode
        {
            set { _lineCompMode = value; }
            get { return _lineCompMode; }
        }

        private int _difCollectNumber = 100;
        /// <summary>
        /// Set difference collection number
        /// </summary>
        public int DifCollectNumber
        {
            get { return _difCollectNumber; }
            set { _difCollectNumber = value; }
        }

        
        private string _resultPath = string.Empty;
        /// <summary>
        /// Result path
        /// </summary>
        public string ResultPath
        {
            get { return _resultPath; }
        }

        private string _refResultPath = string.Empty;
        /// <summary>
        /// Referenced result path
        /// </summary>
        public string RefResultPath
        {
            get { return _refResultPath; }
        }

        private List<string> _resultFileList = new List<string>();
        /// <summary>
        /// Result file list
        /// </summary>
        public List<string> ResultFileList
        {
            get { return _resultFileList; }
            set { _resultFileList = value; }
        }

        private List<string> _refResultFileList = new List<string>();
        /// <summary>
        /// Referenced result file list
        /// </summary>
        public List<string> RefResultFileList
        {
            get { return _refResultFileList; }
        }

        private List<string> _difInResultFileList = new List<string>();
        /// <summary>
        /// Difference in result file list, DifInResultFileList[i] -> ResultFileList[i]
        /// </summary>
        public List<string> DifInResultFileList
        {
            get { return _difInResultFileList; }
        }

        private string _newAddedFilesInResult = string.Empty;
        /// <summary>
        /// Missing file report
        /// </summary>
        public string NewAddedFilesInResult
        {
            get { return _newAddedFilesInResult; }
        }

        private string _notFoundFilesInResult = string.Empty;
        /// <summary>
        /// Missing file report
        /// </summary>
        public string NotFoundFilesInResult
        {
            get { return _notFoundFilesInResult; }
        }


        private List<int> _indexResultInRef = new List<int>();

        private List<int> _indexRefInResult = new List<int>();

        private int _countOfResultMissingFile = 0;
        /// <summary>
        /// Count Of Result Missing File
        /// </summary>
        public int CountOfResultMissingFile
        {
            get { return _countOfResultMissingFile; }
        }

        private int _countOfRefResultMissingFile = 0;
        /// <summary>
        ///  Count Of Referenced Result Missing File
        /// </summary>
        public int CountOfRefResultMissingFile
        {
            get { return _countOfRefResultMissingFile; }
        }


        /// <summary>
        /// Initialize the current compare
        /// </summary>
        /// <param name="resultFolder"></param>
        /// <param name="referencedResultFolder"></param>
        public void InitialCompare(string resultFolder, string referencedResultFolder)
        {
            _resultPath = resultFolder;
            _refResultPath = referencedResultFolder;
            _resultFileList = new List<string>();
            _refResultFileList = new List<string>();

            _indexResultInRef = new List<int>();
            _indexRefInResult = new List<int>();

            _difInResultFileList = new List<string>();

            _newAddedFilesInResult = string.Empty;
            _notFoundFilesInResult = string.Empty;

            try
            {

                //find all the files that need to compare
                string[] fileList = Directory.GetFiles(resultFolder, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < fileList.Length; i++)
                {
                    if (!_ignoredCompFileExtNameList.Contains(Path.GetExtension(fileList[i])))
                    {
                        _resultFileList.Add(fileList[i]);
                        _indexResultInRef.Add(-1);
                        _difInResultFileList.Add(string.Empty);
                    }
                }
                //find all the file that need to be compared to
                fileList = Directory.GetFiles(referencedResultFolder, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < fileList.Length; i++)
                {
                    if (!_ignoredCompFileExtNameList.Contains(Path.GetExtension(fileList[i])))
                    {
                        _refResultFileList.Add(fileList[i]);
                        _indexRefInResult.Add(-1);
                    }
                }
            }
            catch
            {
                _log.Write("Error occurred in getting files from one of the following folder, files not found:" + Environment.NewLine + resultFolder + Environment.NewLine + referencedResultFolder);
                
            }
        }

        /// <summary>
        /// Create indexes between result file list and referenced result file list
        /// </summary>
        public void FindResultFileIndex()
        {
            int nMissing = 0;
            string str = string.Empty;
            for (int i = 0; i < _resultFileList.Count; i++)
            {
                str = _resultFileList[i].Replace(_resultPath, _refResultPath);

                if (i < _refResultFileList.Count)
                {
                    if (str == _refResultFileList[i])
                    {
                        _indexResultInRef[i] = i;
                        _indexRefInResult[i] = i;
                        continue;
                    }
                }

                for (int j = 0; j < _refResultFileList.Count; j++)
                {
                    if (str == _refResultFileList[j])
                    {
                        _indexResultInRef[i] = j;
                        _indexRefInResult[j] = i;
                        break;
                    }
                }

                if (_indexResultInRef[i] == -1)
                {
                    nMissing++;
                    _newAddedFilesInResult += _resultFileList[i] + Environment.NewLine;
                }
            }
            _countOfRefResultMissingFile = nMissing;

            nMissing = 0;
            for (int i = 0; i < _refResultFileList.Count; i++)
            {
                if (_indexRefInResult[i] == -1)
                {
                    str = _refResultFileList[i].Replace(_refResultPath, _resultPath);
                    for (int j = 0; j < _resultFileList.Count; j++)
                    {
                        if (str == _resultFileList[j])
                        {
                            _indexRefInResult[i] = j;
                        }
                    }
                }
                if (_indexRefInResult[i] == -1)
                {
                    nMissing++;
                    _notFoundFilesInResult += str + Environment.NewLine;
                }
            }
            _countOfResultMissingFile = nMissing;
        }

        private List<string> _ignoredCompFileExtNameList = new List<string>();
        /// <summary>
        /// The ignored files in result compare
        /// </summary>
        public List<string> IgnoredCompFileExtNameList
        {
            get { return _ignoredCompFileExtNameList; }
            set { _ignoredCompFileExtNameList = value; }
        }

        private List<string> _ignoredInpFileList = new List<string>();
        /// <summary>
        /// The ignored input file in running min3p, e.g., root.dat
        /// </summary>
        public List<string> IgnoredInpFileList
        {
            get { return _ignoredInpFileList; }
            set { _ignoredInpFileList = value; }
        }

        private List<SpecialFileIgnoreLines> _specialFileIgnoreLinesList = new List<SpecialFileIgnoreLines>();

        /// <summary>
        /// Ignore lines in special file
        /// </summary>
        public List<SpecialFileIgnoreLines> SpecialFileIgnoreLinesList
        {
            get { return _specialFileIgnoreLinesList; }
            set { _specialFileIgnoreLinesList = value; }
        }

        private List<string> _valueCompFileExtNameList = new List<string>();
        /// <summary>
        /// Files for value compare
        /// </summary>
        public List<string> ValueCompFileExtnameList
        {
            get { return _valueCompFileExtNameList; }
            set { _valueCompFileExtNameList = value; }
        }

        private bool _bMixCompare = false;
        /// <summary>
        /// Mix text and value compare
        /// </summary>
        public bool BMixCompare
        {
            get { return _bMixCompare; }
            set { _bMixCompare = value; }
        }

        private double _mixCompErrorTolerance = 0.0;
        /// <summary>
        /// Error tolerance
        /// </summary>
        public double MixCompErrorTolerance
        {
            get { return _mixCompErrorTolerance; }
            set { _mixCompErrorTolerance = value; }
        }

        private double _mixCompErrorToleranceRel = 0.0;
        /// <summary>
        /// Error tolerance
        /// </summary>
        public double MixCompErrorToleranceRel
        {
            get { return _mixCompErrorToleranceRel; }
            set { _mixCompErrorToleranceRel = value; }
        }

        private SpecialFileIgnoreLines getSpecialFileWithExtName(string strExt)
        {
            for (int i = 0; i < _specialFileIgnoreLinesList.Count; i++)
            {
                if (_specialFileIgnoreLinesList[i].ExtensionName == strExt)
                {
                    return _specialFileIgnoreLinesList[i];
                }
            }
            return null;
        }

        /// <summary>
        /// value compare for text
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        private bool mixValueTextCompareSame(string str1, string str2)
        {
            string[] sep = new[] { " " };
            string[] words1 = str1.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            string[] words2 = str2.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            int n1 = words1.Length;
            int n2 = words2.Length;

            if (n1 != n2)
                return false;

            string strVal1, strVal2;
            double val1, val2, val3;
            bool isNum1, isNum2;
            

            for (int i = 0; i < n1; i++)
            {
                strVal1 = makeNumericFormat(words1[i]);
                strVal2 = makeNumericFormat(words2[i]);

                isNum1 = double.TryParse(strVal1, out val1);
                isNum2 = double.TryParse(strVal2, out val2);

                if (isNum1 != isNum2)
                    return false;
                else 
                {
                    if (isNum1 == true)
                    {
                        if (val1 != val2)
                        {
                            // check relative error first
                            val3 = Math.Abs(val1);
                            if (Math.Abs(val2) > val3)
                            {
                                val3 = Math.Abs(val2);
                            }

                            if (val3 != 0.0e0)
                            {
                                if (Math.Abs((val1 - val2) / val3) > _mixCompErrorToleranceRel)
                                {
                                    if (Math.Abs(val1 - val2) > _mixCompErrorTolerance)
                                        return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (words1[i] != words2[i])
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Convert string to numeric format
        /// e.g, 0.234567-200, 0.234567d-200 should be converted to 0.234567e-200 first.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string makeNumericFormat(string str)
        {
            str = str.Replace("D", "E");
            str = str.Replace("d", "E");

            if (str.LastIndexOf("-") > 0)
            {
                if (str.LastIndexOf("E-", StringComparison.OrdinalIgnoreCase) < 0)
                    str = str.Replace("-", "E-");
            }

            if (str.LastIndexOf("+") > 0)
            {
                if (str.LastIndexOf("E+", StringComparison.OrdinalIgnoreCase) < 0)
                    str = str.Replace("+", "E+");
            }

            return str;
        }

        /// <summary>
        /// Text difference compare
        /// </summary>
        /// <param name="iResultFile"></param>
        private void textDiff(int iResultFile)
        {
            
            string sFile = _refResultFileList[_indexResultInRef[iResultFile]];
            string dFile = _resultFileList[iResultFile];
            _log.Write("Runing result compare for:" + Environment.NewLine + sFile + Environment.NewLine + dFile);
            try
            {
                string str1 = string.Empty;
                string str2 = string.Empty;

                DiffList_TextFile sLF = null;
                DiffList_TextFile dLF = null;

                sLF = new DiffList_TextFile(sFile);
                dLF = new DiffList_TextFile(dFile);

                double time = 0;
                DiffEngine de = new DiffEngine();

                _log.Write("Processing difference, please wait... " );

                //time = de.ProcessDiff(sLF, dLF, _level);
                time = de.ProcessDiff(sLF, dLF, _level,_lineCompMode);

                _log.Write("Generating difference report, please wait... ");

                ArrayList rep = de.DiffReport();
                //Results dlg = new Results(sLF, dLF, rep, time);

                _log.Write("Collectting difference report, please wait... ");

                int i, j, ndif;

                SpecialFileIgnoreLines ignoreLines = getSpecialFileWithExtName(Path.GetExtension(sFile));

                if (ignoreLines == null)
                {
                    foreach (DiffResultSpan drs in rep)
                    {
                        ndif = Math.Min(drs.Length, _difCollectNumber);

                        switch (drs.Status)
                        {
                            case DiffResultSpanStatus.DeleteSource:                                
                                for (i = 0; i < ndif; i++)
                                {
                                    //if (bTest)
                                    //    _log.Write("Delete source index: " + i.ToString());

                                    str1 = ((TextLine)sLF.GetByIndex(drs.SourceIndex + i)).Line;
                                    _difInResultFileList[iResultFile] += "CHANGE DELETED: " + str1 + Environment.NewLine;
                                }
                                if (drs.Length > _difCollectNumber)
                                    _difInResultFileList[iResultFile] += "The possible difference by text comparing from " + Convert.ToString(ndif + 1) +
                                        " to " + drs.Length.ToString() + " is not displayed." + Environment.NewLine;
                                break;
                            case DiffResultSpanStatus.NoChange:
                                break;
                            case DiffResultSpanStatus.AddDestination:
                                for (i = 0; i < ndif; i++)
                                {
                                    //if (bTest)
                                    //    _log.Write("Add destination index: " + i.ToString());

                                    str2 = ((TextLine)dLF.GetByIndex(drs.DestIndex + i)).Line;
                                    _difInResultFileList[iResultFile] += "CHANGE ADDED: " + str2 + Environment.NewLine;
                                }
                                if (drs.Length > _difCollectNumber)
                                    _difInResultFileList[iResultFile] += "The possible difference by text comparing from " + Convert.ToString(ndif + 1) +
                                        " to " + drs.Length.ToString() + " is not displayed." + Environment.NewLine;
                                break;
                            case DiffResultSpanStatus.Replace:
                                j = 0;
                                for (i = 0; i < drs.Length; i++)
                                {
                                    //if (bTest)
                                    //    _log.Write("Replace index: " + i.ToString());

                                    str1 = ((TextLine)sLF.GetByIndex(drs.SourceIndex + i)).Line;
                                    str2 = ((TextLine)dLF.GetByIndex(drs.DestIndex + i)).Line;
                                    if (_bMixCompare)
                                    {
                                        if (!mixValueTextCompareSame(str1, str2))
                                        {
                                            _difInResultFileList[iResultFile] += "Line: " + Convert.ToString(drs.SourceIndex + i + 1) + ", REFERENCE: " + str1 + Environment.NewLine;
                                            _difInResultFileList[iResultFile] += "Line: " + Convert.ToString(drs.DestIndex + i + 1)   + ", CURRENT:   " + str2 + Environment.NewLine;
                                            j++;
                                        }
                                    }
                                    else
                                    {
                                        _difInResultFileList[iResultFile] += "Line: " + Convert.ToString(drs.SourceIndex + i + 1) + ", REFERENCE: " + str1 + Environment.NewLine;
                                        _difInResultFileList[iResultFile] += "Line: " + Convert.ToString(drs.DestIndex + i + 1)   + ", CURRENT:   " + str2 + Environment.NewLine;
                                        j++;
                                    }
                                    if (j >= ndif)
                                        break;
                                }
                                if (j >= ndif && drs.Length > _difCollectNumber)
                                    _difInResultFileList[iResultFile] += "The possible difference by text comparing from " + Convert.ToString(ndif + 1) +
                                        " to " + drs.Length.ToString() + " is not displayed." + Environment.NewLine;
                                break;
                        }
                    }
                }
                else
                {
                    foreach (DiffResultSpan drs in rep)
                    {
                        ndif = Math.Min(drs.Length, _difCollectNumber);

                        switch (drs.Status)
                        {
                            case DiffResultSpanStatus.DeleteSource:
                                for (i = 0; i < ndif; i++)
                                {
                                    //if (bTest)
                                    //    _log.Write("Delete source index: " + i.ToString());
                                    
                                    str1 = ((TextLine)sLF.GetByIndex(drs.SourceIndex + i)).Line;
                                    if(!ignoreLines.MatchIgnoredStart(str1))
                                        _difInResultFileList[iResultFile] += "CHANGE DELETED: " + str1 + Environment.NewLine;
                                }
                                if (drs.Length > _difCollectNumber)
                                    _difInResultFileList[iResultFile] += "The possible difference by text comparing from " + Convert.ToString(ndif + 1) +
                                        " to " + drs.Length.ToString() + " is not displayed." + Environment.NewLine;
                                break;
                            case DiffResultSpanStatus.NoChange:
                                break;
                            case DiffResultSpanStatus.AddDestination:
                                for (i = 0; i < ndif; i++)
                                {
                                    //if (bTest)
                                    //    _log.Write("Add destination index: " + i.ToString());

                                    str2 = ((TextLine)dLF.GetByIndex(drs.DestIndex + i)).Line;
                                    if (!ignoreLines.MatchIgnoredStart(str2))
                                        _difInResultFileList[iResultFile] += "CHANGE ADDED: " + str2 + Environment.NewLine;
                                }
                                if (drs.Length > _difCollectNumber)
                                    _difInResultFileList[iResultFile] += "The possible difference by text comparing from " + Convert.ToString(ndif + 1) +
                                        " to " + drs.Length.ToString() + " is not displayed." + Environment.NewLine;
                                break;
                            case DiffResultSpanStatus.Replace:
                                j = 0;
                                for (i = 0; i < drs.Length; i++)
                                {
                                    //if (bTest)
                                    //    _log.Write("Replace index: " + i.ToString());

                                    str1 = ((TextLine)sLF.GetByIndex(drs.SourceIndex + i)).Line;
                                    str2 = ((TextLine)dLF.GetByIndex(drs.DestIndex + i)).Line;
                                    if (!(ignoreLines.MatchIgnoredStart(str1) && ignoreLines.MatchIgnoredStart(str2)))
                                    {
                                        if (_bMixCompare)
                                        {
                                            if (!mixValueTextCompareSame(str1, str2))
                                            {
                                                _difInResultFileList[iResultFile] += "Line: " + Convert.ToString(drs.SourceIndex + i + 1) + ", REFERENCE: " + str1 + Environment.NewLine;
                                                _difInResultFileList[iResultFile] += "Line: " + Convert.ToString(drs.DestIndex + i + 1)   + ", CURRENT:   " + str2 + Environment.NewLine;
                                                j++;
                                            }
                                        }
                                        else
                                        {
                                            _difInResultFileList[iResultFile] += "Line: " + Convert.ToString(drs.SourceIndex + i + 1) + ", REFERENCE: " + str1 + Environment.NewLine;
                                            _difInResultFileList[iResultFile] += "Line: " + Convert.ToString(drs.DestIndex + i + 1)   + ", CURRENT:   " + str2 + Environment.NewLine;
                                            j++;
                                        }
                                    }
                                    if (j >= ndif)
                                        break;
                                }
                                if (j >= ndif && drs.Length > _difCollectNumber)
                                    _difInResultFileList[iResultFile] += "The possible difference by text comparing from " + Convert.ToString(ndif + 1) +
                                        " to " + drs.Length.ToString() + " is not displayed. Please set " + Environment.NewLine;
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Write("Exception detected in compare file " + Path.GetFileName(sFile) + 
                    "." + Environment.NewLine  + "Error Information: " +
                    ex.ToString() + Environment.NewLine + "Is this a binary file?");
                return;
            }
        }

        /// <summary>
        /// Compare all the results
        /// </summary>
        public void RunCompareResult()
        {
            for (int i = 0; i < _indexResultInRef.Count; i++)
            {
                if (_indexResultInRef[i] != -1)
                    textDiff(i);
            }
        }

    }
}
