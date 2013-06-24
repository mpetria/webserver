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

            var connectionManager = new ConnectionManager(tcpClient);
            connectionManager.ProcessBytes();
        }
    }
}
