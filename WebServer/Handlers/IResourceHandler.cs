using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebServer.Entities;

namespace WebServer.Handlers
{
    public interface IResourceHandler
    {
        bool CheckIfExists(string resourceUri);
        IList<string> GetAllowedMethods(string resourceUri);
        IList<string> GetAllowedMediaTypes(string resourceUri, string method);

        bool GetVersioning(string resourceUri, out string lastUpdateDate, out string eTag);

        long GetResourceLength(string resourceUri, string contentType);
        byte[] GetResourceBytes(string resourceUri, string contentType);
        Stream GetResourceStream(string resourceUri, string contentType);

        IList<string> GetAvailableMediaTypes(string resourceUri, string method);

        bool CreateOrUpdateResource(string resourceUri, string contentType, byte[] content);
        

    }
}
