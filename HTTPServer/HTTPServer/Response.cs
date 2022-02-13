using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }

       // StatusCode code;
        List<string> headerLines = new List<string>();
        string headersStr;
        public Response(StatusCode code, string contentType, string content, string redirectoinPath,
                        HTTPVersion httpVersion)
        {
           // Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
          
            DateTime currentDate = DateTime.Now;
            string dateHeader = "Date: " + currentDate.ToString("ddd, dd MMMM yyy HH':'mm':'ss' EST'");
            string contentTypeHeader = "Content-Type: "+contentType;
            string contentLengthHeader = "Content-Length: " + content.Length;
            headerLines.Add(dateHeader);
            headerLines.Add(contentTypeHeader);
            headerLines.Add(contentLengthHeader);

            if (redirectoinPath != null)
            {
                string redirectionalHeader = "location: " + redirectoinPath;
                headerLines.Add(redirectionalHeader);
            }
            foreach(string header in headerLines)
            {
                headersStr += header+"\r\n";
            }
            // TODO: Create the response string
             responseString = GetStatusLine(code, httpVersion) + "\r\n" + headersStr + "\r\n" + content;

        }

        private string GetStatusLine(StatusCode code, HTTPVersion httpVersion)
        {
            // TODO: Create the response status line and return it
            
            string statusLine = string.Empty , version;
            if (httpVersion == HTTPVersion.HTTP09)
                version = "HTTP/0.9";
            else if (httpVersion == HTTPVersion.HTTP10)
                version = "HTTP/1.0";
            else
                version = "HTTP/1.1";
            statusLine = version + " " + ((int)code) + " " + code.ToString();

            return statusLine;
        }
    }
}
