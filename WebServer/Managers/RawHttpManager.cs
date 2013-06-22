using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Entities;
using WebServer.Handlers;

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

    public class RawHttpManager
    {
        private readonly Action<byte[]> SendBytesToClient;
        private byte[] _unprocessedBytes;

        private HttpParserState _httpParserState;
        private RawRequest _currentRequest;

        public  RawHttpManager(Action<byte[]> sendBytesToClient)
        {
            SendBytesToClient = sendBytesToClient;
            _unprocessedBytes = new byte[0];
            
            InitializeNewRequest();
            
        }




        public void ProcessBytes(byte[] receivedBytes)
        {
            _unprocessedBytes = _unprocessedBytes.Concat(receivedBytes).ToArray();

            try
            {
                ParseAndExecuteRequest();
            }
            catch(Exception ex)
            {
                var internalServerError = RawResponse.BuildRawResponse(ResponseStatusCode.INTERNAL_SERVER_ERROR);
                SendBytesToClient(internalServerError.ResponseBytes);
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
            var handler = new StaticAssetsHandler(@"C:\Work\Playground\TestSite");
            var request = RawRequest.BuildRequest(_currentRequest);
            var response = new Response();

            handler.HandleRequest(request, response);

            var rawResponse = RawResponse.BuildRawResponse(response);
            
            SendBytesToClient(rawResponse.ResponseBytes);
            _httpParserState = HttpParserState.RequestDelivered;
            return true;

        }

        private bool ReadRequestLine()
        {
            byte[] lineBytes;
            if (!ReadLine(out lineBytes)) return false;

            _currentRequest.AddRequestLine(lineBytes);

            _httpParserState = HttpParserState.ExpectRequestHeader;
            return true;
        }

        private bool ReadRequestHeader()
        {
            byte[] lineBytes;
            if (!ReadLine(out lineBytes)) return false;

            _currentRequest.AddHeaderLine(lineBytes);

            if (lineBytes.Length == 0)
            {
                _httpParserState = HttpParserState.ExpectRequestBody;
            }

            return true;
        }

        private bool ReadRequestBody()
        {
            byte[] lineBytes;
            if (!ReadBytes(_currentRequest.ContentLength, out lineBytes)) return false;

            _currentRequest.AddBody(lineBytes);
            _httpParserState = HttpParserState.RequestReceived;
            return true;
        }

        private bool ReadLine(out byte[] lineBytes)
        {
            lineBytes = null;

            int lineLength = FindCRLF(_unprocessedBytes);
            if (lineLength < 0)
                return false;
            
            lineBytes = _unprocessedBytes.Take(lineLength).ToArray();

            _unprocessedBytes = _unprocessedBytes.Skip(lineLength + 2).ToArray();

            return true;
        }

        private bool ReadBytes(int numberOfBytes, out byte[] lineBytes)
        {
            lineBytes = null;

            if (numberOfBytes > _unprocessedBytes.Length)
                return false;

            lineBytes = _unprocessedBytes.Take(numberOfBytes).ToArray();

            _unprocessedBytes = _unprocessedBytes.Skip(numberOfBytes).ToArray();

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
