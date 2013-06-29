using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using WebServer.Config;
using WebServer.Entities;
using WebServer.Handlers;
using WebServer.Utils.Logging;

namespace WebServer.Managers
{
    public enum HttpParserState
    {
        ExpectRequestLine,
        ExpectRequestHeader,
        ExpectRequestBody,
        RequestReceived,
        RequestDelivered
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
        private IDataManager _responseManager;
        private byte[] _unprocessedBytes;

        private HttpParserState _httpParserState;
        private RawRequest _currentRequest;

        public  RawRequestManager(ILogger logger)
        {
            _logger = logger;
            _unprocessedBytes = new byte[0];
            
            InitializeNewRequest();
            
        }


        public void SetLinkedDataManager(IDataManager dataManager)
        {
            _responseManager = dataManager;
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
                var internalServerError = RawResponse.BuildRawResponse(ResponseStatusCode.InternalServerError);
                InitializeNewRequest();
                _responseManager.ManageBytes(internalServerError.ResponseBytes);
            }
        }

        private void ParseAndExecuteRequest()
        {
            bool continueReading = true;
            while (continueReading)
            {
                switch (_httpParserState)
                {
                    case HttpParserState.ExpectRequestLine:
                        continueReading = ReadRequestLine();
                        break;
                    case HttpParserState.ExpectRequestHeader:
                        continueReading = ReadRequestHeader();
                        break;
                    case HttpParserState.ExpectRequestBody:
                        continueReading = ReadRequestBody();
                        break;
                    case HttpParserState.RequestReceived:
                        continueReading = DeliverRequestToHandler();
                        break;
                    case HttpParserState.RequestDelivered:
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
            _httpParserState = HttpParserState.ExpectRequestLine;
            return true;
        }

        private bool DeliverRequestToHandler()
        {

            var requestManager = new RequestManager();
            var rawResponse = requestManager.ProceesRequest(_currentRequest);

            
            
            _responseManager.ManageBytes(rawResponse.ResponseBytes);
            if(rawResponse.ResponseStream != null)
            {
              _responseManager.ManageStream(rawResponse.ResponseStream);
            }

            _httpParserState = HttpParserState.RequestDelivered;
            return true;

        }

        private bool ReadRequestLine()
        {
            byte[] lineBytes;
            if (!ReadLine(ref _unprocessedBytes, out lineBytes)) return false;

            _currentRequest.AddRequestLine(lineBytes);

            _httpParserState = HttpParserState.ExpectRequestHeader;
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
                _httpParserState = HttpParserState.ExpectRequestBody;
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
            _httpParserState = HttpParserState.RequestReceived;
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
