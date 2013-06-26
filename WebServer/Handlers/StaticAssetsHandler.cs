using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebServer.Config;
using WebServer.Entities;
using WebServer.Utils;

namespace WebServer.Handlers
{
    public class StaticAssetsHandler : IRequestHandler
    {
        private readonly string _directory;

        public StaticAssetsHandler(string directory)
        {
            _directory = directory;
        }

        public void HandleRequest(Request request, Response response)
        {
            var headerIfModifiedSince = request.GetHeaderValue(HttpHeader.IfModifiedSince);
            var filePath = Path.Combine(_directory, request.Uri.Trim("/".ToCharArray()));

            if(!File.Exists(filePath))
            {
                response.StatusCode = ResponseStatusCode.NotFound;
                return;
            }

            DateTime lastModifiedDate = File.GetLastWriteTime(filePath);
            if (headerIfModifiedSince != null && DateUtils.CheckIfDatesMatch(lastModifiedDate, headerIfModifiedSince))
            {
                response.StatusCode = ResponseStatusCode.NotModified;
                response.LastModified = lastModifiedDate;
                return;
            }
            
            var fileContent = File.ReadAllBytes(filePath);
            response.LastModified = lastModifiedDate;
            var extension = Path.GetExtension(filePath);
            response.ContentType = new ServerConfig().GetMimeTypeForExtension(extension);

            response.StatusCode = ResponseStatusCode.OK;
            response.BodyBytes = fileContent;
        }

        public IList<string> GetAllowedMethods()
        {
            return new List<string>() {"GET"};
        }
    }
}