using bedrock.NetHub.StartUp;
using bedrock.NetHub.Utils;
using bedrock.NetHub.Event;
using bedrock.NetHub.service;
using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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

        private static List<int> BDSVersionArray = [];

        private static string BDSVersionString = string.Empty;

        public static string GetLevelRoot()
        {
            return levelRoot;
        }
        public static Dictionary<string, string> GetServerProperties()
        {
            return serverProperties;
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

            serverProperties = FileIO.ReadProperties(Path.Join(pluginsRoot, "server.properties"));
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

            FileInfo[] oldPluginFileInfo = new DirectoryInfo(Path.Join(levelRoot, "behavior_packs")).GetFiles();
            foreach (FileInfo index in oldPluginFileInfo)
            {
                if (index.Name.StartsWith("__stdhub_plugins"))
                {
                    Directory.Delete(index.FullName, true);
                }
            }
        }
    }
}