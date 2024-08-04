using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using bedrock.NetHub;
using bedrock.NetHub.Event;
using System.ComponentModel.Design;
using bedrock.NetHub.Service;

namespace bedrock.NetHub.service
{
    public partial class Terminal
    {
        private Process bdsProcess = new();

        private StreamWriter bdsWriter;

        private readonly CommandManager commandManager = Program.GetCommandManager();

        private static bool testForServerPackStack = true;
        public Terminal(string bdsCommand)
        {
            bdsProcess.StartInfo.FileName = bdsCommand;
            bdsProcess.StartInfo.UseShellExecute = false;
            bdsProcess.StartInfo.RedirectStandardInput = true;
            bdsProcess.StartInfo.RedirectStandardOutput = true;
            bdsProcess.StartInfo.RedirectStandardError = true;
            bdsProcess.OutputDataReceived += BDSStdOutHandler;
            bdsProcess.ErrorDataReceived += BDSStdErrorHandler;
            bdsProcess.Start();

            bdsWriter = bdsProcess.StandardInput;

            bdsProcess.BeginOutputReadLine();
            bdsProcess.BeginErrorReadLine();
            string InputCommand = "";
            do
            {
                InputCommand = Console.ReadLine();
                commandManager.ProcessConsoleCommand(InputCommand);
                bdsWriter.WriteLine(InputCommand);
            } while (!InputCommand.Equals("stop"));
            bdsWriter.Close();
            bdsProcess.WaitForExit();
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

        public void TriggerScriptEvent(string nameSpace,ScriptEvent Event)
        {
            SendCommand(string.Format("scriptevent {0}:{1} {2}",nameSpace,Event.eventName,Event.ToString()));
        }

        [GeneratedRegex("^\\[(\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}:\\d{3}) (\\w+)] (.*)$")]
        private static partial Regex GetBDSLogRegex();

        [GeneratedRegex("^\\[(\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}:\\d{3}) (\\w+)] ")]
        private static partial Regex GetBDSLogPrefixRegex();

        [GeneratedRegex("^\\[(\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}:\\d{3})")]
        private static partial Regex GetBDSTimeRegex();
    }
}
