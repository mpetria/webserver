using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using WebServer.Utils;


namespace WebServer.Managers
{
    public class ConnectionManager : IConnectionManager, IDataManager
    {
        private readonly TcpClient _tcpClient;
        private readonly ILogger _logger;
        private readonly NetworkStream _clientStream;
        private IDataManager _requestManager;
        private bool _connectionOpened = true;

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
            _connectionOpened = true;

            while (_connectionOpened)
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
            _logger.Log("Bytes Sent", responseBytes);

            clientStream.Write(responseBytes, 0, responseBytes.Length);
            clientStream.Flush();
            
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

        public void Close()
        {
            _connectionOpened = false;
        }

        public void SetLinkedDataManager(IDataManager dataManager)
        {
            _requestManager = dataManager;
        }
    }
}
