﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebServer.Config;

namespace WebServer.ResourceHandlers
{
    public class FsResourceHandler : IResourceHandler
    {

        private readonly string _directory;


        public FsResourceHandler(string directory)
        {
            _directory = directory;
        }

        public virtual bool CheckIfExists(string resourceUri)
        {
            return false;
        }

        public virtual IList<string> GetAllowedMethods(string resourceUri)
        {
            return new List<string>();
        }

        public virtual IList<string> GetAllowedMediaTypes(string resourceUri, string method)
        {
            return new List<string>();
        }

        public virtual bool GetVersioning(string resourceUri, out string lastUpdateDate, out string eTag)
        {
            lastUpdateDate = eTag = null;
            return false;
        }

        public virtual long GetResourceLength(string resourceUri, string contentType)
        {
            return GetResourceBytes(resourceUri, contentType).Length;
        }

        public virtual byte[] GetResourceBytes(string resourceUri, string contentType)
        {
            return new byte[0];
        }

        public virtual Stream GetResourceStream(string resourceUri, string contentType)
        {
            return null;
        }

        public virtual IList<string> GetAvailableMediaTypes(string resourceUri, string method)
        {
            return new List<string>();
        }

        public virtual bool CreateOrUpdateResource(string resourceUri, string contentType, byte[] content)
        {
            return false;
        }

        public virtual bool DeleteResource(string resourceUri)
        {
            return false;
        }

        public virtual string AlterResource(string resourceUri, string contentType, byte[] content)
        {
            return null;
        }

        public string GetPhysicalPath(string requestUri)
        {
            var queryPosition = requestUri.IndexOf("?");
            if(queryPosition != -1)
            {
                requestUri = requestUri.Substring(0, queryPosition);
            }

            var filePath = Path.Combine(_directory, requestUri.Trim("/".ToCharArray()));
            return filePath;
        }

        public string GetResourceUri(string physicalPath)
        {
            var resourceUri = String.Empty;
            if (physicalPath.StartsWith(_directory))
                resourceUri = physicalPath.Substring(_directory.Length);

            return resourceUri.Replace(@"\",@"/");
        }
    }
}
