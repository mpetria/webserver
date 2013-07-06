using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WebServer.Utils
{
    public class SpecialBytes
    {
        public const byte CR = 13;
        public const byte LF = 10;
        public const byte SP = 32;
        public const byte HT = 9;
    }


    public static class RequestParser
    {

        


        #region String Parsing

        static Regex _requestLineRegex = new Regex(@"(?<method>[^\s]+)\s+(?<uri>[^\s]+)\s+(?<version>[^\s]+)");
        static Regex _headerLineRegex = new Regex(@"(?<key>[^:]+):(?<value>.+)");

        public static bool ParseRequestLine(string requestLine, out string method, out string uri, out string version)
        {
            var match = _requestLineRegex.Match(requestLine);

            method = uri = version = null;
            if (!match.Success) return false;

            method = match.Groups["method"].Value;
            uri = match.Groups["uri"].Value;
            version = match.Groups["version"].Value;

            return true;

        }

        public static bool ParseHeaderLine(string headerLine, out string key, out string value)
        {
            var match = _headerLineRegex.Match(headerLine);

            key = value = null;
            if (!match.Success) return false;

            key = match.Groups["key"].Value.Trim();
            value = match.Groups["value"].Value.Trim();

            return true;

        }

        #endregion


        #region Byte Parsing

        public static bool ReadChunkedBytes(ref byte[] unprocessedBytes, out byte[] bodyBytes, out IList<byte[]> footerLinesBytes)
        {
            bodyBytes = new byte[0];
            footerLinesBytes = new List<byte[]>();

            var currentUnprocessedBytes = unprocessedBytes;
            int chunkSize;
            do
            {
                byte[] lineBytes;
                // read the chunk info
                if (!ReadLine(ref currentUnprocessedBytes, out lineBytes)) return false;
                var chunkSizeString = new ASCIIEncoding().GetString(lineBytes);

                // ignore extension
                chunkSizeString = chunkSizeString.SubstringBefore(';');

                chunkSize = int.Parse(chunkSizeString, NumberStyles.HexNumber);

                if (chunkSize > 0)
                {
                    byte[] chunkBytes;
                    if (!ReadBytes(ref currentUnprocessedBytes, chunkSize, out chunkBytes)) return false;
                    bodyBytes = bodyBytes.Concat(chunkBytes).ToArray();
                    
                    // read CRLF
                    if (!ReadLine(ref currentUnprocessedBytes, out lineBytes)) return false;
                }
            } while (chunkSize > 0);

            byte[] footerLineBytes;
            do
            {
                if (!ReadLine(ref currentUnprocessedBytes, out footerLineBytes)) return false;
                if (footerLineBytes.Length > 0)
                {
                    footerLinesBytes.Add(footerLineBytes);
                }
            } while (footerLineBytes.Length > 0);

            unprocessedBytes = currentUnprocessedBytes;
            return true;

        }

        public static bool ReadLine(ref byte[] unprocessedBytes, out byte[] lineBytes)
        {
            lineBytes = null;

            int lineLength = FindCRLF(unprocessedBytes);
            if (lineLength < 0)
                return false;

            lineBytes = unprocessedBytes.Take(lineLength).ToArray();

            unprocessedBytes = unprocessedBytes.Skip(lineLength + 2).ToArray();

            return true;
        }

        public static bool ReadBytes(ref byte[] unprocessedBytes, int numberOfBytes, out byte[] lineBytes)
        {
            lineBytes = null;

            if (numberOfBytes > unprocessedBytes.Length)
                return false;

            lineBytes = unprocessedBytes.Take(numberOfBytes).ToArray();

            unprocessedBytes = unprocessedBytes.Skip(numberOfBytes).ToArray();

            return true;
        }

        public static int FindCRLF(byte[] bytes)
        {
            for (var i = 0; i < bytes.Length - 1; i++)
            {
                if (bytes[i] == SpecialBytes.CR && bytes[i + 1] == SpecialBytes.LF)
                {
                    // found line end
                    return i;
                }
            }
            return -1;
        }

     

        #endregion
    }
}
