using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Utils.Logging
{
    public interface ILogger
    {
        void Log(string info, string message = null);
        void Log(string info, byte[] message);
    }
}
