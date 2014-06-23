using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Threading;

namespace NetGrab
{
    internal abstract class SyncLoaderBase : ILoader
    {
        protected ITaskHost taskHost;
        protected ILogger logger;

        private const int bufferSize = 128*1024;
        private byte[] buffer = new byte[bufferSize];
        int length ;

        protected string downloadPathBase;

        protected string LoadTextFile(string url, out string actualUrl)
        {
            var uri = new Uri(url);

            var request = (HttpWebRequest)HttpWebRequest.CreateDefault(uri);

            request.Accept = @"text/html, application/xhtml+xml, */*";
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            request.Headers[HttpRequestHeader.AcceptLanguage] = "ru-RU";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

            var response = (HttpWebResponse)request.GetResponse();

            actualUrl = response.ResponseUri.ToString();

            Stream responseStream = response.GetResponseStream();

            if (response.ContentEncoding.ToLower().Contains("gzip"))
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            else if (response.ContentEncoding.ToLower().Contains("deflate"))
                responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

            var responseSr = new StreamReader(responseStream);

            var result = responseSr.ReadToEnd();

            responseSr.Close();
            response.Close();

            return result;
        }

        protected int SaveFile(string url, string path)
        {
            var uri = new Uri(url);

            var request = (HttpWebRequest)HttpWebRequest.CreateDefault(uri);

            request.Accept = @"text/html, application/xhtml+xml, */*";
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            request.Headers[HttpRequestHeader.AcceptLanguage] = "ru-RU";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

            var response = (HttpWebResponse)request.GetResponse();

            var responseStream = response.GetResponseStream();
            var fs = new FileStream(path, FileMode.Create);


            int length = 0;

            var count = responseStream.Read(buffer, 0, bufferSize);
            while (count > 0)
            {
                length += count;
                fs.Write(buffer, 0, count); ;
                count = responseStream.Read(buffer, 0, bufferSize);
            }

            responseStream.Close();
            response.Close();
            fs.Flush();
            fs.Close();

            return length;
        }

        public abstract ILoader New();

        public void Init(ITaskHost _taskHost, ILogger _logger, string _downloadPathBase)
        {
            taskHost = _taskHost;
            logger = _logger;
            downloadPathBase = _downloadPathBase;

            if (!Directory.Exists(downloadPathBase))
                Directory.CreateDirectory(downloadPathBase);

            OnInit();
        }

        public virtual void OnInit()
        { }

        public abstract void DoWork(Task task);
    }
}
