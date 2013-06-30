using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Config;
using WebServer.Entities;
using WebServer.Handlers;
using WebServer.Utils;
using WebServer.Utils.Logging;

namespace WebServer.Managers
{
    public class RequestManager
    {
        private readonly ILogger _logger;
        private readonly string _requestId;
        private readonly ServerConfig _serverConfig;

        public RequestManager()
        {
            _requestId = Guid.NewGuid().ToString();
            _logger = new RequestLogger("connection", _requestId);
            _serverConfig = ServerConfig.Instance;
        }

        public RawResponse ProceesRequestExpectation(RawRequest rawRequest, out bool shouldContinue)
        {
            _logger.Log("New Request Expectation", rawRequest.RequestLine);

            var request = RawRequest.BuildRequest(rawRequest);
            var response = InnerProceesRequest(request, processBody: false);
            if(response.StatusCode.IsSuccessCode())
            {
                response = new Response() {StatusCode = HttpStatusCode.Continue};
                shouldContinue = true;

            }
            else
            {
                shouldContinue = false;
            }

            var rawResponse = RawResponse.BuildRawResponse(response);

            _logger.Log("New Response Expectation", response.StatusCode.ToString());

            return rawResponse;
        }

        public RawResponse ProceesRequest(RawRequest rawRequest)
        {
            _logger.Log("New Request", rawRequest.RequestLine);

            var request = RawRequest.BuildRequest(rawRequest);
            var response = InnerProceesRequest(request);
            var rawResponse = RawResponse.BuildRawResponse(response);

            _logger.Log("New Response", response.StatusCode.ToString());

            return rawResponse;
        }

        private Response InnerProceesRequest(Request request, bool processBody = true)
        {
            var response = new Response();
            bool returnResponse = false;


            returnResponse = ValidateRequest(request, response);
            if (returnResponse) return response;

            var handler = _serverConfig.GetHandlerForPath(request.Uri);

            returnResponse = CheckIfMethodIsAllowed(handler, request, response);
            if (returnResponse) return response;
            
            returnResponse = CheckIfResourceExits(handler, request, response);
            if (returnResponse) return response;

            returnResponse = CheckIfMediaTypeIsAllowed(handler, request, response);
            if (returnResponse) return response;

            returnResponse = HandleVersioning(handler, request, response);
            if (returnResponse) return response;


            if (processBody)
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
                var created = handler.CreateOrUpdateResource(request.Uri, request.GetHeaderValue(HttpHeader.ContentType), request.Body);
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
            if (!request.IsValid())
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                return true;
            }

            if(request.Uri.Length > _serverConfig.MaxUriLength)
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
                if (!handler.CheckIfExists(request.Uri))
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    return true;
                }
            }

            return false;
        }

        public bool CheckIfMethodIsAllowed(IResourceHandler handler, Request request, Response response)
        {
            var allowedMethods = handler.GetAllowedMethods(request.Uri);
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
            var allowedMediaTypes = handler.GetAllowedMediaTypes(request.Uri, request.Method);
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

            handler.GetVersioning(request.Uri, out lastModifiedDate, out eTag);
            response.Headers.Add(HttpHeader.LastModified, lastModifiedDate);
            response.Headers.Add(HttpHeader.ETag, eTag);

            var headerIfModifiedSince = request.GetHeaderValue(HttpHeader.IfModifiedSince);
            if (headerIfModifiedSince != null && lastModifiedDate == headerIfModifiedSince)
            {
                response.StatusCode = HttpStatusCode.NotModified;
                return true;
            }

            return false;
        }

        public bool ProduceBody(IResourceHandler handler, Request request, Response response)
        {
            var availableMediaTypes = handler.GetAvailableMediaTypes(request.Uri, request.Method);

            // check accept headers

            var chosenMediaType = availableMediaTypes.FirstOrDefault();
            response.ContentType = chosenMediaType;

            if (request.Method == HttpMethod.HEAD)
            {
                var length = handler.GetResourceLength(request.Uri, chosenMediaType);
                response.Headers[HttpHeader.ContentLength] = length.ToString();
                response.SuppressBody = true;
            }
            else if (request.Method == HttpMethod.GET)
            {
                if(_serverConfig.UseStreams)
                {
                    var length = handler.GetResourceLength(request.Uri, chosenMediaType);
                    response.Headers[HttpHeader.ContentLength] = length.ToString();
                    response.BodyStream = handler.GetResourceStream(request.Uri, chosenMediaType);
                }
                else
                {
                    var fileContent = handler.GetResourceBytes(request.Uri, chosenMediaType);
                    response.BodyBytes = fileContent;
                }
                
            }

            return false;
        }
    }
}
