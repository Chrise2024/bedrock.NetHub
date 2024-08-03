using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace bedrock.NetHub.service
{
    public partial class Logger(string nameSpace)
    {
        private readonly string nameSpace = nameSpace;
        private readonly Dictionary<string,string> colorMap = new Dictionary<string, string> {
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
                    content = content.Replace(m.Value, (string?)colorMap[m.Value]);
                }
            }
            return content;
        }

        public void Info(string message, string timeString = null) {
            message = ReplaceMinecraftColors(message);
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
            formated = ReplaceMinecraftColors(formated);
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
            if (timeString == null)
            {
                Console.WriteLine(string.Format("[{0}][{1}] ", GetFormatTime(), nameSpace) + content);
            }
            else
            {
                Console.WriteLine(string.Format("[{0}][{1}] ", timeString, nameSpace) + content);
            }
        }

        [GeneratedRegex("§[0-9a-fr]")]
        private static partial Regex GetLoggerRegex();
    }
}
