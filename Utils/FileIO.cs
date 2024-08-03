using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace bedrock.NetHub.Utils
{
    public class FileIO
    {
        public static void EnsurePath(string tPath)
        {
            if (!Path.Exists(tPath))
            {
                Directory.CreateDirectory(tPath);
                return;
            }
            else
            {
                return;
            }
        }
        public static void EnsureFile(string tPath)
        {
            if (!File.Exists(tPath))
            {
                File.Create(tPath);
                return;
            }
            else
            {
                return;
            }
        }

        public static string ReadFile(string tPath)
        {
            StreamReader file = new(tPath);
            return file.ReadToEnd();
        }

        public static void WriteFile(string tPath,string Content)
        {
            StreamWriter file = new(tPath);
            file.Write(Content);
            return;
        }

        public static JObject ReadAsJSON (string tPath)
        {

            return JObject.Parse(ReadFile(tPath));
        }

        public static void WriteAsJSON<T>(string tPath,T Content)
        {
            WriteFile(tPath,JsonConvert.SerializeObject(Content));
            return;
        }

        public static void WriteAsJSON(string tPath, JObject Content)
        {
            WriteFile(tPath, JsonConvert.SerializeObject(Content));
            return;
        }

        public static Dictionary<string, string> ReadProperties(string filePath)
        {
            Dictionary<string, string> properties = [];
            if (File.Exists(filePath))
            {
                foreach (string line in File.ReadAllLines(filePath, Encoding.UTF8))
                {
                    string trimmedLine = line.Trim();
                    if (!string.IsNullOrEmpty(trimmedLine) && !trimmedLine.StartsWith('#'))
                    {
                        var separatorIndex = trimmedLine.IndexOf('=');
                        if (separatorIndex > 0)
                        {
                            string key = trimmedLine[..separatorIndex].Trim();
                            string value = trimmedLine[(separatorIndex + 1)..].Trim();
                            properties[key] = value;
                        }
                    }
                }
            }
            return properties;
        }
    }
}
