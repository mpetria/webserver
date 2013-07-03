using System;
using System.Collections.Generic;
using System.Linq;

using WebServer.Config;
using WebServer.Data;
using WebServer.ResourceHandlers;
using WebServer.Utils;


namespace WebServer.Managers
{
    public class RequestManager : IRequestManager
    {
        private readonly ServerConfig _serverConfig;
        private readonly ILogger _logger;

        public RequestManager(ServerConfig serverConfig, ILogger logger)
        {
            _serverConfig = serverConfig;
            _logger = logger;
        }


        public Response ProceesRequest(Request request, bool processExpectation = false)
        {
            _logger.Log("Request", String.Format("Method: {0} Host: {1} UriPath: {2}", request.Method, request.Host, request.UriPath));

            var response = InnerProceesRequest(request, processExpectation);

            _logger.Log("Response", response.StatusCode.ToString());

            return response;
        }


        public Response InnerProceesRequest(Request request, bool processExpectation)
        {
            var response = new Response();
            bool returnResponse = false;

            returnResponse = ValidateRequest(request, response);
            if (returnResponse) return response;

            var handler = _serverConfig.GetHandlerForPath(request.Host, request.UriPath);

            returnResponse = CheckIfMethodIsAllowed(handler, request, response);
            if (returnResponse) return response;
            
            returnResponse = CheckIfResourceExits(handler, request, response);
            if (returnResponse) return response;

            returnResponse = CheckIfMediaTypeIsAllowed(handler, request, response);
            if (returnResponse) return response;

            returnResponse = HandleVersioning(handler, request, response);
            if (returnResponse) return response;


            if (!processExpectation) // do not process the body if processing an expectation
            {
                returnResponse = ProcessBody(handler, request, response);
                if (returnResponse) return response;
            }

            returnResponse = ProduceBody(handler, request, response);
            if (returnResponse) return response;
            
            response.StatusCode = HttpStatusCode.OK;

            return response;
        }

        private bool ProcessBody(IResourceHandler handler, Request request, Response response)
        {
            if(request.Method == HttpMethod.PUT)
            {
                var created = handler.CreateOrUpdateResource(request.UriPath, request.GetHeaderValue(HttpHeader.ContentType), request.Body);
                if(created)
                {
                    response.StatusCode = HttpStatusCode.Created;
                }
                else
                {
                    response.StatusCode = HttpStatusCode.OK;
                }
                return true;
            }
            return false;
        }

        public bool ValidateRequest(Request request, Response response)
        {
            if (!request.IsValid() || !_serverConfig.IsSupportedHost(request.Host))
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return true;
            }

            if(request.UriPath.Length > _serverConfig.MaxUriLength)
            {
                response.StatusCode = HttpStatusCode.RequestURITooLong;
                return true;
            }

            return false;
        }

        public bool CheckIfResourceExits(IResourceHandler handler, Request request, Response response)
        {
            if(request.Method == HttpMethod.HEAD || request.Method == HttpMethod.GET)
            {
                if (!handler.CheckIfExists(request.UriPath))
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    return true;
                }
            }

            return false;
        }

        public bool CheckIfMethodIsAllowed(IResourceHandler handler, Request request, Response response)
        {
            var allowedMethods = handler.GetAllowedMethods(request.UriPath);
            if (!allowedMethods.Contains(request.Method))
            {
                response.StatusCode = HttpStatusCode.MethodNotAllowed;
                response.Headers[HttpHeader.Allow] = allowedMethods.JoinWithSeparator(",");
                return true;
            }

            return false;
        }

        public bool CheckIfMediaTypeIsAllowed(IResourceHandler handler, Request request, Response response)
        {
            var allowedMediaTypes = handler.GetAllowedMediaTypes(request.UriPath, request.Method);
            var contentType = request.GetHeaderValue(HttpHeader.ContentType);
            if (contentType != null && !allowedMediaTypes.Contains(contentType))
            {
                response.StatusCode = HttpStatusCode.UnsupportedMediaType;
                return true;
            }

            return false;
        }

        public bool HandleVersioning(IResourceHandler handler, Request request, Response response)
        {
            string lastModifiedDate, eTag;

            handler.GetVersioning(request.UriPath, out lastModifiedDate, out eTag);

            if(lastModifiedDate != null)
                response.Headers.Add(HttpHeader.LastModified, lastModifiedDate);

            if(eTag != null)
                response.Headers.Add(HttpHeader.ETag, eTag);

            var headerIfModifiedSince = request.GetHeaderValue(HttpHeader.IfModifiedSince);
            if (headerIfModifiedSince != null && DateUtils.CheckIfHttpDatesMatch(lastModifiedDate, headerIfModifiedSince))
            {
                response.StatusCode = HttpStatusCode.NotModified;
                return true;
            }

            var headerIfUnodifiedSince = request.GetHeaderValue(HttpHeader.IfUnmodifiedSince);
            if (headerIfUnodifiedSince != null && DateUtils.CheckIfHttpDatesMatch(lastModifiedDate, headerIfUnodifiedSince))
            {
                response.StatusCode = HttpStatusCode.PreconditionFailed;
                return true;
            }

            return false;
        }

        public bool ProduceBody(IResourceHandler handler, Request request, Response response)
        {
            var availableMediaTypes = handler.GetAvailableMediaTypes(request.UriPath, request.Method);

            // check accept headers

            var chosenMediaType = availableMediaTypes.FirstOrDefault();
            response.ContentType = chosenMediaType;

            if (request.Method == HttpMethod.HEAD)
            {
                var length = handler.GetResourceLength(request.UriPath, chosenMediaType);
                response.Headers[HttpHeader.ContentLength] = length.ToString();
                response.SuppressBody = true;
            }
            else if (request.Method == HttpMethod.GET)
            {
                if(_serverConfig.UseStreams)
                {
                    var length = handler.GetResourceLength(request.UriPath, chosenMediaType);
                    response.Headers[HttpHeader.ContentLength] = length.ToString();
                    response.BodyStream = handler.GetResourceStream(request.UriPath, chosenMediaType);
                }
                else
                {
                    var fileContent = handler.GetResourceBytes(request.UriPath, chosenMediaType);
                    response.BodyBytes = fileContent;
                }
                
            }

            return false;
        }
    }
}
