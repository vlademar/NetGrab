using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetGrab
{
    internal interface ILogger
    {
        void Add(string message);
    }

    class Logger : ILogger
    {
        private const int maxCoutn = 10;

        private StreamWriter stream;
        private int count;

        public Logger(string logFile)
        {
            stream = new StreamWriter(logFile, false, Encoding.Default);
        }

        public void Add(string message)
        {
            stream.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + "   " + message);

            count++;

            if (count > maxCoutn)
            {
                count = 0;
                stream.Flush();
                stream.BaseStream.Flush();
            }
        }
    }
}
