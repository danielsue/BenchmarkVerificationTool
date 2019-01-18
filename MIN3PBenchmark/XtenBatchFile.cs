using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MIN3PBenchmark
{
    class XtenBatchFile
    {
        bool _isOpen = false;
        System.IO.StreamWriter _swBat;
        private string _filePath = string.Empty;
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public XtenBatchFile(string filePath)
        {
            _filePath = filePath;
            _swBat = new System.IO.StreamWriter(filePath);
            _isOpen = true;
        }

        public void Open()
        {
            if (!_isOpen)
            {
                _swBat = new System.IO.StreamWriter(_filePath);
                _isOpen = true;
            }
        }

        public void Write(string str)
        {
            if(_isOpen )
                _swBat.WriteLine(str);
        }

        public void Close()
        {
            if (_isOpen)
            {
                _swBat.Close();
                _isOpen = false;
            }
        }

        public void Delete()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
                _isOpen = false;
            }
        }
    }
}
