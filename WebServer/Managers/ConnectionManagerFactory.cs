using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using WebServer.Config;
using WebServer.Utils;


namespace WebServer.Managers
{
    public static class ConnectionManagerFactory
    {
        public static IConnectionManager CreateInstance(TcpClient tcpClient)
        {
            var connectionId = Guid.NewGuid().ToString();
            var serverConfig = ServerConfigFactory.GetServerConfig();
            var connectionLogger = new Logger(String.Format("[ConnectionID {0}]", connectionId));

            Func<IRequestManager> requestManagerFactory = () =>
                                            {
                                                var requestId = Guid.NewGuid().ToString();
                                                var requestLogger = new Logger(String.Format("[ConnectionID {0}][RequestID {1}]", connectionId, requestId));
                                                
                                                return new RequestManager(serverConfig, requestLogger);
                                            };


            var connectionManager = new ConnectionManager(serverConfig, connectionLogger, requestManagerFactory, connectionId);


            return connectionManager;
        }
    }
}
