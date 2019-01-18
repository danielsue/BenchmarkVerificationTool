using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MIN3PBenchmark
{
    class FileUtility
    {

        private string _filePath = string.Empty;

        public string FilePath
        {
            get { return _filePath; }
        }

        public FileUtility(string filePath)
		{
			//
			// TODO:
			//
            if (filePath.Trim() == string.Empty)
                _filePath = "MIN3PBenchmark.inp";
            else
                _filePath = filePath;
		}
        static SemaphoreSlim _sem = new SemaphoreSlim(1);
        static CountdownEvent _countdown; 

        private int _numberOfThreads = 1;
        public int NumberOfThreads
        {
            get { return _numberOfThreads; }
        }
        private void setNumberOfThreads(int n)
        {
            _numberOfThreads = n;
            _sem = new SemaphoreSlim(n);
        }

		private StreamReader _objReader;

        private bool _bRunMin3pSimulation = false;

        public bool BRunMin3pSimulation
        {
            get { return _bRunMin3pSimulation; }
        }

        private bool _bRunResultCompare = false;

        public bool BRunResultCompare
        {
            get { return _bRunResultCompare; }
        }

        private FileLog _log;
        private XtenBatchFile _xtenBat;
        private FileResultDif _resultDif;

		private string _strBuffer = string.Empty;
		public string StrBuffer
		{
			get { return _strBuffer; }
		}

        private List<string> _min3pInputFileList = new List<string>();
		/// <summary>
		/// Input file list of min3p
		/// </summary>
        public List<string> Min3pInputFileList
		{
			get{return _min3pInputFileList;}
		}

        private string _exeMin3PProgramPath = string.Empty;
        /// <summary>
        /// "min3p" executable program path
        /// </summary>
        public string ExeMin3PProgramPath
        {
            get { return _exeMin3PProgramPath; }
        }

        private string _exeMin3PProgramFullPath = string.Empty;
        /// <summary>
        /// "min3p" executable program path
        /// </summary>
        public string ExeMin3PProgramFullPath
        {
            get { return _exeMin3PProgramFullPath; }
        }

        private string _exeXtenProgramPath = string.Empty;
        /// <summary>
        /// "xten" executable program path
        /// </summary>
        public string ExeXtenProgramPath
        {
            get { return _exeXtenProgramPath; }
        }

        private string _exeXtenProgramFullPath = string.Empty;
        /// <summary>
        /// "xten" executable program path
        /// </summary>
        public string ExeXtenProgramFullPath
        {
            get { return _exeXtenProgramFullPath; }
        }

        private string _argParallelArguments = string.Empty;
        /// <summary>
        /// Parallel arguments
        ///! Parallel arguments.
        ///! If the program runs as openmp parallel version,
        ///! the argements are similar as 
        ///! "-numofthreads_global 8 -output_runtime -input_file"
        ///! If the program runs as MPI parallel version,
        ///! please change "MIN3P EXECUTABLE PROGRAM PATH" to the mpiexec path
        ///! and the arguments are similar as
        ///! "-n 8 path\min3p_thcm.exe -output_runtime -input_file"
        ///PARALLEL ARGUMENTS
        ///-numofthreads_global 8 -output_runtime -input_file
        /// </summary>
        public string ArgParallelArguments
        {
            get { return _argParallelArguments; }
        }

        private string _destinationFolder = string.Empty;
        /// <summary>
        /// Destination folder, this folder contains all the bechmark examples
        /// </summary>
        public string DestinationFolder
        {
            get { return _destinationFolder; }
            set { _destinationFolder = value; }
        }

        private string _destinationFolderFullPath = string.Empty;
        /// <summary>
        /// Destination folder, this folder contains all the bechmark examples
        /// </summary>
        public string DestinationFolderFullPath
        {
            get { return _destinationFolderFullPath; }
            set { _destinationFolderFullPath = value; }
        }

        private string _sourceFolder = string.Empty;
        /// <summary>
        /// Source folder, this folder contains all the refenence bechmark examples
        /// </summary>
        public string SourceFolder
        {
            get { return _sourceFolder; }
            set { _sourceFolder = value; }
        }

        private string _sourceFolderFullPath = string.Empty;
        /// <summary>
        /// Source folder, this folder contains all the refenence bechmark examples
        /// </summary>
        public string SourceFolderFullPath
        {
            get { return _sourceFolderFullPath; }
            set { _sourceFolderFullPath = value; }
        }

        private string _resultDifFolder = string.Empty;
        /// <summary>
        /// Folder to store the result compare difference
        /// </summary>
        public string ResultDifFolder
        {
            get { return _resultDifFolder; }
            set { _resultDifFolder = value; }
        }

        private string _resultDifFullPath = string.Empty;

        /// <summary>
        /// Folder to store the result compare difference
        /// </summary>
        public string ResultDifFullPath
        {
            get { return _resultDifFullPath; }
            set { _resultDifFullPath = value; }
        }


        private bool _bExportDetailDifReport = false;

        public bool BExportDetailDifReport
        {
            get { return _bExportDetailDifReport; }
        }
        

        private bool _bIgnoreMFilesInRefResultFolder = false;
        public bool BIgnoreMFilesInRefResultFolder
        {
            get { return _bIgnoreMFilesInRefResultFolder; }
        }

        private bool _bIgnoreMFilesInResultFolder = false;
        public bool BIgnoreMFilesInResultFolder
        {
            get { return _bIgnoreMFilesInResultFolder; }
        }

        private string _extensionFileNameMin3p = ".dat";
        /// <summary>
        /// The extension file name for min3p input file, e.g, ".dat"
        /// </summary>
        public string ExtensionFileNameMin3P
        {
            get { return _extensionFileNameMin3p; }
        }

        private ResultCompare _min3pResultCompare = new ResultCompare();

        public ResultCompare Min3PResultCompare
        {
            get { return _min3pResultCompare; }
            set { _min3pResultCompare = value; }
        }

		/// <summary>
		/// Open file
		/// </summary>
		/// <param name="filePath"></param>
		public bool OpenFile()
		{

            try
            {
                bool info = System.IO.File.Exists(_filePath);
                if (info)
                {
                    _log = new FileLog(this._filePath.Replace(".inp", ".log"));
                    _min3pResultCompare.Log = _log;
                    _objReader = new StreamReader(_filePath, Encoding.GetEncoding("gb2312"));
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                //CloseFile();
                return false;
            }

		}

		/// <summary>
		/// Read next command line
		/// </summary>
		/// <returns></returns>
		public bool ReadNextCommandLine()
		{
            try
            {
                while ((_strBuffer = _objReader.ReadLine()) != null)
                {
                    if (_strBuffer.StartsWith("!") || _strBuffer.StartsWith("！"))
                        continue;
                    if (_strBuffer.Trim() == string.Empty)
                        continue;
                    _strBuffer = _strBuffer.Trim().ToLower();
                    _log.Write(_strBuffer);
                    return true;
                }
                return false;
            }
            catch
            {
                //CloseFile();
                return false;
            }
		}

        /// <summary>
        /// Read next data line
        /// </summary>
        /// <returns></returns>
        public bool ReadNextDataLine()
        {
            try
            {
                while ((_strBuffer = _objReader.ReadLine()) != null)
                {
                    if (_strBuffer.StartsWith("!") || _strBuffer.StartsWith("！"))
                        continue;
                    if (_strBuffer.Trim() == string.Empty)
                        continue;
                    _log.Write(_strBuffer);
                    return true;
                }
                return false;
            }
            catch
            {
                //CloseFile();
                return false;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		public void CloseInputFile()
		{
			_objReader.Close();
			_strBuffer = string.Empty;
		}

		public bool ReadInputParameters()
		{
            try
            {
                int n = 0;
                while (ReadNextCommandLine())
                {
                    switch (_strBuffer)
                    {
                        case "number of threads":
                            if (ReadNextDataLine())
                            {
                                n = Convert.ToInt32(_strBuffer);
                                setNumberOfThreads(n);
                            }
                            break;
                        case "run min3p simulation":
                            _bRunMin3pSimulation = true;
                            _xtenBat = new XtenBatchFile(this._filePath.Replace(".inp", ".bat"));
                            break;
                        case "run result compare":
                            _bRunResultCompare = true;
                            break;
                        case "min3p executable program path":
                            if (ReadNextDataLine())
                            {
                                _exeMin3PProgramPath = _strBuffer;
                                _exeMin3PProgramFullPath = Path.GetFullPath(_strBuffer);
                                _log.Write("full path of min3p executable program:" + Environment.NewLine + _exeMin3PProgramFullPath);
                            }
                            break;

                        case "parallel arguments":
                            if (ReadNextDataLine())
                            {
                                _argParallelArguments = _strBuffer;
                                _log.Write("parallel arguments:" + Environment.NewLine + _argParallelArguments);
                            }
                            break;

                        case "xtens executable program path":
                            if (ReadNextDataLine())
                            {
                                _exeXtenProgramPath = _strBuffer;
                                _exeXtenProgramFullPath = Path.GetFullPath(_strBuffer);
                                _log.Write("full path of xtens executable program:" + Environment.NewLine + _exeXtenProgramFullPath);
                            }
                            break;
                        case "destination folder":
                            if (ReadNextDataLine())
                            {
                                _destinationFolder = _strBuffer;
                                _destinationFolderFullPath = Path.GetFullPath(_strBuffer);
                                _log.Write("full path of destination folder:" + Environment.NewLine + _destinationFolderFullPath);
                            }
                            break;
                        case "source folder":
                            if (ReadNextDataLine())
                            {
                                _sourceFolder = _strBuffer;
                                _sourceFolderFullPath = Path.GetFullPath(_strBuffer);
                                _log.Write("full path of source folder:" + Environment.NewLine + _sourceFolderFullPath);
                            }
                            break;
                        case "result compare store difference folder":
                            if (ReadNextDataLine())
                            {
                                _resultDifFolder = _strBuffer;
                                _resultDifFullPath = Path.GetFullPath(_strBuffer);
                                _resultDif = new FileResultDif(Path.Combine(_resultDifFullPath, "dif_summary.txt"));
                                _log.Write("full path of difference store folder:" + Environment.NewLine + _resultDifFullPath);
                            }
                            break;
                        case "export result difference detail report":
                            _bExportDetailDifReport = true;
                            break;
                        case "extension file name of min3p input":
                            if (ReadNextDataLine())
                                _extensionFileNameMin3p = _strBuffer;
                            break;
                        case "ignore files of min3p input":
                            if (ReadNextDataLine())
                            {
                                n = Convert.ToInt32(_strBuffer);
                                for (int i = 0; i < n; i++)
                                {
                                    if (ReadNextDataLine())
                                        _min3pResultCompare.IgnoredInpFileList.Add(_strBuffer);
                                }
                            }
                            break;
                        case "ignore missing files in result folder":
                            _bIgnoreMFilesInResultFolder = true;
                            break;
                        case "ignore missing files in referenced result folder":
                            _bIgnoreMFilesInRefResultFolder = true;
                            break;
                        case "ignore files in result compare":
                            if (ReadNextDataLine())
                            {
                                n = Convert.ToInt32(_strBuffer);
                                for (int i = 0; i < n; i++)
                                {
                                    if (ReadNextDataLine())
                                        _min3pResultCompare.IgnoredCompFileExtNameList.Add(_strBuffer);
                                }
                            }
                            break;
                        case "ignore lines start in special files":
                            if (ReadNextDataLine())
                            {
                                _min3pResultCompare.SpecialFileIgnoreLinesList.Add(new SpecialFileIgnoreLines(_strBuffer));

                                if (ReadNextDataLine())
                                {
                                    n = Convert.ToInt32(_strBuffer);
                                    int j = _min3pResultCompare.SpecialFileIgnoreLinesList.Count - 1;
                                    for (int i = 0; i < n; i++)
                                    {
                                        if (ReadNextDataLine())
                                            _min3pResultCompare.SpecialFileIgnoreLinesList[j].IgnoredLineStartString.Add(_strBuffer);
                                    }
                                }
                            }
                            break;
                        case "mix value text compare tolerance":
                            if (ReadNextDataLine())
                            {
                                _min3pResultCompare.BMixCompare = true;
                                _min3pResultCompare.MixCompErrorTolerance = Convert.ToDouble(_strBuffer);
                            }
                            break;
                        case "mix value text compare relative tolerance":
                            if (ReadNextDataLine())
                            {
                                _min3pResultCompare.BMixCompare = true;
                                _min3pResultCompare.MixCompErrorToleranceRel = Convert.ToDouble(_strBuffer);
                            }
                            break;
                        case "global best match compare mode":
                            _min3pResultCompare.LineCompMode = DifferenceEngine.LineCompareMode.GlobalLineCompare;
                            break;
                        case "same line compare mode":
                            _min3pResultCompare.LineCompMode = DifferenceEngine.LineCompareMode.SameLineCompare;
                            break;
                        case "difference collection mode":
                            if (ReadNextDataLine())
                            {
                                n = Convert.ToInt32(_strBuffer);
                                if (n > 0)
                                    _min3pResultCompare.DifCollectNumber = n;
                                else if (n == 0)
                                    _min3pResultCompare.DifCollectNumber = int.MaxValue;
                            }
                            break;
                        default:
                            Console.WriteLine("Unknow input parameter: " + _strBuffer);
                            _log.Write("Unknow input parameter: " + Environment.NewLine + _strBuffer);
                            _log.Close();
                            return false;
                    }
                }

                getMin3PInputFiles();

                return true;
            }
            catch (Exception ex)
            {
                _log.Write("Error: exception during reading input parameters" + Environment.NewLine + ex.ToString());
                //CloseFile();
                return false;
            }
		}

        /// <summary>
        /// Get all the min3p input files
        /// </summary>
        private void getMin3PInputFiles()
        {
            try
            {
                _log.Write("Get input files for min3p from directory");
                string[] tempInpFiles = Directory.GetFiles(_destinationFolder, "*" + _extensionFileNameMin3p, SearchOption.AllDirectories);
                for (int i = 0; i < tempInpFiles.Length; i++)
                {
                    if (!matchIgnoredFile(tempInpFiles[i], _min3pResultCompare.IgnoredInpFileList))
                    {
                        _min3pInputFileList.Add(tempInpFiles[i]);
                        _log.Write(tempInpFiles[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Write("Error: exception during get MIN3P input files" + Environment.NewLine + ex.ToString());
                //CloseFile();
            }
        }

        private bool matchIgnoredFile(string strPath, List<string> strList)
        {
            for (int i = 0; i < strList.Count; i++)
            {
                if (strPath.EndsWith(strList[i]))
                    return true;                
            }
            return false;
        }

        private void runMin3p(int i)
        {
            _log.Write("==================== run min3p simulation " + i.ToString() + "====================");
            try
            {
                if (_bRunMin3pSimulation)
                {
                    //----------Old, run min3p through xten.exe. Will cause access violation in multi-threaded.
                    //_log.Write("Create bat file for xten.exe:" + Environment.NewLine + _min3pInputFileList[i]);
                    //string strXtenContent = generateXtenBat(i);
                    //if (strXtenContent == string.Empty)
                    //{
                    //    _log.Write("Error: empty xten content for " + _min3pInputFileList[i]);
                    //    return;
                    //}
                    //try
                    //{
                    //    _xtenBat.Open();
                    //    _xtenBat.Write(strXtenContent);
                    //    _xtenBat.Close();
                    //}
                    //catch (Exception ex)
                    //{
                    //    _log.Write("Error: exception during creating xten bat file for " + _min3pInputFileList[i] + Environment.NewLine + ex.ToString());
                    //    return; ;
                    //}                                            

                    //_log.Write("Runing MIN3P simulation for:" + Environment.NewLine + _min3pInputFileList[i]);

                    //if (startProcess(_xtenBat.FilePath, string.Empty))
                    //    _log.Write("Finished running MIN3P simulation:" + Environment.NewLine + _min3pInputFileList[i]);
                    //else
                    //    _log.Write("Failed running MIN3P simulation:" + Environment.NewLine + _min3pInputFileList[i]);
                    //---------------
                    _log.Write("Start process: " + _exeMin3PProgramPath + " " + _min3pInputFileList[i].Substring(0, _min3pInputFileList[i].Length - 4));


                    if (startProcess(_exeMin3PProgramPath, _min3pInputFileList[i].Substring(0, _min3pInputFileList[i].Length - 4), _argParallelArguments))
                        _log.Write("Finished running MIN3P simulation:" + Environment.NewLine + _min3pInputFileList[i]);
                    else
                        _log.Write("Failed running MIN3P simulation:" + Environment.NewLine + _min3pInputFileList[i]);

                }
            }
            catch (Exception ex)
            {
                _log.Write("Error: exception during min3p simulation running  for " + _min3pInputFileList[i] + Environment.NewLine + ex.ToString());
                return;
            }
        }

        private void runResultCompare(int i)
        {
            _log.Write("==================== run result compare " + i.ToString() + "====================");
            try
            {
                if (_bRunResultCompare)
                {
                    _log.Write("Runing result compare for " + Environment.NewLine + _min3pInputFileList[i]);
                    string resultFolder = new FileInfo(_min3pInputFileList[i]).Directory.FullName;
                    _log.Write("Result folder:" + Environment.NewLine + resultFolder);
                    string referencedResultFolder = resultFolder.Replace(DestinationFolderFullPath, SourceFolderFullPath);
                    _log.Write("Referenced result folder:" + Environment.NewLine + referencedResultFolder);

                    //Initialize result compare
                    _log.Write("Initialize result compare for " + Environment.NewLine + _min3pInputFileList[i]);
                    _min3pResultCompare.InitialCompare(resultFolder, referencedResultFolder);

                    _log.Write("The following files in the result folder will be compared");
                    for (int j = 0; j < _min3pResultCompare.ResultFileList.Count; j++)
                    {
                        _log.Write(j.ToString() + " " + _min3pResultCompare.ResultFileList[j]);
                    }

                    _log.Write("The following files in the referenced result folder will be compared");
                    for (int j = 0; j < _min3pResultCompare.RefResultFileList.Count; j++)
                    {
                        _log.Write(j.ToString() + " " + _min3pResultCompare.RefResultFileList[j]);
                    }
                    //Find the file index between result files and referenced result files
                    //Missing files are detected
                    _min3pResultCompare.FindResultFileIndex();
                    _log.Write("Find the file index between result files and referenced result files");
                    _log.Write("Count of missing files in result:" + _min3pResultCompare.CountOfResultMissingFile.ToString());
                    _log.Write("Count of missing files in referenced result:" + _min3pResultCompare.CountOfRefResultMissingFile.ToString());

                    //Run result compare
                    _log.Write("Runing result compare for:" + Environment.NewLine + _min3pInputFileList[i] + Environment.NewLine + "Please wait ...");
                    _min3pResultCompare.RunCompareResult();
                    _log.Write("Writing result compare for:" + Environment.NewLine + _min3pInputFileList[i] + Environment.NewLine + "Please wait ...");
                    //for (int j = 0; j < _min3pResultCompare.ResultFileList.Count; j++)
                    //{
                    //    _log.Write("Difference in file: " + Path.GetFileName(_min3pResultCompare.ResultFileList[j]) + Environment.NewLine);
                    //    _log.Write(_min3pResultCompare.DifInResultFileList[j] + Environment.NewLine);
                    //}
                    //Write result compare difference
                    writeResultDifSummaryToFile();
                    if (_bExportDetailDifReport)
                        writeResultDifDetailToFile();
                }
            }
            catch (Exception ex)
            {
                _log.Write("Error: exception during min3p result compare for " +_min3pInputFileList[i] + Environment.NewLine + ex.ToString());
            }
        }

        private void runMin3P(object id)
        {
            int i = (int)id;
            _log.Write("run min3p and result compare using multi-threads for " + _min3pInputFileList[i] + Environment.NewLine );
            _sem.Wait();
            runMin3p(i);
            //runResultCompare(i);
            _sem.Release();
            _countdown.Signal();
        }


        public void Run()
        {
            _countdown = new CountdownEvent(_min3pInputFileList.Count);
            for (int i = 0; i < _min3pInputFileList.Count; i++)
            {
                //runMin3p(i);
                new Thread(runMin3P).Start(i);
            }

            _countdown.Wait();

            for (int i = 0; i < _min3pInputFileList.Count; i++)
                runResultCompare(i);

            //if (_bRunMin3pSimulation)
            //    _xtenBat.Delete();
        }

        /// <summary>
        /// Write result of differences to file
        /// </summary>
        private void writeResultDifSummaryToFile()
        {
            try
            {
                _resultDif.Create();

                System.IO.StreamWriter sw = _resultDif.SW;

                if (!BIgnoreMFilesInRefResultFolder &&  _min3pResultCompare.CountOfRefResultMissingFile > 0)
                {
                    sw.WriteLine("The following files are new added in the current result");
                    sw.WriteLine(_min3pResultCompare.NewAddedFilesInResult);
                }
                if (!BIgnoreMFilesInResultFolder && _min3pResultCompare.CountOfResultMissingFile > 0)
                {
                    sw.WriteLine("The following files are not found in the current result");
                    sw.WriteLine(_min3pResultCompare.NotFoundFilesInResult);
                }                
                for (int i = 0; i < _min3pResultCompare.DifInResultFileList.Count; i++)
                {
                    if (_min3pResultCompare.DifInResultFileList[i] != string.Empty)
                    {
                        sw.WriteLine("Different in file:");
                        sw.WriteLine(_min3pResultCompare.ResultFileList[i]);
                    }
                }
                _log.Write("Difference summary is writen in to file:" + Environment.NewLine + _resultDif.DifFilePath);
            }
            catch (Exception ex)
            {
                _log.Write("Error: exception during writing result difference summary to file" + Environment.NewLine + ex.ToString());
                //CloseFile();
            }
        }

        /// <summary>
        /// Write result of differences to file
        /// </summary>
        private void writeResultDifDetailToFile()
        {
            FileResultDif detailDif = new FileResultDif();
            
            try
            {
                for (int i = 0; i < _min3pResultCompare.DifInResultFileList.Count; i++)
                {
                    
                    if (_min3pResultCompare.DifInResultFileList[i] != string.Empty)
                    {
                        string strDiffPath = Path.GetFullPath(_min3pResultCompare.ResultFileList[i]);
                        strDiffPath = strDiffPath.Replace(_destinationFolderFullPath, _resultDifFullPath);
                        detailDif = new FileResultDif(strDiffPath);
                        detailDif.Create();

                        detailDif.Write(_min3pResultCompare.DifInResultFileList[i]);
                        detailDif.Close();
                        _log.Write("Difference detail is writen in to file:" + Environment.NewLine + strDiffPath);
                    }
                }                
            }
            catch (Exception ex)
            {
                _log.Write("Error: exception during writing detail difference to file" + Environment.NewLine + ex.ToString());
                detailDif.Close();
                //CloseFile();
            }
        }

        private string generateXtenBat(int i)
        {
            try
            {
                string strBatCmd = string.Empty;

                strBatCmd = "cd " + Directory.GetParent(Path.GetFullPath(_min3pInputFileList[i])).ToString() + Environment.NewLine +
                    "FOR  %%D in (" + Path.GetFileName(_min3pInputFileList[i]) + ") DO (" + Environment.NewLine +
                    "echo %%D | " + Path.GetFullPath(_exeXtenProgramPath) + " | " + Path.GetFullPath(_exeMin3PProgramPath) + Environment.NewLine + ")";
                return strBatCmd;
            }
            catch (Exception ex)
            {
                _log.Write("Error: exception during generating xten bat file" + Environment.NewLine + ex.ToString());
                //CloseFile();
                return string.Empty;
            }
        }
        
	
        /// <summary>
        /// You should modify the input function getstrqq in min3p if you want to use startProcess.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool startProcess(string file, string args, string parallel_args)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo ps = new System.Diagnostics.ProcessStartInfo();
                ps.WorkingDirectory = Path.GetDirectoryName(args);
                ps.FileName = Path.GetFullPath(file);
                if (parallel_args.Trim() != "")
                {
                    ps.Arguments = parallel_args + " " + Path.GetFileName(args);  
                }
                else
                {
                    ps.Arguments = Path.GetFileName(args);
                }
                _log.Write("Process working dir: " + ps.WorkingDirectory + Environment.NewLine + "Process file path: " + ps.FileName + Environment.NewLine + "Process arguments: " + ps.Arguments);
                ps.UseShellExecute = false;
                ps.CreateNoWindow = true;
                Process p = Process.Start(ps);
                p.WaitForExit();
                p.Close();
                return true;
            }
            catch (Exception e0)
            {
                _log.Write("Run error:" + e0.Message + System.Environment.NewLine + "Input File: " + file);
                //CloseFile();
                return false;                 
            }
        }

        public void CloseFile()
        {
            _log.Close();
            _resultDif.Close();
        }

    }
}
