﻿using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using bedrock.NetHub;
using bedrock.NetHub.Event;
using System.ComponentModel.Design;
using bedrock.NetHub.Service;
using Newtonsoft.Json;
using bedrock.NetHub.Utils;
using Newtonsoft.Json.Linq;

namespace bedrock.NetHub.service
{
    public partial class Terminal
    {
        private Process bdsProcess = new();

        private StreamWriter bdsWriter;

        private readonly CommandManager commandManager = Program.GetCommandManager();

        private static bool testForServerPackStack = true;

        private static readonly XuidManager xuidManager = Program.GetXuidManager();

        private string customCommandPrefix = ".";
        public Terminal(string bdsCommand)
        {
            bdsProcess.StartInfo.FileName = bdsCommand;
            bdsProcess.StartInfo.UseShellExecute = false;
            bdsProcess.StartInfo.RedirectStandardInput = true;
            bdsProcess.StartInfo.RedirectStandardOutput = true;
            bdsProcess.StartInfo.RedirectStandardError = true;
            bdsProcess.OutputDataReceived += BDSStdOutHandler;
            bdsProcess.ErrorDataReceived += BDSStdErrorHandler;
            string cPath = Path.Join(Program.pluginsRoot, "command-core", "config.json");
            if (File.Exists(cPath))
            {
                try
                {
                    JObject configJSON = FileIO.ReadAsJSON(cPath);
                    if (configJSON.ContainsKey("commandPrefix"))
                    {
                        customCommandPrefix = configJSON["commandPrefix"].Value<string>();
                    }
                    else
                    {
                        customCommandPrefix = ".";
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    customCommandPrefix = ".";
                }
            }
            else
            {
                customCommandPrefix = ".";
            }
        }

        public void Start()
        {
            bdsProcess.Start();

            bdsWriter = bdsProcess.StandardInput;

            bdsProcess.BeginOutputReadLine();
            bdsProcess.BeginErrorReadLine();
            InputHandler();
            bdsProcess.WaitForExit();
        }

        private async void InputHandler()
        {
            await Task.Run(() =>
            {
                string InputCommand = "";
                while (!InputCommand.Equals("stop"))
                {
                    InputCommand = Console.ReadLine();
                    if (InputCommand.StartsWith(customCommandPrefix))
                    {
                        commandManager.ProcessConsoleCommand(InputCommand[customCommandPrefix.Length..]);
                    }
                    else if (InputCommand.Equals("reload"))
                    {
                        if (!Program.IsDebug())
                        {
                            Program.stdhubLOGGER.Info("§cReload is a dangerous operation and can only be performed in debug mode.");
                        }
                        else
                        {
                            Program.GetCommandManager().ClearRegistry();
                            Program.GetPermissionsGroupManager().ClearPermissionSettings();
                        }
                    }
                    else
                    {
                        bdsWriter.WriteLine(InputCommand);
                    }
                }
                bdsWriter.Close();
            });
        }
        private static void BDSStdOutHandler(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                string[] Lines = e.Data.Split("\r*\n");
                foreach (string line in Lines)
                {
                    if (GetBDSLogRegex().IsMatch(line))
                    {
                        string Content = GetBDSLogPrefixRegex().Replace(line, "").Trim();
                        string prefix = GetBDSLogPrefixRegex().Matches(line)[0].Value;
                        string timeString = GetBDSTimeRegex().Matches(line)[0].Value.Trim().Replace("[","");
                        string Level = GetBDSTimeRegex().Replace(line, "").Split("]")[0].Trim();
                        if (Content.StartsWith("[Packs] [SERVER] Pack Stack"))
                        {
                            return;
                        }
                        else
                        {
                            xuidManager.HandleXuidLogging(Content);
                            Program.BDSLOGGER.Info(Level+": "+Content,timeString);
                        }
                    }
                }
            }
        }

        private static void BDSStdErrorHandler(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Program.stdhubLOGGER.Info(e.Data);
            }
        }

        public void SendCommand(string command)
        {
            bdsWriter.WriteLine(command);
        }

        public void TriggerScriptEvent(string nameSpace, ScriptEventSchema Event)
        {
            //Console.WriteLine(string.Format("scriptevent {0}:{1} {2}", nameSpace, "NACC", JsonConvert.SerializeObject(new CommandDispatchEvent("AAA","BBB"))));
            SendCommand(string.Format("scriptevent {0}:{1} {2}",nameSpace, "CommandDispatchEvent", JsonConvert.SerializeObject(Event)));
        }

        [GeneratedRegex("^\\[(\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}:\\d{3}) (\\w+)] (.*)$")]
        private static partial Regex GetBDSLogRegex();

        [GeneratedRegex("^\\[(\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}:\\d{3}) (\\w+)] ")]
        private static partial Regex GetBDSLogPrefixRegex();

        [GeneratedRegex("^\\[(\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}:\\d{3})")]
        private static partial Regex GetBDSTimeRegex();
    }
}
