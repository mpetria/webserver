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
    public class FileHandler : IResourceHandler
    {
        private readonly string _directory;
        private readonly ServerConfig _serverConfig;

        public FileHandler(string directory)
        {
            _directory = directory;
            _serverConfig = ServerConfig.Instance;
        }


        public bool CheckIfExists(string resourceUri)
        {
            var filePath = GetPhysicalPath(resourceUri);

            if (!File.Exists(filePath))
                return false;
            return true;
        }

        public IList<string> GetAllowedMethods(string resourceUri)
        {
            return new List<string>() { HTTPMethod.GET, HTTPMethod.HEAD };
        }

        public IList<string> GetAllowedMediaTypes(string resourceUri, string method)
        {
            return new List<string>();
        }

        public bool GetVersioning(string resourceUri, out string lastUpdateDate, out string eTag)
        {
            var filePath = GetPhysicalPath(resourceUri);
            DateTime lastModifiedDate = File.GetLastWriteTime(filePath);
            lastUpdateDate = DateUtils.GetFormatedServerDate(lastModifiedDate.ToUniversalTime());
            eTag = lastUpdateDate.Replace(' ','-');
            return true;
        }

        public long GetResourceLength(string resourceUri, string contentType)
        {
            var filePath = GetPhysicalPath(resourceUri);
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        public byte[] GetResourceBytes(string resourceUri, string contentType)
        {
            var filePath = GetPhysicalPath(resourceUri);
            var fileContent = File.ReadAllBytes(filePath);
            return fileContent;
        }

        public Stream GetResourceStream(string resourceUri, string contentType)
        {
            var filePath = GetPhysicalPath(resourceUri);
            return File.OpenRead(filePath);
        }

        public IList<string> GetAvailableMediaTypes(string resourceUri, string method)
        {
            var filePath = GetPhysicalPath(resourceUri);
            var extension = Path.GetExtension(filePath);
            var mediaType = _serverConfig.GetMimeTypeForExtension(extension);
            return new List<string>(){ mediaType };
        }

        public bool HandleBody(Request request, Response response)
        {
            var filePath = GetPhysicalPath(request.Uri);

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

        public string GetPhysicalPath(string requestUri)
        {
            var filePath = Path.Combine(_directory, requestUri.Trim("/".ToCharArray()));
            return filePath;
        }
    }
}