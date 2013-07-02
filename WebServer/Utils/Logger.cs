using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WebServer.Utils
{
    public class Logger : ILogger
    {
        private readonly string _loggerId;


        public Logger(string loggerId)
        {
            _loggerId = loggerId;
        }

        public virtual void Log(string info, string message)
        {
            message = message ?? String.Empty;
            Debug.WriteLine("{0} {1}\n{2}", _loggerId, info, message);
        }

        public void Log(string info, byte[] message)
        {
            message = message ?? new byte[0];

            ASCIIEncoding encoder = new ASCIIEncoding();
            Log(info, encoder.GetString(message));
        }
    }
}
