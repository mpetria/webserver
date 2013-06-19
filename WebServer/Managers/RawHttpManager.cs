using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebServer.Entities;

namespace WebServer.Managers
{
    public enum HttpParserState
    {
        ExpectRequestLine,
        ExpectRequestHeader,
        ExpectRequestBody,
        RequestReceived
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
        private readonly Action<byte[]> _pushBytes;
        private byte[] _unprocessedBytes;

        private HttpParserState _httpParserState;
        private RawRequest _currentRequest;

        public  RawHttpManager(Action<byte[]> pushBytes)
        {
            _pushBytes = pushBytes;
            _unprocessedBytes = new byte[0];
            
            InitializeNewRequest();
            
        }




        public void ProcessBytes(byte[] receivedBytes)
        {
            _unprocessedBytes = _unprocessedBytes.Concat(receivedBytes).ToArray();

            bool continueReading = true;
            while(continueReading)
            {
                switch (_httpParserState)
                {
                    case HttpParserState.ExpectRequestLine:
                        continueReading = TryReadRequestLine();
                        break;
                    case HttpParserState.ExpectRequestHeader:
                        continueReading = TryReadRequestHeader();
                        break;
                    case HttpParserState.ExpectRequestBody:
                        continueReading = TryReadRequestBody();
                        break;
                    case HttpParserState.RequestReceived:
                        continueReading = TryToDeliverRequest();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void InitializeNewRequest()
        {
            _httpParserState = HttpParserState.ExpectRequestLine;
            _currentRequest = new RawRequest();
        }


        private const string RESPONSE_NOT_FOUND = "HTTP/1.0 404 Not Found";

        private const string SAMPLE_TEXT_RESPONSE =
            @"HTTP/1.1 200 OK
Date: Fri, 31 Dec 1999 23:59:59 GMT
Content-Type: text/plain
Content-Length: 11

Hello World";


        private bool TryToDeliverRequest()
        {
            System.Diagnostics.Debug.WriteLine("Deliver Request");
            System.Diagnostics.Debug.WriteLine(_currentRequest.RequestLine);

            //message has successfully been received
            ASCIIEncoding encoder = new ASCIIEncoding();

            byte[] responseBytes = encoder.GetBytes(SAMPLE_TEXT_RESPONSE);
            _pushBytes(responseBytes);

            InitializeNewRequest();
            return true;

        }

        private bool TryReadRequestLine()
        {
            byte[] lineBytes;
            if (!TryReadLine(out lineBytes)) return false;

            _currentRequest.AddRequestLine(lineBytes);

            _httpParserState = HttpParserState.ExpectRequestHeader;
            return true;
        }

        private bool TryReadRequestHeader()
        {
            byte[] lineBytes;
            if (!TryReadLine(out lineBytes)) return false;

            _currentRequest.AddHeaderLine(lineBytes);

            if (lineBytes.Length == 0)
            {
                _httpParserState = HttpParserState.ExpectRequestBody;
            }

            return true;
        }

        private bool TryReadRequestBody()
        {
            byte[] lineBytes;
            if (!TryReadBytes(_currentRequest.ContentLength, out lineBytes)) return false;

            _currentRequest.AddBody(lineBytes);
            _httpParserState = HttpParserState.RequestReceived;
            return true;
        }

        private bool TryReadLine(out byte[] lineBytes)
        {
            lineBytes = null;

            int lineLength = FindCRLF(_unprocessedBytes);
            if (lineLength < 0)
                return false;
            
            lineBytes = _unprocessedBytes.Take(lineLength).ToArray();

            _unprocessedBytes = _unprocessedBytes.Skip(lineLength + 2).ToArray();

            return true;
        }

        private bool TryReadBytes(int numberOfBytes, out byte[] lineBytes)
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
