using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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

        private const string RESPONSE_NOT_FOUND = "HTTP/1.0 404 Not Found";

        private const string sampleRespone =
            @"HTTP/1.1 200 OK
Date: Fri, 31 Dec 1999 23:59:59 GMT
Content-Type: text/plain
Content-Length: 11

Hello World";

        private void HandleClientCommunication(object tcpClientObject)
        {
            TcpClient tcpClient = tcpClientObject as TcpClient;
            var clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;


            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
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

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();
                System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, bytesRead));

                byte[] buffer = encoder.GetBytes(sampleRespone);

                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
                
            }
        }
    }
}
