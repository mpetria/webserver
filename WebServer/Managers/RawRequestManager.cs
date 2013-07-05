using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

    public class SpecialBytes
    {
        public const byte CR = 13;           
        public const byte LF = 10;
        public const byte SP = 32;
        public const byte HT = 9;
    }

    public class RawRequestManager : IDataManager
    {
        private readonly ILogger _logger;
        private readonly Func<IRequestManager> _requestManagerFactory;
        private IDataManager _connectionManager;
        private byte[] _unprocessedBytes;

        private HttpParserState _httpParserState;
        private RawRequest _currentRequest;

        private bool _connectionIsClosing = false;

        public  RawRequestManager(ILogger logger, Func<IRequestManager> requestManagerFactory)
        {
            _logger = logger;
            _requestManagerFactory = requestManagerFactory;
            _unprocessedBytes = new byte[0];
            
            InitializeNewRequest();
        }


        public void Close()
        {
            _connectionIsClosing = true;
        }

        public void SetLinkedDataManager(IDataManager dataManager)
        {
            _connectionManager = dataManager;
        }


        public void ManageStream(Stream stream)
        {

            
        }


        public void ManageBytes(byte[] receivedBytes)
        {
            

            _unprocessedBytes = _unprocessedBytes.Concat(receivedBytes).ToArray();

            try
            {
                ParseAndExecuteRequest();
            }
            catch(Exception ex)
            {
                var internalServerError = RawResponse.BuildRawResponse(HttpStatusCode.InternalServerError);
                InitializeNewRequest();
                _connectionManager.ManageBytes(internalServerError.ResponseBytes);
            }
        }

        private void ParseAndExecuteRequest()
        {
            bool continueReading = true;
            while (continueReading)
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

        private bool DeliverRequest()
        {
            _connectionIsClosing = _currentRequest.CloseConnection;
            var requestManager = _requestManagerFactory();

            var request = RawRequest.BuildRequest(_currentRequest);
            var response = requestManager.ProceesRequest(request);

            SendResponseToConnectionManager(response);

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

            var response = requestManager.ProceesRequest(request, processExpectation : true);
            if (response.StatusCode.IsSuccessCode())
            {
                response = new Response() { StatusCode = HttpStatusCode.Continue };
            }

            SendResponseToConnectionManager(response);
            
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

        private void SendResponseToConnectionManager(Response response)
        {
            var rawResponse = RawResponse.BuildRawResponse(response);
            _connectionManager.ManageBytes(rawResponse.ResponseBytes);

            if (rawResponse.ResponseStream != null)
            {
                _connectionManager.ManageStream(rawResponse.ResponseStream);
            }
            if(_connectionIsClosing)
            {
                _connectionManager.Close();
            }
        }

        private bool ReadRequestLine()
        {
            byte[] lineBytes;
            if (!ReadLine(ref _unprocessedBytes, out lineBytes)) return false;

            _currentRequest.AddRequestLine(lineBytes);

            _httpParserState = HttpParserState.ReadRequestHeader;
            return true;
        }

        private bool ReadRequestHeader()
        {
            byte[] lineBytes;
            if (!ReadLine(ref _unprocessedBytes, out lineBytes)) return false;

            

            if (lineBytes.Length > 0)
            {
                _currentRequest.AddHeaderLine(lineBytes);
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

           
            if(_currentRequest.IsChunkedTransferEncoding)
            {
                if (!ReadChunkedBytes(ref _unprocessedBytes, out bodyBytes)) return false;
                
            }
            else if(_currentRequest.ContentLength.HasValue)
            {
                if (!ReadBytes(ref _unprocessedBytes, _currentRequest.ContentLength.Value, out bodyBytes)) return false;
            }

            _currentRequest.AddBody(bodyBytes);
            _httpParserState = HttpParserState.DeliverRequest;
            return true;
        }

        private static bool ReadChunkedBytes(ref byte[] unprocessedBytes, out byte[] bodyBytes)
        {
            bodyBytes = new byte[0];
            var currentUnprocessedBytes = unprocessedBytes;
            byte[] chunkBytes;
            do
            {

                byte[] lineBytes;
                if (!ReadLine(ref currentUnprocessedBytes, out lineBytes)) return false;
                var chunkSizeString = new ASCIIEncoding().GetString(lineBytes);
                int chunkSize = int.Parse(chunkSizeString, NumberStyles.HexNumber);

                if (!ReadBytes(ref currentUnprocessedBytes, chunkSize, out chunkBytes)) return false;
                bodyBytes = bodyBytes.Concat(chunkBytes).ToArray();

                // eat the ending CRLF
                if (!ReadLine(ref currentUnprocessedBytes, out lineBytes)) return false;
                if(lineBytes.Length > 0) throw new Exception("Bad Format");
            } while (chunkBytes.Length > 0);

            unprocessedBytes = currentUnprocessedBytes;
            return true;

        }

        private static bool ReadLine(ref byte[] unprocessedBytes, out byte[] lineBytes)
        {
            lineBytes = null;

            int lineLength = FindCRLF(unprocessedBytes);
            if (lineLength < 0)
                return false;
            
            lineBytes = unprocessedBytes.Take(lineLength).ToArray();

            unprocessedBytes = unprocessedBytes.Skip(lineLength + 2).ToArray();

            return true;
        }

        private static bool ReadBytes(ref byte[] unprocessedBytes, int numberOfBytes, out byte[] lineBytes)
        {
            lineBytes = null;

            if (numberOfBytes > unprocessedBytes.Length)
                return false;

            lineBytes = unprocessedBytes.Take(numberOfBytes).ToArray();

            unprocessedBytes = unprocessedBytes.Skip(numberOfBytes).ToArray();

            return true;
        }

        private static int FindCRLF(byte[] bytes)
        {
            for (var i = 0; i < bytes.Length - 1; i++)
            {
                if(bytes[i] == SpecialBytes.CR && bytes[i+1] == SpecialBytes.LF)
                {
                    // found line end
                    return i;
                }
            }
            return -1;
        }
    }
}
