using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bedrock.NetHub.Utils
{
    public struct ServerApiVersionMappingSchema
    {
        public List<VersionMappingSchema> versionMapping;
        public bool cacheUsed;
    }

    public struct VersionMappingSchema
    {
        public string apiVersion;
        public string releaseVersion;
    }
    public struct BehaviorPacksSchema
    {
        public string pack_id;
        public int[] version;
    }

    public struct PluginJSONSchema
    {
        public PluginPropertiesSchema plugin;
        public string targetMinecraftVersion;
    }
    public struct PluginPropertiesSchema
    {
        public string name;
        public string description;
        public string version;
    }

    public abstract class Schemas
    {
        public static string ManifestFileGenerator(
            string name,
            string description,
            string UUID,
            List<int> version,
            List<int> BDSVersion,
            string scriptModuleUUID,
            string entryScriptName,
            string apiVersion
            )
        {
            string Templet = @"{
        ""format_version"": 2,
        ""header"": {
          ""name"": ""{0}"",
          ""description"": ""{1}"",
          ""uuid"": ""{3}"",
          ""version"": {4},
          ""min_engine_version"": {5},
        },
        ""modules"": [
          {
            ""description"": ""Script resources"",
            ""language"": ""javascript"",
            ""type"": ""script"",
            ""uuid"": ""{6}"",
            ""version"": {4},
            ""entry"": ""scripts/{7}"",
          },
        ],
        ""dependencies"": [
          {
            ""module_name"": ""@minecraft/server"",
            ""version"": {8},
          },
          {
            ""module_name"": ""@minecraft/server-net"",
            ""version"": ""1.0.0-beta"",
          },
          {
            ""module_name"": ""@minecraft/server-admin"",
            ""version"": ""1.0.0-beta"",
          },
        ],
      }";
            return string.Format(
                Templet,
                name,
                description,
                UUID,
                version,
                BDSVersion,
                scriptModuleUUID,
                entryScriptName,
                apiVersion
                );
        }
    }
}
