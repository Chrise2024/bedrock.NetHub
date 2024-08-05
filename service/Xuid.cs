using bedrock.NetHub.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace bedrock.NetHub.Service
{
    public partial class XuidManager
    {
        private readonly string userCacheFileName = Path.Join(Program.programRoot, "bsh-user-cache.json");

        private XuidMapSchema XuidMap;

        public XuidManager()
        {
            FileIO.EnsureFile(userCacheFileName, JsonConvert.SerializeObject(new XuidMapSchema([], [])));
            XuidMap = FileIO.ReadAsJSON<XuidMapSchema>(userCacheFileName);
        }

        public void HandleXuidLogging(string message)
        {
            if (PlayerJoinInfoRegex().IsMatch(message))
            {
                string[] splitedMessage = PlayerJoinInfoSplitRegex().Split(message.Replace("Player connected: ", ""));
                string PlayerName = splitedMessage[0].Trim();
                string xuid = splitedMessage[1].Trim();
                XuidMap.name2xuid[PlayerName] = xuid;
                XuidMap.xuid2name[xuid] = PlayerName;
                FileIO.WriteAsJSON<XuidMapSchema>(userCacheFileName, XuidMap);
            }
        }

        public string GetXuidByName(string name)
        {
            return XuidMap.name2xuid.TryGetValue(name, out string? value) ? value : "";
        }

        public string GetNameByXuid(string xuid)
        {
            return XuidMap.xuid2name.TryGetValue(xuid, out string? value) ? value : "";
        }

        [GeneratedRegex("^Player connected: (.+), xuid: (\\d+)$")]
        private static partial Regex PlayerJoinInfoRegex();

        [GeneratedRegex(", xuid: ")]
        private static partial Regex PlayerJoinInfoSplitRegex();
    }
}
