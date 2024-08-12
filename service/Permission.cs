using bedrock.NetHub.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace bedrock.NetHub.Service
{
    public class PermissionsGroupManager
    {
        private readonly Dictionary<string, PermissionsGroupSchema> PermissionsGroupMap = [];

        private readonly Dictionary<string, List<string>> PermissionsGroupMapCache = [];

        private readonly Dictionary<string, List<string>> PlayerGroupingInfo = [];

        private PermissionsGroupSchema DefaultPermissionsGroup = new([]);

        private readonly List<string> PermissionKeys = [];

        public readonly string permissionDataPath = Path.Join(Program.programRoot, "permissions");

        public readonly string playersJsonPath = Path.Join(Program.programRoot, "permissions", "players.json");
        public PermissionsGroupManager() {
            FileIO.EnsurePath(permissionDataPath);
            FileIO.EnsureFile(Path.Join(permissionDataPath, "default.group.json"));
            WriteGroup("default", DefaultPermissionsGroup);
            FileInfo[] permissionFiles = new DirectoryInfo(permissionDataPath).GetFiles();
            foreach (FileInfo permissionFile in permissionFiles)
            {
                if (permissionFile.Name.EndsWith(".group.json")) {
                    string groupName = permissionFile.Name.Split('.')[0];
                    PermissionsGroupSchema groupData = FileIO.ReadAsJSON<PermissionsGroupSchema>(permissionFile.FullName);
                    PermissionsGroupMap.Add(groupName, groupData);
                    foreach (string permissionKey in groupData.permissions) {
                        if (!PermissionKeys.Contains(permissionKey)) {
                            PermissionKeys.Add(permissionKey);
                        }
                    }
                }
            }
            foreach (string GroupName in PermissionsGroupMap.Keys)
            {
                PermissionsGroupMapCache.Add(GroupName, FindPermissionsOfGroup(GroupName));
            }
            FileIO.EnsureFile(playersJsonPath,"{}");
            PlayerGroupingInfo = FileIO.ReadAsJSON<Dictionary<string, List<string>>>(playersJsonPath);
        }

        public List<string> FindPermissionsOfGroup(string groupName)
        {
            if (!PermissionKeys.Contains(groupName))
            {
                return [];
            }
            List<string> Find(string groupName)
            {
                PermissionsGroupSchema groupData = PermissionsGroupMap[groupName];
                if (groupData.extends.Length == 0)
                {
                    return groupData.permissions;
                }
                List<string> Temp = [.. groupData.permissions, .. Find(groupData.extends)];
                return Temp.Distinct().ToList();
            }
            return Find(groupName);
        }

        public bool PermissionExists(string permission)
        {
            return PermissionKeys.Contains(permission);
        }

        public void AddPermissionKey(string permissionKey)
        {
            PermissionKeys.Add(permissionKey);
        }

        public List<string> GetGroups()
        {
            return [.. PermissionsGroupMap.Keys];
        }

        public void CreateGroup(string groupName,string extendsFrom)
        {
            PermissionsGroupSchema groupData = new([],extendsFrom);
            if (!PermissionKeys.Contains(groupName))
            {
                PermissionsGroupMap.Add(groupName, groupData);
                PermissionsGroupMapCache.Add(groupName, FindPermissionsOfGroup(groupName));
            }
            WriteGroup(groupName, groupData);
        }

        public void DeleteGroup(string groupName)
        {
            PermissionsGroupMap.Remove(groupName);
            PermissionsGroupMapCache.Remove(groupName);
            foreach (string index in PlayerGroupingInfo.Keys)
            {
                PlayerGroupingInfo[index].Remove(groupName);
            }
            FileIO.SafeDeleteFile(Path.Join(permissionDataPath, groupName + ".group.json"));
            WritePlayerGroupingInfo();
        }

        public List<string> GetExplicitPermissionsOfGroup(string groupName)
        {
            if (!PermissionsGroupMap.TryGetValue(groupName, out PermissionsGroupSchema value))
            {
                return [];
            }
            else
            {
                if (value.permissions == null)
                {
                    return [];
                }
                else
                {
                    return value.permissions;
                }
            }
        }

        public List<string> GetAllPermissionsOfGroup(string groupName)
        {
            if (!PermissionsGroupMap.TryGetValue(groupName, out PermissionsGroupSchema value)) {
                return [];
            }
            else
            {
                return value.permissions;
            }
        }

        public bool PermissionExistsInGroup(string groupName,string premission)
        {
            if (premission.Length == 0)
            {
                return true;
            }
            if (!PermissionsGroupMapCache.TryGetValue(groupName, out List<string> value))
            {
                return false;
            }
            else
            {
                return value.Contains(premission);
            }
        }

        public void GrantPermissionToGroup(string groupName,string permission)
        {
            if (PermissionsGroupMap.TryGetValue(groupName, out PermissionsGroupSchema value))
            {
                PermissionsGroupSchema groupData = value;
                if (!groupData.permissions.Contains(permission))
                {
                    groupData.permissions.Add(permission);
                }
                RecursivelyUpdateGroupPermissionCache(groupName);
                WriteGroup(groupName,groupData);
            }
            
        }

        public void RevokePermissionFromGroup(string groupName,string permission)
        {
            if (PermissionsGroupMap.TryGetValue(groupName, out PermissionsGroupSchema value))
            {
                PermissionsGroupSchema groupData = value;
                groupData.permissions.Remove(permission);
                RecursivelyUpdateGroupPermissionCache(groupName);
                WriteGroup(groupName, groupData);
            }
        }

        public bool TestPermission(string xuid,string permission)
        {
            if (PlayerGroupingInfo.TryGetValue(xuid, out List<string> value))
            {
                List<string> groupsOfPlayer = value;
                groupsOfPlayer.Add("default");
                foreach (string group in groupsOfPlayer)
                {
                    if (PermissionExistsInGroup(group,permission))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return PermissionExistsInGroup("default", permission);
            }
        }

        public List<string> GetGroupsOfPlayer(string xuid)
        {
            if (!PlayerGroupingInfo.TryGetValue(xuid,out List<string> value))
            {
                return value;
            }
            return [];
        }

        public List<string> GetPlayersInGroup(string groupName)
        {
            List<string> players = [];
            foreach (string index in PlayerGroupingInfo.Keys)
            {
                if (PlayerGroupingInfo[index].Contains(groupName))
                {
                    players.Add(index);
                }
            }
            return players;
        }

        public bool PlayerExistsInGroup(string xuid,string groupName)
        {
            if (PlayerGroupingInfo.TryGetValue(xuid,out List<string> value))
            {
                return value.Contains(groupName);
            }
            return false;
        }

        public void AddPlayerToGroup(string xuid,string groupName)
        {
            if (!PlayerGroupingInfo.TryGetValue(xuid, out List<string> value))
            {
                List<string> groupsOfPlayer = value;
                if (!groupsOfPlayer.Contains(groupName))
                {
                    PlayerGroupingInfo[xuid].Add(groupName);
                }
            }
            else
            {
                List<string> groupsOfPlayer = ["default"];
                if (!groupsOfPlayer.Contains(groupName))
                {
                    PlayerGroupingInfo[xuid].Add(groupName);
                }
            }
            WritePlayerGroupingInfo();
        }

        public void RemovePlayerFromGroup(string xuid,string groupName)
        {
            if (!PlayerGroupingInfo.TryGetValue(xuid, out List<string> value))
            {
                List<string> groupsOfPlayer = value;
                if (groupsOfPlayer.Contains(groupName))
                {
                    PlayerGroupingInfo[xuid].Remove(groupName);
                    WritePlayerGroupingInfo();
                }
            }
        }

        public void ClearPermissionSettings()
        {
            if (!Program.IsDebug())
            {
                throw new Exception("Illegal operation");
            }
            PlayerGroupingInfo.Clear();
            PermissionsGroupMap.Clear();
            PermissionsGroupMapCache.Clear();
            PermissionsGroupMap.Add("default",DefaultPermissionsGroup);
            PermissionsGroupMapCache.Add("default", FindPermissionsOfGroup("default"));
            Program.stdhubLOGGER.Info("Permission settings cleared.");
        }
        private void WriteGroup(string groupName,PermissionsGroupSchema groupData)
        {
            if (Program.IsDebug())
            {
                return;
            }
            FileIO.WriteAsJSON<PermissionsGroupSchema>(Path.Join(permissionDataPath,groupName+ ".group.json"),groupData);
        }

        private void WritePlayerGroupingInfo()
        {
            FileIO.WriteAsJSON<Dictionary<string, List<string>>>(playersJsonPath,PlayerGroupingInfo);
        }

        private void RecursivelyUpdateGroupPermissionCache(string groupName)
        {
            if (PermissionsGroupMapCache.ContainsKey(groupName))
            {
                PermissionsGroupMapCache[groupName] = FindPermissionsOfGroup(groupName);
            }
            foreach (string index in PermissionsGroupMap.Keys)
            {
                PermissionsGroupSchema groupData = PermissionsGroupMap[index];
                if (groupData.extends.Equals(groupName))
                {
                    RecursivelyUpdateGroupPermissionCache(index);
                }
            }
        }
    }
}
