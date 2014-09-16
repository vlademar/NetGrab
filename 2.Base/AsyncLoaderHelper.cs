using System;
using System.IO;
using System.Net;

namespace NetGrab
{
    public partial class AsyncLoaderHelper
    {
        private class RequestState
        {
            public const int BufferSize = 64 * 1024;
            public readonly byte[] buffer;
            public HttpWebRequest request;
            public HttpWebResponse response;
            public Stream streamResponse;
            public Stream resultStream;
            public int resultStreamLength;

            public RequestState()
            {
                buffer = new byte[BufferSize];
                resultStreamLength = 0;
                request = null;
                streamResponse = null;
            }
        }

        private IAsyncLoaderHelperMode mode;
        public bool IsRunning { get; private set; }

        public void LoadStringAsync(string url, WebProxy proxy, LoadStringCompleteCallback callback)
        {
            if (IsRunning)
                throw new Exception("IsRunning");

            mode = new IAsyncLoaderHelperLoadString(callback);
            LoadStreamAsyncInternal(url, proxy);
        }
        public void SaveFileAsync(string url, string path, WebProxy proxy, SaveFileCompleteCallback callback)
        {
            if (IsRunning)
                throw new Exception("IsRunning");

            mode = new IAsyncLoaderHelperSaveFile(path, callback);
            LoadStreamAsyncInternal(url, proxy);
        }

        private void LoadStreamAsyncInternal(string url, WebProxy proxy)
        {
            IsRunning = true;

            var uri = new Uri(url);

            var request = (HttpWebRequest)WebRequest.CreateDefault(uri);

            request.Accept = @"text/html, application/xhtml+xml, */*";
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            request.Headers[HttpRequestHeader.AcceptLanguage] = "ru-RU";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";

            if (proxy != null)
                request.Proxy = proxy;

            var requestState = new RequestState { request = request };

            try
            {
                request.BeginGetResponse(LoadStreamGetResponse, requestState);
            }
            catch (Exception e)
            {
                OnFail(requestState, e);
            }
        }
        private void LoadStreamGetResponse(IAsyncResult ar)
        {
            var requestState = (RequestState)ar.AsyncState;

            try
            {
                requestState.response = (HttpWebResponse)requestState.request.EndGetResponse(ar);
                requestState.streamResponse = requestState.response.GetResponseStream();
                requestState.resultStream = new MemoryStream();
                requestState.streamResponse.BeginRead(requestState.buffer, 0, RequestState.BufferSize, LoadStreamEndInternal, requestState);
            }
            catch (Exception e)
            {
                OnFail(requestState, e);
            }
        }
        private void LoadStreamEndInternal(IAsyncResult ar)
        {
            var state = (RequestState)ar.AsyncState;
            var loadedBytes = state.streamResponse.EndRead(ar);
            state.resultStream.Write(state.buffer, 0, loadedBytes);
            state.resultStreamLength += loadedBytes;

            if (loadedBytes != 0)
            {
                state.streamResponse.BeginRead(state.buffer, 0, RequestState.BufferSize, LoadStreamEndInternal, state);
                return;
            }

            state.resultStream.Position = 0;
            IsRunning = false;
            mode.OnSuccess(state);

            state.streamResponse.Close();
            state.response.Close();
            state.resultStream.Flush();
            state.resultStream.Close();
        }

        private void OnFail(RequestState state, Exception e)
        {
            IsRunning = false;

            if (state.streamResponse != null)
                state.streamResponse.Close();

            if (state.response != null)
                state.response.Close();

            if (state.resultStream != null)
                state.resultStream.Close();

            mode.OnFail(e);
        }
    }


}
