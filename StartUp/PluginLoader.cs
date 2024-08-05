using System;
using System.Collections;
using System.Collections.Generic;
using bedrock.NetHub;
using bedrock.NetHub.Utils;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;

namespace bedrock.NetHub.StartUp
{
    public abstract class PluginLoader
    {
        public static readonly string pluginEntryScriptName = "main.js";
        public static int Load(string pluginPath,string levelRoot)
        {
            int loadedPluginNumber = 0;
            FileInfo[] pluginFileInfo = new DirectoryInfo(pluginPath).GetFiles();
            string originalWorldBehaviorPacksFilePath = Path.Join(levelRoot, "world_behavior_packs.json.original");
            string worldBehaviorPacksFilePath = Path.Join(levelRoot, "world_behavior_packs.json");
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

            JArray worldBehaviorPacks = FileIO.ReadAsJArray(originalWorldBehaviorPacksFilePath);
            List<string> plugins = [];
            foreach(FileInfo index in pluginFileInfo)
            {
                if (index.Name.EndsWith(".stdplugin"))
                {
                    plugins.Add(index.FullName);
                }
            }

            List<int> currentBDSVersionArray = BDSVersion.GetCurrentBDSVersion();
            string currentBDSVersionString = string.Join(".", currentBDSVersionArray);
            Program.stdhubLOGGER.Info(string.Format("Your current BDS version is: {0}", string.Join('.', currentBDSVersionArray)));
            Program.stdhubLOGGER.Info("If this does not match, please report an issue.");
            List<VersionMappingSchema> versionMapping = BDSVersion.GetMinecraftServerApiVersionMapping();
            bool status = true;
            VersionMappingSchema currentVersionMap = new("0.0.0", "0.0.0");
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
                try
                {
                    ZipArchive pluginArchive = ZipFile.OpenRead(index);
                    ZipArchiveEntry pluginEntry = pluginArchive.GetEntry("plugin.json");
                    ZipArchiveEntry scriptEntry = pluginArchive.GetEntry("script.js");
                    if (pluginEntry != null && scriptEntry != null)
                    {
                        StreamReader sw = new(pluginEntry.Open());
                        JObject pluginJSON = JObject.Parse(sw.ReadToEnd());
                        JObject pluginPropertiesJSON = pluginJSON["plugin"].Value<JObject>();
                        string pluginName = pluginPropertiesJSON["name"].Value<string>();
                        string pluginVersionString = pluginPropertiesJSON["version"].Value<string>();
                        string pluginDescription = pluginPropertiesJSON["description"].Value<string>();
                        string targetMinecraftVersion = pluginJSON["targetMinecraftVersion"].Value<string>();

                        if (!currentBDSVersionString.Equals(targetMinecraftVersion))
                        {
                            Program.stdhubLOGGER.Info(string.Format("§eThe Minecraft version requirement of plugin §b{0}§e (§c{1}§e)", pluginName, targetMinecraftVersion));
                            Program.stdhubLOGGER.Info(string.Format("§edoes not match current version (§a{0}§e).", currentBDSVersionString));
                            Program.stdhubLOGGER.Info("§eWe will still enable this plugin.");
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
                        worldBehaviorPacks.Add(JsonConvert.DeserializeObject(string.Format("{{\"pack_id\": \"{0}\",\"version\": [{1}]}}", pluginUUID, string.Join(',',pluginVersionArray))));

                        FileIO.WriteAsJSON(worldBehaviorPacksFilePath, worldBehaviorPacks);
                        loadedPluginNumber++;
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Program.stdhubLOGGER.Info(string.Format("Bad plugin {0}:",index) + ex.Message);
                    return 0;
                }
            }
            return loadedPluginNumber;
        }
    }
}
