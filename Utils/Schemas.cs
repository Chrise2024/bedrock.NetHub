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

    public struct VersionMappingSchema(string apiVersion, string releaseVersion)
    {
        public string apiVersion = apiVersion;
        public string releaseVersion = releaseVersion;
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

    public struct PermissionsGroupSchema(List<string> permissions, string extends = "")
    {
        public string extends = extends;
        public List<string> permissions = permissions;
    }

    public struct ResolvedCommandSchema(string NameSpace, string resolvedText,string permission = null)
    {
        public string NameSpace = NameSpace;
        public string resolvedText = resolvedText;
        public string permission = permission;
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
            string Templet = @"{{
        ""format_version"": 2,
        ""header"": {{
          ""name"": ""{0}"",
          ""description"": ""{1}"",
          ""uuid"": ""{2}"",
          ""version"": [{3}],
          ""min_engine_version"": [{4}]
        }},
        ""modules"": [
          {{
            ""description"": ""Script resources"",
            ""language"": ""javascript"",
            ""type"": ""script"",
            ""uuid"": ""{5}"",
            ""version"": [{3}],
            ""entry"": ""scripts/{6}""
          }}
        ],
        ""dependencies"": [
          {{
            ""module_name"": ""@minecraft/server"",
            ""version"": ""{7}""
          }},
          {{
            ""module_name"": ""@minecraft/server-net"",
            ""version"": ""1.0.0-beta""
          }},
          {{
            ""module_name"": ""@minecraft/server-admin"",
            ""version"": ""1.0.0-beta""
          }}
        ]
      }}";
            return string.Format(
                Templet,
                name,
                description,
                UUID,
                string.Join(',', version),
                string.Join(',', BDSVersion),
                scriptModuleUUID,
                entryScriptName,
                apiVersion
                );
        }
    }
}
