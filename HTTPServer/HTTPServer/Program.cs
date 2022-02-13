using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        
        static string path = @"RedirectionRulesFile";

        static void Main(string[] args)
        {
            CreateRedirectionRulesFile();
            //Start server
            // 1) Make server object on port 1000
             Server server = new Server(1000, path);
            // 2) Start Server
            server.StartServer();


        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            
            FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

            string text = "aboutus.html,aboutus2.html";
            byte[] writeArr = Encoding.UTF8.GetBytes(text);

            file.Write(writeArr, 0,text.Length);
            file.Close();
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
        }
         
    }
}
