using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Utils.Logging
{
    public class RequestLogger : ConnectionLogger
    {
        private readonly string _requestId;

        public RequestLogger(string connectionId, string requestId) : base(connectionId)
        {
            _requestId = requestId;
        }

        public override void Log(string info, string message)
        {
            base.Log(String.Format("[RequestID {0}] {1}", _requestId, info), message);
        }
    }
}
