﻿using System;
using System.IO;
using System.Text;
using System.Windows.Threading;

namespace NetGrab
{
    public interface ILogger
    {
        void Add(string message);
        void Add(string format, params object[] args);
        void Flush();
    }

    class Logger : ILogger, IDisposable
    {
        private readonly bool _autoFlush;
        private const int maxCoutn = 10;

        private readonly Dispatcher dispatcher;
        private readonly StreamWriter stream;
        private int count;

        private delegate void AddThreadSafeDelegate(string message, bool forceWrite);

        public Logger(string logFile)
            : this(logFile, false) { }

        public Logger(string logFile, bool autoFlush)
        {
            _autoFlush = autoFlush;
            stream = new StreamWriter(logFile, true, Encoding.Default);
            stream.WriteLine("");
            stream.WriteLine("");
            stream.WriteLine("--------------------------------------------------------------------------------------------------------");
            stream.WriteLine("               " + DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss"));
            stream.WriteLine("--------------------------------------------------------------------------------------------------------");
            stream.Flush();

            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void Add(string message)
        {
            dispatcher.Invoke(new AddThreadSafeDelegate(AddThreadSafe), message, false);
        }

        public void Add(string format, params object[] args)
        {
            var message = string.Format(format, args);
            dispatcher.Invoke(new AddThreadSafeDelegate(AddThreadSafe), message, false);
        }

        public void Flush()
        {
            dispatcher.Invoke(new AddThreadSafeDelegate(AddThreadSafe), "FLUSH", true);
        }

        private void AddThreadSafe(string message, bool flush)
        {
            lock (stream)
            {
                stream.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + "   " + message);

                count++;
                if (_autoFlush || count > maxCoutn || flush)
                {
                    count = 0;
                    stream.Flush();
                    stream.BaseStream.Flush();
                }
            }
        }

        public void Dispose()
        {
            stream.Flush();
            stream.BaseStream.Flush();

            stream.Close();
        }
    }
}
