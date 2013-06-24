﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Config;
using WebServer.Entities;
using WebServer.Handlers;
using WebServer.Utils.Logging;

namespace WebServer.Managers
{
    public class RequestManager
    {
        private readonly ILogger _logger;
        private readonly string _requestId;

        public RequestManager()
        {
            _requestId = Guid.NewGuid().ToString();
            _logger = new RequestLogger("connection", _requestId);
        }

        public RawResponse ProceesRequest(RawRequest rawRequest)
        {
            _logger.Log("New Request", rawRequest.RequestLine);

            var handler = new StaticAssetsHandler(ServerConfig.Instance.RootDirectory);
            var request = RawRequest.BuildRequest(rawRequest);
            var response = new Response();

            

            handler.HandleRequest(request, response);

            var rawResponse = RawResponse.BuildRawResponse(response);

            _logger.Log("New Response", response.StatusCode.ToString());

            return rawResponse;
        }
    }
}