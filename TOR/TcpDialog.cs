using System;
using System.Net.Sockets;

namespace NetGrab.TOR
{
    class TcpDialog
    {
        public TcpClient HttpClient { get; private set; }
        public TcpClient SocksClient { get; private set; }

        public TcpDialog(TcpClient httpClient, SocksProxy proxy)
        {
            HttpClient = httpClient;
            EstableDialog(proxy);
        }

        private void EstableDialog(SocksProxy proxy)
        {
            SocksClient = Socks5Assistant.CreateSocksTcpClient(proxy, )
        }


        private void AcceptAndReadIncomeClient(TcpClient client)
        {
            var state = new TcpListenerIOState
            {
                Client = client,
                ClientIoStream = client.GetStream(),
            };

            state.ClientIoStream.BeginRead(state.Buffer, 0, state.Buffer.Length, ClientDataIncome, state);
        }

        private void ClientDataIncome(IAsyncResult ar)
        {
            var state = (TcpListenerIOState)ar.AsyncState;
            var bytesRead = state.ClientIoStream.EndRead(ar);

            if (bytesRead != 0)
                socksTcpClient.GetStream().Write(state.Buffer, 0, bytesRead);

            if (state.Client.Connected)
                state.ClientIoStream.BeginRead(state.Buffer, 0, state.Buffer.Length, ClientDataIncome, state);
        }
    }
}
