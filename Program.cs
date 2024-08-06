using bedrock.NetHub.StartUp;
using bedrock.NetHub.Utils;
using bedrock.NetHub.Event;
using bedrock.NetHub.service;
using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using bedrock.NetHub.Service;
using System.Net.NetworkInformation;

namespace bedrock.NetHub
{
    internal abstract class Program
    {
        public static readonly Logger stdhubLOGGER = new("stdhub");

        public static readonly Logger BDSLOGGER = new("bds");

        public static readonly string programRoot = Environment.CurrentDirectory;

        public static readonly string pluginsRoot = Path.Join(programRoot, "plugins");

        private static Dictionary<string, string> serverProperties = [];

        private static string levelRoot = string.Empty;

        private static Terminal TERMINAL = null;

        private static HttpManager httpManager = null;

        private static List<int> BDSVersionArray = [];

        private static string BDSVersionString = string.Empty;

        private static readonly PermissionsGroupManager permissionGroupManager = new();

        private static readonly CommandManager commandManager = new();

        private static readonly XuidManager xuidManager = new();

        public static string GetLevelRoot()
        {
            return levelRoot;
        }
        public static Dictionary<string, string> GetServerProperties()
        {
            return serverProperties;
        }
        public static PermissionsGroupManager GetPermissionsGroupManager()
        {
            return permissionGroupManager;
        }public static Terminal GetTerminal()
        {
            return TERMINAL;
        }
        public static HttpManager GetHttpManager()
        {
            return httpManager;
        }

        public static CommandManager GetCommandManager()
        {
            return commandManager;
        }

        public static XuidManager GetXuidManager()
        {
            return xuidManager;
        }
        private static int GetAllAvailableTCPPort(int startPort = 8000)
        {
            IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] iPEndPoints = iPGlobalProperties.GetActiveTcpListeners();
            List<int> APorts = [];
            foreach (IPEndPoint i in iPEndPoints)
            {
                if (i.Port > startPort && i.Port <= 65535)
                {
                    APorts.Add(i.Port);
                }
            }
            if (APorts.Count > 0)
            {
                return APorts.Min();
            }
            else
            {
                return -1;
            }
        }
        static void Main(string[] args)
        {
            string bdsCommand;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                bdsCommand = ".\\bedrock_server.exe";
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                bdsCommand = "./bedrock_server";
            }
            else
            {
                bdsCommand = string.Empty;
                stdhubLOGGER.Info(string.Format("§cThe current platform {0} is not supported.", Environment.OSVersion.Platform.ToString()));
                Environment.Exit(1);
            }

            if (!File.Exists("bedrock_server.exe") && !File.Exists("bedrock_server"))
            {
                stdhubLOGGER.Info("§cBDS binary file not found. Please check your installation.");
                Environment.Exit(1);
            }

            if (!File.Exists("server.properties"))
            {
                stdhubLOGGER.Info("§cserver.properties file not found. Please check your installation.");
                Environment.Exit(1);
            }

            serverProperties = FileIO.ReadProperties(Path.Join(programRoot, "server.properties"));
            if (serverProperties == null) {
                stdhubLOGGER.Info("§cserver.properties file is wrong. Please check your installation.");
                Environment.Exit(1);
            }

            if (!serverProperties.TryGetValue("level-name", out string? value)) {
                levelRoot = Path.Join(programRoot, "worlds", "Bedrock level");
            }
            else
            {
                levelRoot = Path.Join(programRoot, "worlds", value);
            }

            if (!Path.Exists(levelRoot))
            {
                stdhubLOGGER.Info("§cWorld folder not found. Please run bedrock_server first to generate a world.");
                Environment.Exit(1);
            }

            if (Array.IndexOf(args,"--debug-mode") != -1) {
                stdhubLOGGER.Info(
                    "§e============ ATTENTION ============" +
                    "§eThe application is running under DEBUG MODE§r." +
                    "§eIt will listen to every existent file in `plugins` folder" +
                    "§eWhen any file changes, it will copy new plugin files to the world folder and delete the old." +
                    "§eHowever, it won\'t listen to newly added files, nor will it copy it to the world folder." +
                    "§eSo if you want to have a test on new plugins, please restart the application.\n"
                    );
            }

            FileIO.EnsurePath(Path.Join(levelRoot, "behavior_packs"));

            FileIO.EnsurePath(pluginsRoot);

            DirectoryInfo[] oldPluginFileInfo = new DirectoryInfo(Path.Join(levelRoot, "behavior_packs")).GetDirectories();
            foreach (DirectoryInfo index in oldPluginFileInfo)
            {
                if (index.Name.StartsWith("__stdhub_plugins"))
                {
                    Directory.Delete(index.FullName, true);
                }
            }
            stdhubLOGGER.Info("§aRemoved old plugins.");
            PluginLoader.Load(pluginsRoot, levelRoot);

            //int port = GetAllAvailableTCPPort(8000);
            int port = 8001;
            if (port == -1)
            {
                throw new Exception("No Avaliable Port");
            }
            else
            {
                string listenerUrl = $"http://127.0.0.1:{port}";
                FileIO.WriteFile(Path.Join(programRoot,"config", "default", "variables.json"),JsonConvert.SerializeObject(new { backendAddress = listenerUrl }));
                TERMINAL = new(bdsCommand);
                httpManager = new(listenerUrl);
                Task.Run(async () =>
                {
                    httpManager.Start();
                });
                //httpManager.Start();
                stdhubLOGGER.Info($"Backend server started on *:{port}");
                TERMINAL.Start();
                stdhubLOGGER.Info("Starting BDS process...");
            }
        }
    }
}