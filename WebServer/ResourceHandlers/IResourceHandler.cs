using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebServer.Data;

namespace WebServer.ResourceHandlers
{
    // This interface is an abstractization of a resource state
    public interface IResourceHandler
    {
        bool CheckIfExists(string resourceUri);
        IList<string> GetAllowedMethods(string resourceUri);
        IList<string> GetAllowedMediaTypes(string resourceUri, string method);

        bool GetVersioning(string resourceUri, out string lastUpdateDate, out string eTag);

        long GetResourceLength(string resourceUri, string contentType);
        
        // Used by GET
        byte[] GetResourceBytes(string resourceUri, string contentType);
        Stream GetResourceStream(string resourceUri, string contentType);

        IList<string> GetAvailableMediaTypes(string resourceUri, string method);

        // Used by PUT
        bool CreateOrUpdateResource(string resourceUri, string contentType, byte[] content);
        
        // Used by DELETE
        bool DeleteResource(string resourceUri);

        // Used by POST - returns the uri of a new resoruces if it was created
        string AlterResource(string resourceUri, string contentType, byte[] content);
        

    }
}
