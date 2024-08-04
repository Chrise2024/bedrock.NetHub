using System;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using bedrock.NetHub;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace bedrock.NetHub.Utils
{
    public abstract partial class BDSVersion
    {
        public static List<int> GetCurrentBDSVersion()
        {
            Regex vanillaBehaviorPackExp = VBPRegex();
            string behaviorPacksRoot = Path.Join(Program.programRoot, "behavior_packs");
            DirectoryInfo[] behaviorPacks = new DirectoryInfo(behaviorPacksRoot).GetDirectories();
            List<List<int>> behaviorPackVersions = [];
            foreach (DirectoryInfo behaviorPack in behaviorPacks)
            {
                if (vanillaBehaviorPackExp.IsMatch(behaviorPack.Name))
                {
                    List<int> temp = [];
                    foreach (string index in behaviorPack.Name[8..].Split('.'))
                    {
                        temp.Add(int.Parse(index));
                    }
                    behaviorPackVersions.Add(temp);
                }
            }
            return FindMaxVersion(behaviorPackVersions);
        }

        public static List<VersionMappingSchema> GetMinecraftServerApiVersionMapping(bool useCache = true)
        {
            List<VersionMappingSchema> versionMapping = [];
            string cachePath = Path.Join(Program.programRoot, "cache", "serverApiVersions.json");
            FileIO.EnsurePath(Path.Join(Program.programRoot, "cache"));
            if (!File.Exists(cachePath) || !useCache)
            {
                Program.stdhubLOGGER.Info("§eCache disabled or not found. Fetching fresh information...");
                List<string> RawVersionList = FetchVersions("@minecraft/server");
                foreach (string index in RawVersionList)
                {
                    if (VersionRegex().IsMatch(index))
                    {
                        //Console.WriteLine(VersionRegex().Matches(index)[0].Value);
                        string reducedVersion = ReducedVersionPrefixRegex().Matches(index)![0].Value;
                        VersionMappingSchema VMT = new(ApiVersionRegex().Matches(reducedVersion)[0].Value, ApiVersionRegex().Replace(reducedVersion, "")[1..]);
                        versionMapping.Add(VMT);
                    }
                }
                versionMapping.Reverse();
                FileIO.WriteFile(cachePath,JsonConvert.SerializeObject(versionMapping));
                return versionMapping;
            }
            else
            {
                versionMapping = FileIO.ReadAsJArray(cachePath).ToObject<List<VersionMappingSchema>>();
                return versionMapping;
            }
        }

        public static List<string> FetchVersions(string packageName)
        {
            List<string> versions = [];
            HttpClient httpClient = new();
            HttpResponseMessage response = httpClient.GetAsync(string.Format("https://registry.npmjs.org/{0}",packageName)).Result;
            //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            JObject res = JObject.Parse(response.Content.ReadAsStringAsync().Result)["versions"].Value<JObject>();
            foreach (var index in res.Properties())
            {
                versions.Add(index.Name);
            }
            return versions;
        }
        private static bool VersionCompare(List<int> targetA,List<int> targetB)
        {
            if (targetA.Count > targetB.Count)
            {
                return true;
            }
            else if (targetA.Count < targetB.Count) {
                return false;
            }
            else
            {
                for (int i = 0; i < targetA.Count; i++)
                {
                    if (targetA[i] > targetB[i])
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private static List<int> FindMaxVersion(List<List<int>> Versions)
        {
            List<int> MaxVersion = [0, 0, 0];
            foreach (List<int> Version in Versions)
            {
                if (VersionCompare(Version, MaxVersion))
                {
                    MaxVersion = Version;
                }
            }
            return MaxVersion;
        }

        [GeneratedRegex("^vanilla_\\d+\\.\\d+\\.\\d+$")]
        private static partial Regex VBPRegex();

        [GeneratedRegex("^(\\d+\\.\\d+\\.\\d+-beta\\.\\d+\\.\\d+\\.\\d+)-stable$")]
        private static partial Regex VersionRegex();

        [GeneratedRegex("^(\\d+\\.\\d+\\.\\d+-beta)\\.(\\d+\\.\\d+\\.\\d+)")]
        private static partial Regex ReducedVersionPrefixRegex();
        [GeneratedRegex("^(\\d+\\.\\d+\\.\\d+-beta)")]
        private static partial Regex ApiVersionRegex();
    }
}
