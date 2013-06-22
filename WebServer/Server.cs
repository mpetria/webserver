using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WebServer.Managers;

namespace WebServer
{
    public class Server
    {
        private TcpListener _tcpListener;
        private Thread _tcpListenerThread;

        public void Start(IPAddress ipAddress, int port)
        {
            _tcpListener = new TcpListener(ipAddress, port);
            _tcpListenerThread = new Thread(new ThreadStart(ListenForClients));
            _tcpListenerThread.Start();
        }

        public void Stop()
        {
            
        }

        private void ListenForClients()
        {
            _tcpListener.Start();
            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = _tcpListener.AcceptTcpClient();
                System.Diagnostics.Debug.WriteLine("New Client");

                //create a thread to handle communication 
                //with connected client
                Thread clientThread = new Thread(HandleClientCommunication);
                clientThread.Start(client);
            }
        }
      

        private void HandleClientCommunication(object tcpClientObject)
        {
            TcpClient tcpClient = tcpClientObject as TcpClient;
            var clientStream = tcpClient.GetStream();

            byte[] buffer = new byte[4096];

            RawHttpManager rawHttpManager = new RawHttpManager(responseBytes => SendBytesToClient(clientStream, responseBytes));


            while (true)
            {
                int bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(buffer, 0, 4096);
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
                ASCIIEncoding encoder = new ASCIIEncoding();
                System.Diagnostics.Debug.WriteLine(encoder.GetString(message));
                rawHttpManager.ProcessBytes(message);
                
                
            }
        }

        void SendBytesToClient(NetworkStream clientStream, byte[] responseBytes)
        {
            clientStream.Write(responseBytes, 0, responseBytes.Length);
            clientStream.Flush();
        }
    }
}
