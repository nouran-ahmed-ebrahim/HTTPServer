using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        string requestString;
        string[] contentLines;
        public string relativeURI;
        public HTTPVersion httpVersion;
        Dictionary<string, string> headerLines = new Dictionary<string, string>();

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //TODO: parse the receivedRequest using the \r\n delimeter
            string[] seperator = { "\r\n" };
            requestLines = requestString.Split(seperator, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (requestLines.Length < 3) return false;

            // Validate blank line exists
            // Load header lines into HeaderLines dictionary
            // Parse Request line
            return ValidateBlankLine() && LoadHeaderLines() && ParseRequestLine() && ValidateIsURI(relativeURI);
        }

        private bool ParseRequestLine()
        {
            string[] seperator = { " " };
            contentLines = requestLines[0].Split(seperator, StringSplitOptions.RemoveEmptyEntries);

            if (contentLines.Length > 3 || contentLines.Length <2)
                return false;

            if (contentLines[0] == "GET")
                method = RequestMethod.GET;
            else if (contentLines[0] == "POST")
                method = RequestMethod.POST;
            else
                method = RequestMethod.HEAD;

            relativeURI = contentLines[1];
            relativeURI = relativeURI.Substring(1);
            // return ValidateIsURI( uri)

            if (contentLines[2] == "HTTP/1.0")
                httpVersion = HTTPVersion.HTTP10;
            else if (contentLines[2] == "HTTP/1.1")
                httpVersion = HTTPVersion.HTTP11;
            else
                httpVersion = HTTPVersion.HTTP09;

            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            if (requestLines[1] == "\r\n") // if it is empty
                return false;

            if (method == RequestMethod.GET)
            {
                string[] headerInfo = new string[2];
                for (int i = 1; i < requestLines.Length - 2; i++)
                {
                    string[] seperator = { ": " };
                    headerInfo = requestLines[i].Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                    HeaderLines[headerInfo[0]] = headerInfo[1];
                }
            }

            return true;
        }

        private bool ValidateBlankLine()
        {
            if (method == RequestMethod.GET)
            {
                if (requestLines[requestLines.Length - 1] == "")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
