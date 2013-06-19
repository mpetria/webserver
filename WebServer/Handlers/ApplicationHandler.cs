using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Entities;

namespace WebServer.Handlers
{
    public abstract class ApplicationHandler
    {
        public abstract void HandleRequest(Request request, Response response);
    }
}
