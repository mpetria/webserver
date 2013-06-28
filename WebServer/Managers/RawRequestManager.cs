using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Config;
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

    public class RawRequestManager
    {
        private readonly Action<byte[]> SendBytesToClient;
        private byte[] _unprocessedBytes;

        private HttpParserState _httpParserState;
        private RawRequest _currentRequest;

        public  RawRequestManager(Action<byte[]> sendBytesToClient)
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
                var internalServerError = RawResponse.BuildRawResponse(ResponseStatusCode.InternalServerError);
                InitializeNewRequest();
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

            var requestManager = new RequestManager();
            var rawResponse = requestManager.ProceesRequest(_currentRequest);

            SendBytesToClient(rawResponse.ResponseBytes);
            if(rawResponse.ResponseStream != null)
            {
                while (true)
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;

                    try
                    {
                        //blocks until a client sends a message
                        bytesRead = rawResponse.ResponseStream.Read(buffer, 0, 4096);
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

                    var message = buffer.Take(bytesRead).ToArray();
                    SendBytesToClient(message);
                }
            }

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
