using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using WebServer.Config;
using WebServer.Data;
using WebServer.Utils;


namespace WebServer.Managers
{
    public enum HttpParserState
    {
        ReadRequestLine,
        ReadRequestHeader,
        ReadRequestBody,
        DeliverRequestExpectation,
        DeliverRequest,
        RequestCompleted
    }

    public enum HttpConnectionState
    {
        ConnectionUnitialized,
        ConnectionOpened,
        ConnectionClosed
    }

   

    public class ConnectionManager : IConnectionManager
    {
        private readonly ServerConfig _serverConfig;
        private readonly ILogger _logger;
        private readonly Func<IRequestManager> _requestManagerFactory;
        private readonly string _connectionId;
        private byte[] _unprocessedBytes;

        private HttpParserState _httpParserState;
        private HttpConnectionState _httpConnectionState;

        private RawRequest _currentRequest;
        private Stream _clientStream;

        public  ConnectionManager(ServerConfig serverConfig, ILogger logger, Func<IRequestManager> requestManagerFactory, string connectionId)
        {
            _serverConfig = serverConfig;
            _logger = logger;
            _requestManagerFactory = requestManagerFactory;
            _connectionId = connectionId;
            _unprocessedBytes = new byte[0];
            _httpConnectionState = HttpConnectionState.ConnectionUnitialized;
        }

        #region IConnectionManager members

        public void ProcessStream(Stream clientStream)
        {
            _clientStream = clientStream;
            _httpConnectionState = HttpConnectionState.ConnectionOpened;

            InitializeNewRequest();
            ReceiveBytesFromClient();
        }


        public void Close()
        {
            _httpConnectionState = HttpConnectionState.ConnectionClosed;
        }

        public string ConnectionId 
        { 
            get
            {
                return _connectionId;

            } 
        }

        #endregion

        #region Cient Stream Processing
        private void ReceiveBytesFromClient()
        {
            byte[] buffer = new byte[4096];

            while (_httpConnectionState == HttpConnectionState.ConnectionOpened)
            {
                int bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = _clientStream.Read(buffer, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                var bytes = buffer.Take(bytesRead).ToArray();
                //message has successfully been received
                _logger.Log("Bytes Received", bytes);

                ManageBytes(bytes);

                _logger.Log("Bytes Processed");
            }
        }


        private void SendBytesToClient(byte[] responseBytes)
        {
            _logger.Log("Bytes Sent", responseBytes);
            _clientStream.Write(responseBytes, 0, responseBytes.Length);
            _clientStream.Flush();

        }

        private void SendStreamToClient(Stream stream)
        {
            if (stream != null)
                stream.CopyTo(_clientStream, 4096);
        }

        #endregion

        #region Request Processing


        private void ManageBytes(byte[] receivedBytes)
        {
            _unprocessedBytes = _unprocessedBytes.Concat(receivedBytes).ToArray();

            try
            {
                ParseAndExecuteRequest();
            }
            catch (Exception ex)
            {
                InitializeNewRequest();
                SendResponseToClient(new Response() { StatusCode = HttpStatusCode.InternalServerError });
            }
        }

        private void ParseAndExecuteRequest()
        {
            bool continueReading = true;
            while (continueReading && _httpConnectionState == HttpConnectionState.ConnectionOpened)
            {
                switch (_httpParserState)
                {
                    case HttpParserState.ReadRequestLine:
                        continueReading = ReadRequestLine();
                        break;
                    case HttpParserState.ReadRequestHeader:
                        continueReading = ReadRequestHeader();
                        break;
                    case HttpParserState.ReadRequestBody:
                        continueReading = ReadRequestBody();
                        break;
                    case HttpParserState.DeliverRequestExpectation:
                        continueReading = DeliverRequestExpectation();
                        break;
                    case HttpParserState.DeliverRequest:
                        continueReading = DeliverRequest();
                        break;
                    case HttpParserState.RequestCompleted:
                        continueReading = InitializeNewRequest();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private bool InitializeNewRequest()
        {
            _currentRequest = new RawRequest();
            _httpParserState = HttpParserState.ReadRequestLine;
            return true;
        }

        private bool ProcessRequestHeader(byte[] headerLine)
        {
            string key, value;
            _currentRequest.AddHeaderLine(headerLine, out key, out value);

            bool isImplemented = true;

            // A server which receives an entity-body with a transfer-coding it does not understand SHOULD return 501 (Unimplemented), and close the connection.
            if(key == HttpHeader.TransferEncoding)
            {
                isImplemented = _serverConfig.IsSupportedTransferEncoding(value);
            }
            else if(key == HttpHeader.ContentEncoding)
            {
                isImplemented = _serverConfig.IsSupportedContentEncoding(value);
            }

            if(!isImplemented)
            {
                var response = new Response() {StatusCode = HttpStatusCode.NotImplemented};
                SendResponseToClient(response);
                Close();
                return false;
            }

            return true;
        }

        private bool DeliverRequest()
        {
            var requestManager = _requestManagerFactory();

            var request = RawRequest.BuildRequest(_currentRequest);
            var response = requestManager.ProceesRequest(request);

            SendResponseToClient(response);

            if(_currentRequest.CloseConnection)
            {
                Close();
                return false;
            }

            _httpParserState = HttpParserState.RequestCompleted;
            return true;

        }

        private bool DeliverRequestExpectation()
        {
            var request = RawRequest.BuildRequest(_currentRequest);
            
            // Deal with request expectation only for HTTP/1.1 clients
            if(request.Version != HttpVersion.HTTP_1_1)
            {
                _httpParserState = HttpParserState.ReadRequestBody;
                return true;
            }

            var requestManager = _requestManagerFactory();

            var response = requestManager.ProceesRequest(request, processBody : false);
            if (response.StatusCode.IsSuccessCode())
            {
                response = new Response() { StatusCode = HttpStatusCode.Continue };
            }

            SendResponseToClient(response);
            
            if (response.StatusCode == HttpStatusCode.Continue)
            {
                _httpParserState = HttpParserState.ReadRequestBody;
            }
            else
            {
                _httpParserState = HttpParserState.RequestCompleted;
            }
            
            return true;

        }

        private void SendResponseToClient(Response response)
        {
            var rawResponse = RawResponse.BuildRawResponse(response);
            SendBytesToClient(rawResponse.ResponseBytes);

            if (rawResponse.ResponseStream != null)
            {
                SendStreamToClient(rawResponse.ResponseStream);
            }
        }
        #endregion

        #region Request Parsing

        private bool ReadRequestLine()
        {
            byte[] lineBytes;
            if (!RequestParser.ReadLine(ref _unprocessedBytes, out lineBytes)) return false;

            _currentRequest.AddRequestLine(lineBytes);

            _httpParserState = HttpParserState.ReadRequestHeader;
            return true;
        }

        private bool ReadRequestHeader()
        {
            byte[] lineBytes;
            if (!RequestParser.ReadLine(ref _unprocessedBytes, out lineBytes)) return false;

            

            if (lineBytes.Length > 0)
            {
                if (!ProcessRequestHeader(lineBytes)) return false;
            }
            else
            {

                if (_currentRequest.Expects100Continue)
                {
                    _httpParserState = HttpParserState.DeliverRequestExpectation;
                }
                else
                {
                    _httpParserState = HttpParserState.ReadRequestBody;
                }
            }

            return true;
        }

        private bool ReadRequestBody()
        {
            byte[] bodyBytes=null;
           
            // All HTTP/1.1 applications MUST be able to receive and decode the "chunked" transfer-coding, and MUST ignore chunk-extension extensions they do not understand. 
            if(_currentRequest.IsChunkedTransferEncoding)
            {
                IList<byte[]> footers;
                if (!RequestParser.ReadChunkedBytes(ref _unprocessedBytes, out bodyBytes, out footers)) return false;
                for (int i = 0; i < footers.Count; i++)
                {
                    if (!ProcessRequestHeader(footers[i])) return false;
                }
            }
            else if(_currentRequest.ContentLength.HasValue)
            {
                if (!RequestParser.ReadBytes(ref _unprocessedBytes, _currentRequest.ContentLength.Value, out bodyBytes)) return false;
            }

            _currentRequest.AddBody(bodyBytes);
            _httpParserState = HttpParserState.DeliverRequest;
            return true;
        }

        

        #endregion

    }
}
