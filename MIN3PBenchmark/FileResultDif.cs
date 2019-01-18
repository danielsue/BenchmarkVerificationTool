using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MIN3PBenchmark
{
    class FileResultDif
    {
        public FileResultDif()
        { }

        public FileResultDif(string strPath)
        {

            _difFilePath = strPath;
        }        

        bool _isOpen = false;
        private StreamWriter _sw;
        public StreamWriter SW
        {
            get { return _sw; }
            set { _sw = value; }
        }
        private string _difFilePath = string.Empty;
        public string DifFilePath
        {
            get { return _difFilePath; }
            set { _difFilePath = value; }
        }

        public void Create()
        {
            if (!_isOpen)
            {
                string strFolder = new FileInfo(_difFilePath).Directory.FullName;
                if (!Directory.Exists(strFolder))
                    Directory.CreateDirectory(strFolder);

                _sw = new StreamWriter(_difFilePath);
                _isOpen = true;
            }
        }

        public void Open()
        {
            if (!_isOpen)
            {
                _sw = File.AppendText(_difFilePath);
                _isOpen = true;
            }
        }

        public void Write(string str)
        {
            _sw.WriteLine(str);
            //Console.WriteLine(str);
        }

        public void Close()
        {
            if (_isOpen)
            {
                _sw.Close();
                _isOpen = false;
            }
        }

    }    
}
