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
    internal abstract class AsyncLoaderBase : ILoader
    {
        public delegate void SaveFileCompleteCallback(int fileLength, Exception error);
        public delegate void LoadStringCompleteCallback(string s, string actualUrl, Exception error);

        public class RequestState
        {
            public const int BufferSize = 64 * 1024;
            public byte[] buffer;
            public HttpWebRequest request;
            public HttpWebResponse response;
            public Stream streamResponse;
            public Stream resultStream;
            public string path;
            public RequestState()
            {
                buffer = new byte[BufferSize];
                request = null;
                streamResponse = null;
            }
        }

        protected ITaskHost taskHost;
        protected ILogger logger;

        private const int bufferSize = 128 * 1024;
        private byte[] buffer = new byte[bufferSize];
        int length;

        private LoadStringCompleteCallback loadStringCompleteCallback;
        private SaveFileCompleteCallback saveFileCompleteCallback;

        protected void LoadStringAsync(string url, LoadStringCompleteCallback callback)
        {
            LoadStringAsync(url, null, callback);
        }
        protected void LoadStringAsync(string url, WebProxy proxy, LoadStringCompleteCallback callback)
        {
            loadStringCompleteCallback = callback;

            try
            {
                var uri = new Uri(url);

                var request = (HttpWebRequest)HttpWebRequest.CreateDefault(uri);

                request.Accept = @"text/html, application/xhtml+xml, */*";
                request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
                request.Headers[HttpRequestHeader.AcceptLanguage] = "ru-RU";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

                if (proxy != null)
                    request.Proxy = proxy;

                var requestState = new RequestState
                {
                    request = request
                };

                request.BeginGetResponse(LoadStringGetResponse, requestState);
            }
            catch (Exception e)
            {
                loadStringCompleteCallback.Invoke(string.Empty, string.Empty, e);
            }
        }
        private void LoadStringGetResponse(IAsyncResult ar)
        {

            try
            {
                var requestState = (RequestState)ar.AsyncState;

                requestState.response = (HttpWebResponse)requestState.request.EndGetResponse(ar);
                requestState.streamResponse = requestState.response.GetResponseStream();
                requestState.resultStream = new MemoryStream();

                length = 0;

                requestState.streamResponse.BeginRead(requestState.buffer, 0, RequestState.BufferSize, LoadStringEnd, requestState);
            }
            catch (Exception e)
            {
                loadStringCompleteCallback.Invoke(string.Empty, string.Empty, e);
            }
        }
        private void LoadStringEnd(IAsyncResult ar)
        {
            var state = (RequestState)ar.AsyncState;

            var loadedBytes = state.streamResponse.EndRead(ar);

            state.resultStream.Write(state.buffer, 0, loadedBytes);
            length += loadedBytes;

            if (loadedBytes != 0)
            {
                state.streamResponse.BeginRead(state.buffer, 0, RequestState.BufferSize, LoadStringEnd, state);
                return;
            }

            state.resultStream.Position = 0;
            Stream decodedStream;

            if (state.response.ContentEncoding.ToLower().Contains("gzip"))
                decodedStream = new GZipStream(state.resultStream, CompressionMode.Decompress);
            else if (state.response.ContentEncoding.ToLower().Contains("deflate"))
                decodedStream = new DeflateStream(state.resultStream, CompressionMode.Decompress);
            else
                decodedStream = state.resultStream;

            var responseSr = new StreamReader(decodedStream);

            var result = responseSr.ReadToEnd();

            state.streamResponse.Close();
            state.response.Close();
            state.resultStream.Flush();
            state.resultStream.Close();

            responseSr.Close();

            loadStringCompleteCallback.Invoke(result, state.response.ResponseUri.ToString(), null);
        }

        protected void SaveFileAsync(string url, string path, SaveFileCompleteCallback callback)
        {
            SaveFileAsync(url, path, null, callback);
        }
        protected void SaveFileAsync(string url, string path, WebProxy proxy, SaveFileCompleteCallback callback)
        {
            saveFileCompleteCallback = callback;

            try
            {
                var uri = new Uri(url);

                var request = (HttpWebRequest)HttpWebRequest.CreateDefault(uri);

                request.Accept = @"text/html, application/xhtml+xml, */*";
                request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
                request.Headers[HttpRequestHeader.AcceptLanguage] = "ru-RU";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

                if (proxy != null)
                    request.Proxy = proxy;

                var requestState = new RequestState
                {
                    request = request,
                    path = path
                };

                request.BeginGetResponse(FileReadGetResponse, requestState);

            }
            catch (Exception e)
            {
                saveFileCompleteCallback.Invoke(-1, e);
            }
        }
        private void FileReadGetResponse(IAsyncResult ar)
        {
            try
            {
                var requestState = (RequestState)ar.AsyncState;

                requestState.streamResponse = requestState.response.GetResponseStream();
                requestState.resultStream = new FileStream(requestState.path, FileMode.Create);

                length = 0;

                requestState.streamResponse.BeginRead(requestState.buffer, 0, RequestState.BufferSize, FileReadEnd, requestState);
            }
            catch (Exception e)
            {
                saveFileCompleteCallback.Invoke(-1, e);
            }
        }
        private void FileReadEnd(IAsyncResult ar)
        {
            var state = (RequestState)ar.AsyncState;

            var loadedBytes = state.streamResponse.EndRead(ar);

            state.resultStream.Write(state.buffer, 0, loadedBytes);
            length += loadedBytes;

            if (loadedBytes != 0)
            {
                state.streamResponse.BeginRead(state.buffer, 0, RequestState.BufferSize, FileReadEnd, state);
                return;
            }

            state.streamResponse.Close();
            state.response.Close();
            state.resultStream.Flush();
            state.resultStream.Close();

            saveFileCompleteCallback.Invoke(length, null);
        }


        public void Init(ITaskHost _taskHost, ILogger _logger)
        {
            taskHost = _taskHost;
            logger = _logger;
            OnInit();
        }

        public virtual void OnInit()
        { }

        public abstract void DoWork(Task task);
    }
}
