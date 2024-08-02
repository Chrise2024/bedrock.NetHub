using bedrock.NetHub.service;
using System;
using System.Diagnostics;

namespace bedrock.NetHub
{   
    internal abstract class Program
    {
        public static Logger stdhubLOGGER = new("stdhub");

        public static Logger BDSLOGGER = new("bds");

        private static Terminal TERMINAL = null;
        static void Main(string[] args)
        {
            TERMINAL = new(".\\bedrock_server.exe");
        }
    }
}