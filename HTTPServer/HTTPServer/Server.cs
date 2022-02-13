using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            serverSocket.Bind(hostEndPoint);
        }

        public void StartServer()
        {
            // Listen to connections, with large backlog.
            serverSocket.Listen(1000);
            Console.WriteLine("Listening.....");
            // Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
               // accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("New client accepted: {0}", clientSocket.RemoteEndPoint);
                Thread newthread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newthread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            byte[] data;
            int receivedLength;
            // Create client socket 
            Socket clientSocket = (Socket)obj;
            //clientSocket.Send(Encoding.ASCII.GetBytes("please Enter your request:\n"));
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    data = new byte[1024];
                    receivedLength = clientSocket.Receive(data);

                    // TODO: break the while loop if receivedLen==0
                    if (receivedLength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSocket.RemoteEndPoint);
                        break;
                    }

                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(data,0,receivedLength));
                    // TODO: Call HandleRequest Method that returns the response
                    Response response= this.HandleRequest(request);
                    // TODO: Send Response back to client
                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));

                }
                catch (Exception ex)
                {
                    // log exception using Logger class
                    Logger.LogException(ex);
                }
               
            }
            // TODO: close client socket

            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            Response response;
            string content,filePath;
            try
            {
              //  throw new NotImplementedException();
                // check for bad request
                if (!request.ParseRequest())
                {
                    content = LoadDefaultPage("BadRequest.html");
                    response = new Response(StatusCode.BadRequest, "text/html", content, null, request.httpVersion);
                    return response;
                }

                // map the relativeURI in request to get the physical path of the resource.
                filePath = Path.Combine(Configuration.RootPath, request.relativeURI);
                //  check for redirect
                if (Configuration.RedirectionRules.ContainsKey(request.relativeURI))
                { 
                    string redirectPath = GetRedirectionPagePathIFExist(request.relativeURI);
                    content = LoadDefaultPage("Redirect.html");
                    response = new Response(StatusCode.Redirect, "text/html", content, redirectPath, request.httpVersion);
                    return response;
                }

                // check file exists
                // read the physical file
                // Create OK response
                if (File.Exists(filePath))
                {
                    content = LoadDefaultPage(filePath);
                    response = new Response(StatusCode.OK, "text/html", content, null, request.httpVersion);
                    return response;
                }
                else
                {
                    content = LoadDefaultPage("NotFound.html");
                    response = new Response(StatusCode.NotFound, "text/html", content, null, request.httpVersion);
                    return response;
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                // TODO: in case of exception, return Internal Server Error.
                content = LoadDefaultPage("InternalError.html");
                response = new Response(StatusCode.InternalServerError, "text/html", content, null, request.httpVersion);
                Logger.LogException(ex);
                return response;
            }
          
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            
            return Configuration.RedirectionRules[relativePath];
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            // else read file and return its content
            try
            {
                return File.ReadAllText(filePath);
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                     
                Configuration.RedirectionRules  = new Dictionary<string, string>();
                foreach (string rule in File.ReadLines(filePath))
                {
                    string[] URLs = rule.Split(',');
                    Configuration.RedirectionRules[URLs[0]] = URLs[1];
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
