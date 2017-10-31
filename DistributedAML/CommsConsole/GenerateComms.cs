using System;
using System.Collections.Generic;
using System.Text;

namespace CommsConsole
{
    public class GenerateComms
    {
        private GenerateCommsClient client;
        private GenerateCommsServer server;
        public GenerateComms(Type myType, List<Type> googleTypes, String directory) 
        {
            client = new GenerateCommsClient(myType,googleTypes,directory);
            server = new GenerateCommsServer(myType,googleTypes,directory);
        }

        public void GenerateFiles()
        {
            client.GenerateFile();
            server.GenerateFile();
        }



    }
}
