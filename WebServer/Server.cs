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
        private IDictionary<string, IConnectionManager> _connections; 
        private readonly object _syncObject = new object();

        public void Start(IPAddress ipAddress, int port)
        {
            _connections = new Dictionary<string, IConnectionManager>();

            _tcpListener = new TcpListener(ipAddress, port);
            _tcpListenerThread = new Thread(ListenForClients);
            _tcpListenerThread.Start();
            
        }

        public void Stop()
        {
            lock (_syncObject)
            {
                _tcpListener.Server.Close();

                foreach (var connection in _connections)
                {
                    connection.Value.Close();
                }
            }
            _tcpListenerThread.Abort();
        }

        private void ListenForClients()
        {
            _tcpListener.Start();
            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = _tcpListener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(HandleClientCommunication, client);
            }
        }


        private void HandleClientCommunication(object tcpClientObject)
        {
            TcpClient tcpClient = tcpClientObject as TcpClient;

            var connectionManager = ConnectionManagerFactory.CreateInstance(tcpClient);

            lock (_syncObject)
            {
                _connections.Add(connectionManager.ConnectionId, connectionManager);
            }

            connectionManager.ListenForBytesFromClient();

            lock (_syncObject)
            {
                _connections.Remove(connectionManager.ConnectionId);
            }
        }
    }
}
