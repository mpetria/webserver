using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Entities;

namespace WebServer.Handlers
{
    public interface IRequestHandler
    {
        void HandleRequest(Request request, Response response);
        IList<string> GetAllowedMethods();
    }
}
