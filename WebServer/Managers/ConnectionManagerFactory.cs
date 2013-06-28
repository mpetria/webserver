using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using WebServer.Utils.Logging;

namespace WebServer.Managers
{
    public static class ConnectionManagerFactory
    {
        public static IConnectionManager CreateInstance(TcpClient tcpClient)
        {
            var connectionId = Guid.NewGuid().ToString();
            var logger = new ConnectionLogger(connectionId);
            var rawRequestmanager = new RawRequestManager(logger);
            var connectionManager = new ConnectionManager(tcpClient, logger);
            rawRequestmanager.SetLinkedDataManager(connectionManager);
            connectionManager.SetLinkedDataManager(rawRequestmanager);

            return connectionManager;
        }
    }
}
