using System;
using System.IO;
using System.Text;
using System.Windows.Threading;

namespace NetGrab
{
    public interface ILogger
    {
        void Add(string message, bool forceWrite = false);
    }

    class Logger : ILogger
    {
        private const int maxCoutn = 10;

        private readonly Dispatcher dispatcher;
        private readonly StreamWriter stream;
        private int count;

        private delegate void AddThreadSafeDelegate(string message, bool forceWrite);


        public Logger(string logFile)
        {
            stream = new StreamWriter(logFile, false, Encoding.Default);
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Add(string message, bool forceWrite = false)
        {
            dispatcher.Invoke(new AddThreadSafeDelegate(AddThreadSafe), message, forceWrite);
        }

        private void AddThreadSafe(string message, bool forceWrite)
        {
            lock (stream)
            {
                stream.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + "   " + message);

                count++;

                if (count > maxCoutn || forceWrite)
                {
                    count = 0;
                    stream.Flush();
                    stream.BaseStream.Flush();
                }
            }
        }
    }
}
