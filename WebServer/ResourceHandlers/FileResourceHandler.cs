using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebServer.Config;
using WebServer.Data;
using WebServer.Utils;

namespace WebServer.ResourceHandlers
{
    public class FileResourceHandler : FsResourceHandler
    {
        private readonly ServerConfig _serverConfig;

        public FileResourceHandler(string directory, ServerConfig serverConfig) : base(directory)
        {
            _serverConfig = serverConfig;
        }


        override public bool CheckIfExists(string resourceUri)
        {
            var filePath = GetPhysicalPath(resourceUri);

            if (!File.Exists(filePath))
                return false;
            return true;
        }

        override public IList<string> GetAllowedMethods(string resourceUri)
        {
            return new List<string>() { HttpMethod.GET, HttpMethod.HEAD, HttpMethod.PUT, HttpMethod.DELETE };
        }

        override public IList<string> GetAllowedMediaTypes(string resourceUri, string method)
        {
            // allowing the same media types that can be produced
            return GetAvailableMediaTypes(resourceUri, method);
        }

        override public bool GetVersioning(string resourceUri, out string lastUpdateDate, out string eTag)
        {
            var filePath = GetPhysicalPath(resourceUri);
            DateTime lastModifiedDate = File.GetLastWriteTimeUtc(filePath);
            lastUpdateDate = DateUtils.GetFormatedHttpDateFromUtcDate(lastModifiedDate);
            eTag = null;
            return true;
        }

        override public long GetResourceLength(string resourceUri, string contentType)
        {
            var filePath = GetPhysicalPath(resourceUri);
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        override public byte[] GetResourceBytes(string resourceUri, string contentType)
        {
            var filePath = GetPhysicalPath(resourceUri);
            var fileContent = File.ReadAllBytes(filePath);
            return fileContent;
        }

        override public Stream GetResourceStream(string resourceUri, string contentType)
        {
            var filePath = GetPhysicalPath(resourceUri);
            return File.OpenRead(filePath);
        }

        override public IList<string> GetAvailableMediaTypes(string resourceUri, string method)
        {
            var filePath = GetPhysicalPath(resourceUri);
            var extension = Path.GetExtension(filePath);
            var mediaType = _serverConfig.GetMimeTypeForExtension(extension);
            return new List<string>(){ mediaType };
        }

        override public bool CreateOrUpdateResource(string resourceUri, string contentType, byte[] content)
        {
            var alreadyExists = CheckIfExists(resourceUri);
            var filePath = GetPhysicalPath(resourceUri);
            
            using(var fileStream = File.Create(filePath))
            {
                fileStream.Write(content, 0, content.Length);
            }

            return !alreadyExists;
        }

        override public bool DeleteResource(string resourceUri)
        {
            var filePath = GetPhysicalPath(resourceUri);
            
            File.Delete(filePath);

            return true;
        }
       
    }
}