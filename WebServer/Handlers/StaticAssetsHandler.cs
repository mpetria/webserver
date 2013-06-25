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

        private const string RESPONSE_NOT_FOUND = "HTTP/1.0 404 Not Found";

        private const string SAMPLE_TEXT_RESPONSE =
            @"HTTP/1.1 200 OK
Date: Fri, 31 Dec 1999 23:59:59 GMT
Content-Type: text/plain
Content-Length: 11

Hello World";

        public void HandleRequest(Request request, Response response)
        {
            System.Diagnostics.Debug.WriteLine("Method {0} Host {1} Uri {2}", request.Method, request.Host, request.Uri);

            var headerIfModifiedSince = request.GetHeaderValue("If-Modified-Since");
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

            response.StatusCode = ResponseStatusCode.Ok;
            response.BodyBytes = fileContent;
        }

        public IList<string> GetAllowedMethods()
        {
            return new List<string>() {"GET"};
        }
    }
}