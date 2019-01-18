using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace MIN3PBenchmark
{
    class FileLog
    {
        bool _isOpen = false;
        System.IO.StreamWriter _swLog;
        private string _filePath = string.Empty;

        public string FilePath
        {
            get { return _filePath; }
        }

        private object _lock = new object();

        public FileLog()
        { }

        public FileLog(string filePath)
        {
            _filePath = filePath;
            Create();
        }

        public void Create()
        {
            if (!_isOpen)
            {
                _swLog = new StreamWriter(_filePath);
                _isOpen = true;
            }
        }

        public void Open()
        {
            if (!_isOpen)
            {
                _swLog = File.AppendText(_filePath);
                _isOpen = true;
            }
        }

        public void Write(string str)
        {
            lock (_lock)
            {
                _swLog.WriteLine(str);
                Console.WriteLine(str);
            }
        }

        public void Close()
        {
            if (_isOpen)
            {
                _swLog.Close();
                _isOpen = false;
            }
        }
    }
}
