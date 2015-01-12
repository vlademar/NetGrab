using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using NetGrab.TOR;

namespace NetGrab
{
    class HttpToSocksRelay
    {
        private static readonly IPAddress localhost = IPAddress.Parse("127.0.0.1");
        private const int _4Kb = 4096;

        public int HttpPort { get; private set; }
        public int SocksPort { get; private set; }
        public bool Running { get; private set; }

        private TcpListener httpTcpListener = null;
        private TcpClient socksTcpClient = null;

        public HttpToSocksRelay(int socksPort)
        {
            InitRelay();
        }
        public HttpToSocksRelay(int httpPort, int socksPort)
        {
            InitRelay(httpPort, socksPort);
        }

        private void InitRelay()
        {

        }

        private void InitRelay(int httpPort, int socksPort)
        {
            socksTcpClient = CreateSocksTcpClient(socksPort);
            httpTcpListener = CreateTcpListener(httpPort);

            HttpPort = httpPort;
            SocksPort = socksPort;
        }

        private TcpListener CreateTcpListener(int listenerPort)
        {
            var httpEndPoint = new IPEndPoint(localhost, listenerPort);
            var tcpListener = new TcpListener(httpEndPoint);
            httpTcpListener.Start();
            httpTcpListener.BeginAcceptTcpClient(ListenerConnectionIncome, httpTcpListener);
            return tcpListener;
        }

        private void ListenerConnectionIncome(IAsyncResult ar)
        {
            var tcpListener = (TcpListener)ar.AsyncState;
            var tcpClient = tcpListener.EndAcceptTcpClient(ar);
            var state = new TcpListenerIOState
            {
                Listener = tcpListener,
                Client = tcpClient,
                ClientIoStream = tcpClient.GetStream(),
            };

            state.ClientIoStream.BeginRead(state.Buffer, 0, state.Buffer.Length, ListenerDataIncome, state);
        }

        private void ListenerDataIncome(IAsyncResult ar)
        {
            var state = (TcpListenerIOState)ar.AsyncState;
            var bytesRead = state.ClientIoStream.EndRead(ar);

            if (bytesRead != 0)
                socksTcpClient.GetStream().Write(state.Buffer, 0, bytesRead);

            if(state.Client.Connected)
                state.ClientIoStream.BeginRead(state.Buffer, 0, state.Buffer.Length, ListenerDataIncome, state);
        }

        private TcpClient CreateSocksTcpClient(int socksPort)
        {
            var socksEndPoint = new IPEndPoint(localhost, socksPort);
            var socksClient = new TcpClient(socksEndPoint);
        }
    }
}
