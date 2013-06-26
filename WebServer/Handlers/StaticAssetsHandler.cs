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
            
            var filePath = Path.Combine(_directory, request.Uri.Trim("/".ToCharArray()));

            bool returnResponse = false;

            if(!File.Exists(filePath))
            {
                response.StatusCode = ResponseStatusCode.NotFound;
                return;
            }

            returnResponse = HandleCaching(request, response);
            if(returnResponse) return;
            
            
            var extension = Path.GetExtension(filePath);
            response.ContentType = new ServerConfig().GetMimeTypeForExtension(extension);
            response.StatusCode = ResponseStatusCode.OK;

            HandleBody(request, response);
        }

        public IList<string> GetAllowedMethods()
        {
            return new List<string>() { HTTPMethod.GET, HTTPMethod.HEAD };
        }

        public bool HandleCaching(Request request, Response response)
        {
            var filePath = GetPhysicalPath(request);
            var headerIfModifiedSince = request.GetHeaderValue(HttpHeader.IfModifiedSince);
            DateTime lastModifiedDate = File.GetLastWriteTime(filePath);
            
            response.LastModified = lastModifiedDate;

            if (headerIfModifiedSince != null && DateUtils.CheckIfDatesMatch(lastModifiedDate, headerIfModifiedSince))
            {
                response.StatusCode = ResponseStatusCode.NotModified;
                return true;
            }

            return false;
        }

        public bool HandleBody(Request request, Response response)
        {
            var filePath = GetPhysicalPath(request);

            if(request.Method == HTTPMethod.HEAD)
            {
                var fileInfo = new FileInfo(filePath);
                response.Headers[HttpHeader.ContentLength] = fileInfo.Length.ToString();
                response.SuppressBody = true;
            }
            else if(request.Method == HTTPMethod.GET)
            {
                var fileContent = File.ReadAllBytes(filePath);
                response.BodyBytes = fileContent;
            }

            return false;
        }

        public string GetPhysicalPath(Request request)
        {
            var filePath = Path.Combine(_directory, request.Uri.Trim("/".ToCharArray()));
            return filePath;
        }
    }
}