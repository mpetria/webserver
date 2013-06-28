using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using WebServer.Utils.Logging;

namespace WebServer.Managers
{
    public class ConnectionManager : IConnectionManager, IDataManager
    {
        private readonly TcpClient _tcpClient;
        private readonly string _connectionId;
        private readonly ILogger _logger;
        private readonly NetworkStream _clientStream;
        private IDataManager _requestManager;

        public ConnectionManager(TcpClient tcpClient, ILogger logger)
        {
            _tcpClient = tcpClient;
            _logger = logger;
            _clientStream = _tcpClient.GetStream();
        }

        public void ListenForBytesFromClient()
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

                var bytes = buffer.Take(bytesRead).ToArray();
                //message has successfully been received
                _logger.Log("Bytes Received", bytes);

                _requestManager.ManageBytes(bytes);

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

        public void ManageBytes(byte[] bytes)
        {
            SendBytesToClient(_clientStream, bytes);
        }

        public void ManageStream(Stream stream)
        {
            if (stream != null)
            {
                while (true)
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;

                    try
                    {
                        //blocks until a client sends a message
                        bytesRead = stream.Read(buffer, 0, 4096);
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

                    var bytes = buffer.Take(bytesRead).ToArray();
                    ManageBytes(bytes);
                }
            }
        }

        public void SetLinkedDataManager(IDataManager dataManager)
        {
            _requestManager = dataManager;
        }
    }
}
