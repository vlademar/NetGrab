using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Windows.Documents;
using NetGrab.TOR;

namespace NetGrab
{
    class HttpToSocksRelay
    {
        private static readonly IPAddress localhost = IPAddress.Parse("127.0.0.1");

        public int HttpPort { get; private set; }

        private SocksProxy socksProxy;
        private TcpListener httpTcpListener;
        private TcpClient socksTcpClient;
        private List<TcpDialog> dialogs = new List<TcpDialog>();

        public HttpToSocksRelay(SocksProxy socksServer)
        {
            InitRelay(9999, socksServer);
        }
        public HttpToSocksRelay(int httpPort, SocksProxy socksServer)
        {
            InitRelay(httpPort, socksServer);
        }

        private void InitRelay(int httpPort, SocksProxy socks)
        {
            socksProxy = socks;
            HttpPort = httpPort;

            httpTcpListener = CreateTcpListener(httpPort);
            Socks5Assistant.EstableMasterConnection(socks);
        }

        private TcpListener CreateTcpListener(int listenerPort)
        {
            var httpEndPoint = new IPEndPoint(localhost, listenerPort);
            var tcpListener = new TcpListener(httpEndPoint);
            tcpListener.Start();
            LoopAcceptTcpClient(tcpListener);
            return tcpListener;
        }

        private void LoopAcceptTcpClient(TcpListener tcpListener)
        {
            tcpListener.BeginAcceptTcpClient(ListenerConnectionIncome, tcpListener);
        }

        private void ListenerConnectionIncome(IAsyncResult ar)
        {
            var tcpListener = (TcpListener)ar.AsyncState;
            var tcpClient = tcpListener.EndAcceptTcpClient(ar);
            var dialog = new TcpDialog(tcpClient, socksProxy);
            LoopAcceptTcpClient(tcpListener);
        }

    }
}
