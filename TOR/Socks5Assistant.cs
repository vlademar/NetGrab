using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetGrab.TOR
{
    static class Socks5Assistant
    {
        // http://tools.ietf.org/html/rfc1928

        private const byte Socks5Version = 0x05;
        private const byte SocksAuthNone = 0x00;
        private const byte SocksAuthLoginPass = 0x02;
        private const byte SocksCommandConnect = 0x01;
        private const byte SocksCommandConnectSuccess = 0x00;
        private const byte SocksReserved = 0x00;
        private const byte SocksAddressIPv4 = 0x01;
        private const byte SocksAddressRaw = 0x03;
        private const byte SocksAddressIPv6 = 0x04;

        internal static TcpClient EstableMasterConnection(SocksProxy socksServer)
        {
            var request = new byte[257];
            var response = new byte[257];

            var socksClient = new TcpClient(socksServer.ProxyAddress, socksServer.ProxyPort);
            socksClient.Connect(socksServer.ProxyAddress, socksServer.ProxyPort);
            var stream = socksClient.GetStream();

            var nIndex = 0;
            request[nIndex++] = Socks5Version;
            request[nIndex++] = 0x02; // далее идут 2 метода аутентификации
            request[nIndex++] = SocksAuthNone;
            request[nIndex++] = SocksAuthLoginPass;
            stream.Write(request, 0, nIndex);

            var bytesRead = stream.Read(response, 0, response.Length);
            if (bytesRead != 2)
                throw new Exception("Неверный ответ");

            if (response[1] == 0xff)
                throw new Exception("Ни один из способов аутентификации не признан сервером");

            if (response[1] == SocksAuthLoginPass)
            {
                nIndex = 0;
                request[nIndex++] = Socks5Version;

                var rawLoginBytes = Encoding.Default.GetBytes(socksServer.Login);
                request[nIndex++] = (byte)rawLoginBytes.Length;
                rawLoginBytes.CopyTo(request, nIndex);
                nIndex += (ushort)rawLoginBytes.Length;

                var rawPassBytes = Encoding.Default.GetBytes(socksServer.Pass);
                request[nIndex++] = (byte)rawPassBytes.Length;
                rawPassBytes.CopyTo(request, nIndex);
                nIndex += (ushort)rawPassBytes.Length;

                stream.Write(request, 0, nIndex);

                bytesRead = stream.Read(response, 0, response.Length);

                if (bytesRead != 2)
                    throw new Exception("Неверный ответ");
                if (response[1] != 0x00)
                    throw new Exception("Неправильные логин/пароль");
            }

            return socksClient;
        }

        internal static TcpClient CreateSocksTcpClient(SocksProxy socksServer, string destinationAddress, ushort destinationPort)
        {
            var request = new byte[257];
            var response = new byte[257];
            var stream = socksServer.MasterClient.GetStream();
            //TODO: убедждаться что MasterClient всё ещё активен

            var nIndex = 0;
            request[nIndex++] = Socks5Version;
            request[nIndex++] = SocksCommandConnect;
            request[nIndex++] = SocksReserved;

            IPAddress destIP;
            byte[] rawAddressBytes;

            if (IPAddress.TryParse(destinationAddress, out destIP))
            {
                switch (destIP.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        request[nIndex++] = SocksAddressIPv4;
                        rawAddressBytes = destIP.GetAddressBytes();
                        rawAddressBytes.CopyTo(request, nIndex);
                        nIndex += (ushort)rawAddressBytes.Length;
                        break;
                    case AddressFamily.InterNetworkV6:
                        request[nIndex++] = SocksAddressIPv6;
                        rawAddressBytes = destIP.GetAddressBytes();
                        rawAddressBytes.CopyTo(request, nIndex);
                        nIndex += (ushort)rawAddressBytes.Length;
                        break;
                }
            }
            else
            {
                request[nIndex++] = SocksAddressRaw;
                rawAddressBytes = Encoding.Default.GetBytes(destinationAddress);
                request[nIndex++] = Convert.ToByte(rawAddressBytes.Length);
                rawAddressBytes.CopyTo(request, nIndex);
                nIndex += (ushort)rawAddressBytes.Length;
            }

            byte[] portBytes = BitConverter.GetBytes(destinationPort);
            for (var i = portBytes.Length - 1; i >= 0; i--)
                request[nIndex++] = portBytes[i];

            stream.Write(request, 0, nIndex);

            stream.Read(response, 0, response.Length);
            if (response[1] != SocksCommandConnectSuccess)
                throw new Exception("Не удалось установить соединение : " + response[1]);

            var ATYP = response[3];
            var server = Encoding.Default.GetString(response, 4, ATYP);
            var portOffset = 3 + ATYP;
            var port = ( response[portOffset] << 8 ) | response[portOffset + 1];

            var socksClient = new TcpClient(server, port);
            socksClient.Connect(server, port);

            return socksClient;
        }
    }
}
