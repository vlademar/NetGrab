using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace NetGrab
{
    internal interface ILogger
    {
        void Add(string message);
    }

    class Logger : ILogger
    {
        private const int maxCoutn = 10;

        private Dispatcher dispatcher;
        private StreamWriter stream;
        private int count;

        private delegate void AddThreadSafeDelegate(string message);


        public Logger(string logFile)
        {
            stream = new StreamWriter(logFile, false, Encoding.Default);

            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Add(string message)
        {
            dispatcher.Invoke(new AddThreadSafeDelegate(AddThreadSafe), message);
        }

        private void AddThreadSafe(string message)
        {
            lock (stream)
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
}
