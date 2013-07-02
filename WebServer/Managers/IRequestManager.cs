using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Entities;

namespace WebServer.Managers
{
    public interface IRequestManager
    {
        Response ProceesRequest(Request request, bool processExpectation = false);
    }
}
