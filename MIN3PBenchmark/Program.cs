using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace MIN3PBenchmark
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //
            // TODO:
            //
            Console.Write(_ProInfo);

            FileUtility min3pBenchmarkInput;

            //Read input parameters

            while (true)
            {
                min3pBenchmarkInput = new FileUtility(Console.ReadLine());
                if (min3pBenchmarkInput.OpenFile())
                {
                    if (min3pBenchmarkInput.ReadInputParameters())
                    {
                        min3pBenchmarkInput.CloseInputFile();
                        break;
                    }
                    else
                        Environment.Exit(0);
                    break;
                }
                else
                    Console.Write("File does not exist, please check." + System.Environment.NewLine + "Type in the input file name:");
            }

            min3pBenchmarkInput.Run();

            min3pBenchmarkInput.CloseFile();
            
            
        }

        private static string _ProInfo = "=======================================================================" + System.Environment.NewLine
                                       + "                         MIN3P Batch Run & Compare                     " + System.Environment.NewLine
                                       + "                                Version 1.7                            " + System.Environment.NewLine
                                       + "                                2017-11-14                             " + System.Environment.NewLine
                                       + "  This program is designed to batch run MIN3P simulations and compare  " + System.Environment.NewLine
                                       + "  the results to the referenced benchmark.                             " + System.Environment.NewLine 
                                       + "  Please send any advice or bug to:                                    " + System.Environment.NewLine 
                                       + "  Danyang SU: dsu@eoas.ubc.ca; danyang.su@gmail.com                    " + System.Environment.NewLine 
                                       + "=======================================================================" + System.Environment.NewLine
                                       + "  Type in the input file name" + System.Environment.NewLine 
                                       + "(Press enter if you want to use the default input file MIN3PBenchmark.inp)" + System.Environment.NewLine + ":";
    }
}
