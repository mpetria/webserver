using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Entities;

namespace WebServer.Handlers
{
    public class StaticAssetsHandler : ApplicationHandler
    {
        private readonly string _directory;

        public StaticAssetsHandler(string directory)
        {
            _directory = directory;
        }


        public override void HandleRequest(Request request, Response response)
        {
            System.Diagnostics.Debug.WriteLine("Method {0} Host {1} Uri {2}", request.Method, request.Host, request.Uri);
        }
    }
}
