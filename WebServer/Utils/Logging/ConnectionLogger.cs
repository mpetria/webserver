using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WebServer.Utils.Logging
{
    public class ConnectionLogger : ILogger
    {
        private readonly string _connectionId;

        public ConnectionLogger(string connectionId)
        {
            _connectionId = connectionId;
        }

        public virtual void Log(string info, string message)
        {
            message = message ?? String.Empty;
            Debug.WriteLine("[ConnectionID: {0}] {1}\n{2}", _connectionId, info, message);
        }

        public void Log(string info, byte[] message)
        {
            message = message ?? new byte[0];

            ASCIIEncoding encoder = new ASCIIEncoding();
            Log(info, encoder.GetString(message));
        }
    }
}
