using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using System.ComponentModel.Design;

namespace bedrock.NetHub.service
{
    public partial class Logger(string nameSpace)
    {
        private readonly string nameSpace = nameSpace;
        private readonly Dictionary<string,string> colorMap = new(){
            { "§0", "\x1b{30m" }, // Black
            { "§1", "\x1b{34m" }, // Dark Blue
            { "§2", "\x1b{32m" }, // Dark Green
            { "§3", "\x1b{36m" }, // Dark Aqua
            { "§4", "\x1b{31m" }, // Dark Red
            { "§5", "\x1b{35m" }, // Dark Purple
            { "§6", "\x1b{33m" }, // Gold
            { "§7", "\x1b{37m" }, // Gray
            { "§8", "\x1b{90m" }, // Dark Gray
            { "§9", "\x1b{94m" }, // Blue
            { "§a", "\x1b{92m" }, // Green
            { "§b", "\x1b{96m" }, // Aqua
            { "§c", "\x1b{91m" }, // Red
            { "§d", "\x1b{95m" }, // Light Purple
            { "§e", "\x1b{93m" }, // Yellow
            { "§f", "\x1b{97m" }, // White
            { "§r", "\x1b{0m"  }, // return plain text
        };

        private readonly Dictionary<string, ConsoleColor> colorReference = new()
        {
            { "§0", ConsoleColor.Black }, // Black
            { "§1", ConsoleColor.DarkBlue }, // Dark Blue
            { "§2", ConsoleColor.DarkGreen }, // Dark Green
            { "§3", ConsoleColor.DarkCyan }, // Dark Aqua
            { "§4", ConsoleColor.DarkRed }, // Dark Red
            { "§5", ConsoleColor.DarkMagenta }, // Dark Purple
            { "§6", ConsoleColor.Yellow }, // Gold
            { "§7", ConsoleColor.Gray }, // Gray
            { "§8", ConsoleColor.DarkGray }, // Dark Gray
            { "§9", ConsoleColor.Blue }, // Blue
            { "§a", ConsoleColor.Green }, // Green
            { "§b", ConsoleColor.Cyan }, // Aqua
            { "§c", ConsoleColor.Red }, // Red
            { "§d", ConsoleColor.Magenta }, // Light Purple
            { "§e", ConsoleColor.Yellow }, // Yellow
            { "§f", ConsoleColor.White }, // White
            { "§r", ConsoleColor.White  }, // return plain text
        };
        public string GetNameSpace()
        {
            return nameSpace;
        }
        public static string GetFormatTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public string ReplaceMinecraftColors(string content)
        {
            MatchCollection matches = GetLoggerRegex().Matches(content);
            foreach (Match m in matches.Cast<Match>())
            {
                if (colorMap[m.Value] != null)
                {
                    content = content.Replace(m.Value, colorMap[m.Value]);
                }
            }
            return content;
        }

        public void Info(string message, string timeString = null) {
            //message = ReplaceMinecraftColors(message);
            if (message.Contains('\n'))
            {
                foreach (string i in message.Split("\r*\n"))
                {
                    Log(nameSpace, i,timeString);
                }
            }
            else
            {
                Log(nameSpace, message,timeString);
            }
        }
        public void InfoFormat(string format, params object[] args) {
            string formated = string.Format("[{0}][{1}]", GetFormatTime(), nameSpace) + string.Format(format, args);
            if (formated.Contains('\n'))
            {
                foreach (string i in formated.Split('\n'))
                {
                    Log(nameSpace, i);
                }
            }
            else
            {
                Log(nameSpace, formated);
            }
        }

        private void Log(string nameSpace,string content,string timeString = null)
        {
            //Console.WriteLine(string.Format("[{0}][{1}] ", timeString ?? GetFormatTime(), nameSpace) + content);
            Console.WriteLine(string.Format("[{0}][{1}] ", timeString ?? GetFormatTime(), nameSpace) + GetLoggerRegex().Replace(content,""));
            /*
            string[] SplitedContent = GetLoggerRegex().Split(content);
            MatchCollection maches = GetLoggerRegex().Matches(content);
            if (maches == null || maches.Count == 0)
            {
                Console.WriteLine(string.Format("[{0}][{1}] ", timeString ?? GetFormatTime(), nameSpace) + content);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(string.Format("[{0}][{1}] ", timeString ?? GetFormatTime(), nameSpace));
                if (content.StartsWith('§'))
                {
                    for (int i = 0;i < maches.Count; i++)
                    {
                        Console.Write("\n" + maches[i].Value + "----" + SplitedContent[i] + "\n");
                        if (!colorReference.TryGetValue(maches[i].Value,out ConsoleColor cc))
                        {
                            Console.Write(SplitedContent[i]);
                        }
                        else
                        {
                            Console.ForegroundColor = cc;
                            Console.Write(SplitedContent[i]);
                        }
                    }
                    Console.Write('\n');
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (SplitedContent.Length == maches.Count + 1)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(SplitedContent[0]);
                    for (int i = 0; i < maches.Count; i++)
                    {
                        Console.Write("\n" + maches[i].Value + "----" + SplitedContent[i + 1] + "\n");
                        if (!colorReference.TryGetValue(maches[i].Value, out ConsoleColor cc))
                        {
                            Console.Write(SplitedContent[i + 1]);
                        }
                        else
                        {
                            Console.ForegroundColor = cc;
                            Console.Write(SplitedContent[i + 1]);
                        }
                    }
                    Console.Write('\n');
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.Write(GetLoggerRegex().Replace(content, "") + "\n");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            */
        }

        public static void GeneralLog(string nameSpace,string content)
        {
            Console.WriteLine(string.Format("[{0}][{1}] ", GetFormatTime(), nameSpace));
        }

        [GeneratedRegex("§[0-9a-fr]")]
        private static partial Regex GetLoggerRegex();
    }
}
