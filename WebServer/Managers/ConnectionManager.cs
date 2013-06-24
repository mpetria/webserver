using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using WebServer.Utils.Logging;

namespace WebServer.Managers
{
    public class ConnectionManager
    {
        private readonly TcpClient _tcpClient;
        private readonly string _connectionId;
        private readonly ILogger _logger;
        private readonly NetworkStream _clientStream;
        private readonly RawRequestManager _rawRequestManager;

        public ConnectionManager(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _connectionId = Guid.NewGuid().ToString();
            _logger = new ConnectionLogger(_connectionId);
            _clientStream = _tcpClient.GetStream(); 
            _rawRequestManager = new RawRequestManager(responseBytes => SendBytesToClient(_clientStream, responseBytes));

        }

        public void ProcessBytes()
        {
            _logger.Log("Connection Opened");

            byte[] buffer = new byte[4096];


            while (true)
            {
                int bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = _clientStream.Read(buffer, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                var message = buffer.Take(bytesRead).ToArray();
                //message has successfully been received
                _logger.Log("Bytes Received", message);

                _rawRequestManager.ProcessBytes(message);

                _logger.Log("Bytes Processed");
            }

            _logger.Log("Connection Closed");
        }

        void SendBytesToClient(NetworkStream clientStream, byte[] responseBytes)
        {
            clientStream.Write(responseBytes, 0, responseBytes.Length);
            clientStream.Flush();
            _logger.Log("Bytes Sent", responseBytes);
        }
    }
}
