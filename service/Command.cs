using bedrock.NetHub.Event;
using bedrock.NetHub.Utils;
using System;
using System.Collections.Generic;

namespace bedrock.NetHub.Service
{
    public class CommandManager
    {
        private readonly Dictionary<string, string> Commands = [];

        private readonly Dictionary<string, string> DefaultCommandNames = [];
        public CommandManager() {

        }

        public bool RegisterCommand(string NameSpace,string commandName, string permission = null) {
            string cmdNameWithNs = NameSpace + ":" + commandName;
            string permissionWithNs = permission != null ? NameSpace + ":" + permission : "";
            if (Commands.ContainsKey(cmdNameWithNs))
            {
                return false;
            }
            if (DefaultCommandNames.TryGetValue(cmdNameWithNs, out string? value))
            {
                Program.stdhubLOGGER.Info(string.Format("Command naming conflict: '{0}' & '{1}'.", value, cmdNameWithNs));
                Program.stdhubLOGGER.Info(string.Format("Consider removing one of the plugins, or call the latter with prefix {0}.",NameSpace));
            }
            else
            {
                DefaultCommandNames.Add(cmdNameWithNs, commandName);
            }
            Commands.Add(cmdNameWithNs, permissionWithNs);
            Program.GetPermissionsGroupManager().AddPermissionKey(permissionWithNs);
            if (permission != null)
            {
                Program.stdhubLOGGER.Info(string.Format("Command registered: §a{0}§r with permission §b{1}",cmdNameWithNs,permissionWithNs));
            }
            else
            {
                Program.stdhubLOGGER.Info(string.Format("Command registered: §a{0}", cmdNameWithNs));
            }
            return true;
        }

        public ResolvedCommandSchema ResolveCommand(string commandString)
        {
            string commandName = commandString.Split(" ")[0];
            if (commandName.Contains(':'))
            {
                if (!Commands.TryGetValue(commandName, out string? value))
                {
                    return new ResolvedCommandSchema(null, null);
                }
                else
                {
                    string NameSpace = commandName.Split(':')[0];
                    return new ResolvedCommandSchema(NameSpace, commandString[(NameSpace.Length + 1)..], value);
                }
            }
            else
            {
                if (DefaultCommandNames.TryGetValue(commandName, out string? defaultCommand)) 
                {
                    string NameSpace = defaultCommand.Split(":")[0];
                    return new ResolvedCommandSchema(NameSpace, commandString, Commands[commandName]);
                }
                else
                {
                    return new ResolvedCommandSchema(null, null);
                }
            }
        }
        public int TriggerCommand(string commandString,bool isOP = false,string playerID = null,string xuid = null)
        {
            ResolvedCommandSchema resolved = ResolveCommand(commandString);
            if (resolved.NameSpace == null)
            {
                return 404;
            }
            else
            {
                if (!isOP && xuid != null && resolved.permission != null) 
                {
                    if (!Program.GetPermissionsGroupManager().TestPermission(xuid, resolved.permission))
                    {
                        return 403;
                    }
                }
                Program.GetTerminal().TriggerScriptEvent(resolved.NameSpace,new CommandDispatchEvent(resolved.resolvedText,playerID));
                return 200;
            }
        }

        public void ProcessConsoleCommand(string commandString)
        {
            string commandName = commandString.Split(' ')[0];
            int triggerResult = TriggerCommand(commandString,true);
            if (triggerResult == 404)
            {
                Program.stdhubLOGGER.Info(string.Format("§cUnknown command: {0}. Please check that the command exists and you have permission to use it.",commandName));
            }
        }
        public void ClearRegistry()
        {
            Commands.Clear();
            DefaultCommandNames.Clear();
            Program.stdhubLOGGER.Info("Command registry cleared.");
        }
    }
}
