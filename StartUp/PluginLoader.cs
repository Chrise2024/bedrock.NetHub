using System;
using System.Collections;
using System.Collections.Generic;
using bedrock.NetHub;
using bedrock.NetHub.Utils;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace bedrock.NetHub.StartUp
{
    public class PluginLoader
    {
        public readonly string pluginEntryScriptName = "main.js";

        private readonly FileSystemWatcher fileSystemWatcher = new();

        private string currentBDSVersionString = string.Empty;

        private List<int> currentBDSVersionArray = [];

        private VersionMappingSchema currentVersionMap = new();

        private List<VersionMappingSchema> versionMapping = [];

        private JArray worldBehaviorPacks = [];

        private string worldBehaviorPacksFilePath = string.Empty;

        private readonly Dictionary<string, string> pluginNameReference = [];
        public PluginLoader()
        {
            fileSystemWatcher.Path = Program.pluginsRoot;
            fileSystemWatcher.NotifyFilter =
                NotifyFilters.Attributes
              | NotifyFilters.CreationTime
              | NotifyFilters.DirectoryName
              | NotifyFilters.FileName
              | NotifyFilters.LastAccess
              | NotifyFilters.LastWrite
              | NotifyFilters.Security
              | NotifyFilters.Size;
            fileSystemWatcher.Filter = "*.stdplugin";
            fileSystemWatcher.Changed += OnPluginChange;
            fileSystemWatcher.Created += OnPluginAdd;
            fileSystemWatcher.Deleted += OnPluginDelete;
        }

        public void StartWatch()
        {
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void StopWatch()
        {
            if (!Program.IsDebug())
            {
                return;
            }
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();
        }

        private void OnPluginAdd(object source, FileSystemEventArgs e)
        {
            ExtractPlugin(e.FullPath);
            Program.stdhubLOGGER.Info(string.Format("§ePlugin §b{0}§e added. Please execute `§areload§e` AT THE TERMINAL to see changes.",e.Name));
        }

        private void OnPluginChange(object source, FileSystemEventArgs e)
        {
            ExtractPlugin(e.FullPath);
            Program.stdhubLOGGER.Info(string.Format("§ePlugin §b{0}§e changed. Please execute `§areload§e` AT THE TERMINAL to see changes.", e.Name));
        }

        private void OnPluginDelete(object source, FileSystemEventArgs e)
        {
            string rootPath = Directory.GetDirectoryRoot(e.FullPath);
            if (pluginNameReference.TryGetValue(e.Name, out string pluginPath))
            {
                if (Directory.Exists(pluginPath))
                {
                    Directory.Delete(Path.Join(rootPath, pluginPath), true);
                    pluginNameReference.Remove(e.Name);
                    Program.stdhubLOGGER.Info(string.Format("§ePlugin §b{0}§e deleted. Please execute `§areload§e` AT THE TERMINAL to see changes.", e.Name));
                }
            }
        }

        public int Load(string pluginPath,string levelRoot)
        {
            int loadedPluginNumber = 0;
            FileInfo[] pluginFileInfo = new DirectoryInfo(pluginPath).GetFiles();
            string originalWorldBehaviorPacksFilePath = Path.Join(levelRoot, "world_behavior_packs.json.original");
            worldBehaviorPacksFilePath = Path.Join(levelRoot, "world_behavior_packs.json");
            if (!File.Exists(originalWorldBehaviorPacksFilePath))
            {
                if (!File.Exists(worldBehaviorPacksFilePath))
                {
                    FileIO.WriteFile(originalWorldBehaviorPacksFilePath, "[]");
                }
                else
                {
                    File.Copy(worldBehaviorPacksFilePath, originalWorldBehaviorPacksFilePath, true);
                }
            }

            worldBehaviorPacks = FileIO.ReadAsJArray(originalWorldBehaviorPacksFilePath);
            List<string> plugins = [];
            foreach(FileInfo index in pluginFileInfo)
            {
                if (index.Name.EndsWith(".stdplugin"))
                {
                    plugins.Add(index.FullName);
                }
            }

            currentBDSVersionArray = BDSVersion.GetCurrentBDSVersion();
            currentBDSVersionString = string.Join(".", currentBDSVersionArray);
            Program.stdhubLOGGER.Info(string.Format("Your current BDS version is: {0}", string.Join('.', currentBDSVersionArray)));
            Program.stdhubLOGGER.Info("If this does not match, please report an issue.");
            versionMapping = BDSVersion.GetMinecraftServerApiVersionMapping();
            bool status = true;
            currentVersionMap = new("0.0.0", "0.0.0");
            foreach (VersionMappingSchema versionMap in versionMapping)
            {
                if (versionMap.releaseVersion.Equals(currentBDSVersionString))
                {
                    currentVersionMap = versionMap;
                    status = false;
                    break;
                }
            }
            if (status)
            {
                versionMapping = BDSVersion.GetMinecraftServerApiVersionMapping(false);
                foreach (VersionMappingSchema versionMap in versionMapping)
                {
                    if (versionMap.releaseVersion.Equals(currentBDSVersionString))
                    {
                        currentVersionMap = versionMap;
                        status = false;
                        break;
                    }
                }
            }
            if (status)
            {
                Program.stdhubLOGGER.Info("Seems that you are using a newer release of BDS, which is not listed in npm registry.");
                Program.stdhubLOGGER.Info("Check if you are using a preview version, or wait a moment.");
                throw new Exception("Version not supported");
            }

            foreach(string index in plugins)
            {
                bool res = ExtractPlugin(index);
                loadedPluginNumber += res ? 1 : 0;
            }
            Program.stdhubLOGGER.Info(string.Format("§aSuccessfully loaded §b{0}§a plugin(s).", loadedPluginNumber));
            return loadedPluginNumber;
        }

        private bool ExtractPlugin(string pluginPath)
        {
            try
            {
                ZipArchive pluginArchive = ZipFile.OpenRead(pluginPath);
                ZipArchiveEntry pluginEntry = pluginArchive.GetEntry("plugin.json");
                ZipArchiveEntry scriptEntry = pluginArchive.GetEntry("script.js");
                if (pluginEntry != null && scriptEntry != null)
                {
                    StreamReader sw = new(pluginEntry.Open());
                    JObject pluginJSON = JObject.Parse(sw.ReadToEnd());
                    JObject pluginPropertiesJSON = pluginJSON["plugin"].ToObject<JObject>();
                    string pluginName = pluginPropertiesJSON["name"].Value<string>();
                    string pluginVersionString = pluginPropertiesJSON["version"].Value<string>();
                    string pluginDescription = pluginPropertiesJSON["description"].Value<string>();
                    string targetMinecraftVersion = pluginJSON["targetMinecraftVersion"].Value<string>();

                    if (!currentBDSVersionString.Equals(targetMinecraftVersion))
                    {
                        Program.stdhubLOGGER.Info(string.Format("§eThe Minecraft version requirement of plugin §b{0}§e (§c{1}§e)", pluginName, targetMinecraftVersion));
                        Program.stdhubLOGGER.Info(string.Format("§edoes not match current version (§a{0}§e).", currentBDSVersionString));
                        Program.stdhubLOGGER.Info("§eWe will still enable this plugin.");
                        Program.stdhubLOGGER.Info("§eBut when it does not function as expected, do not report any issue.");
                    }
                    string pluginUUID = Guid.NewGuid().ToString();
                    string scriptModuleUUID = Guid.NewGuid().ToString();
                    List<int> pluginVersionArray = TypeCast.VersionStringToArray(pluginVersionString);
                    Program.stdhubLOGGER.Info(string.Format("Loading plugin §b{0}§r...", pluginName));
                    string tempPluginName = "__stdhub_plugins_" + pluginUUID;
                    string pluginRoot = Path.Join(Program.GetLevelRoot(), "behavior_packs", tempPluginName);
                    string pluginScriptRoot = Path.Join(pluginRoot, "scripts");

                    FileIO.EnsurePath(pluginRoot);
                    FileIO.EnsurePath(pluginScriptRoot);

                    string ManifestFileContent = Schemas.ManifestFileGenerator(
                        pluginName,
                        pluginDescription,
                        pluginUUID,
                        pluginVersionArray,
                        currentBDSVersionArray,
                        scriptModuleUUID,
                        pluginEntryScriptName,
                        currentVersionMap.apiVersion
                        );
                    FileIO.WriteFile(Path.Join(pluginRoot, "manifest.json"), ManifestFileContent);

                    string scriptPath = Path.Join(pluginScriptRoot, pluginEntryScriptName);
                    FileIO.EnsureFile(scriptPath);
                    sw = new(scriptEntry.Open());
                    FileIO.WriteFile(scriptPath, sw.ReadToEnd());
                    worldBehaviorPacks.Add(JsonConvert.DeserializeObject(string.Format("{{\"pack_id\": \"{0}\",\"version\": [{1}]}}", pluginUUID, string.Join(',', pluginVersionArray))));

                    FileIO.WriteAsJSON(worldBehaviorPacksFilePath, worldBehaviorPacks);
                    pluginNameReference.Add(pluginName, tempPluginName);
                    pluginArchive.Dispose();
                    return true;
                }
                else
                {
                    pluginArchive.Dispose();
                    return false;
                }
            }
            catch(Exception ex)
            {
                Program.stdhubLOGGER.Info(string.Format("Bad plugin {0}:", pluginPath) + ex.Message);
                return false;
            }
        }
    }
}
