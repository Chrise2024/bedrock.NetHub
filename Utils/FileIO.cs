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
        public static void EnsureFile(string tPath,string initData = null)
        {
            if (!File.Exists(tPath))
            {
                EnsurePath(Path.GetDirectoryName(tPath));
                File.Create(tPath).Close();
                if (initData != null)
                {
                    WriteFile(tPath, initData);
                }
                else
                {
                    WriteFile(tPath, string.Empty);
                }
                return;
            }
            else
            {
                return;
            }
        }

        public static void SafeDeleteFile(string tPath)
        {
            if (File.Exists(tPath))
            {
                File.Delete(tPath);
            }
        }

        public static void SafeDeletePath(string tPath)
        {
            if (Path.Exists(tPath))
            {
                Directory.Delete(tPath);
            }
        }

        public static string ReadFile(string tPath)
        {
            StreamReader file = new(tPath);
            string Content = file.ReadToEnd();
            file.Close();
            return Content;
        }

        public static void WriteFile(string tPath,string Content)
        {
            EnsureFile(tPath);
            StreamWriter file = new(tPath);
            file.Write(Content);
            file.Close();
            return;
        }

        public static byte[] ReadAsBytes(string tPath,Encoding targetEncoding = null)
        {
            StreamReader file = new(tPath);
            string Content = file.ReadToEnd();
            file.Close();
            if (targetEncoding == null)
            {
                return Encoding.UTF8.GetBytes(Content);
            }
            else
            {
                return targetEncoding.GetBytes(Content);
            }
        }

        public static JObject ReadAsJSON (string tPath)
        {
            return JObject.Parse(ReadFile(tPath));
        }

        public static T ReadAsJSON<T>(string tPath)
        {
            return JObject.Parse(ReadFile(tPath)).ToObject<T>();
        }
        public static JArray ReadAsJArray(string tPath)
        {

            return JArray.Parse(ReadFile(tPath));
        }

        public static void WriteAsJSON<T>(string tPath,T Content)
        {
            EnsureFile(tPath);
            WriteFile(tPath,JsonConvert.SerializeObject(Content));
            return;
        }

        public static void WriteAsJSON(string tPath, JObject Content)
        {
            EnsureFile(tPath);
            WriteFile(tPath, JsonConvert.SerializeObject(Content));
            return;
        }
        public static void WriteAsJSON(string tPath, JArray Content)
        {
            EnsureFile(tPath);
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
