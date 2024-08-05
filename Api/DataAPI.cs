using bedrock.NetHub.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace bedrock.NetHub.Api
{
    public abstract class DataAPI
    {
        public static void Read(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("subDataPath"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    string dataFilePath = Path.Join(Program.pluginsRoot, ReqJSON["namespace"].Value<string>(), "data", ReqJSON["subDataPath"].Value<string>());
                    string dataFileDirname = Path.GetDirectoryName(dataFilePath);
                    if (!File.Exists(dataFilePath))
                    {
                        writer.Write("{}");
                        context.Response.StatusCode = 404;
                    }
                    else
                    {
                        writer.Write(FileIO.ReadFile(dataFilePath));
                        context.Response.StatusCode = 200;
                    }
                    writer.Close();
                    context.Response.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                StreamWriter writer = new(context.Response.OutputStream);
                writer.Write("{}");
                context.Response.StatusCode = 400;
                writer.Close();
                context.Response.Close();
                return;
            }
        }

        public static void Write(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("subDataPath") || !ReqJSON.ContainsKey("data"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    string dataFilePath = Path.Join(Program.pluginsRoot, ReqJSON["namespace"].Value<string>(), "data", ReqJSON["subDataPath"].Value<string>());
                    string dataFileDirname = Path.GetDirectoryName(dataFilePath);
                    FileIO.EnsureFile(dataFilePath);
                    FileIO.WriteFile(dataFilePath,ReqJSON["data"].Value<string>());
                    writer.Write("{}");
                    context.Response.StatusCode = 200;
                }
                writer.Close();
                context.Response.Close();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                StreamWriter writer = new(context.Response.OutputStream);
                writer.Write("{}");
                context.Response.StatusCode = 400;
                writer.Close();
                context.Response.Close();
                return;
            }
        }

        public static void Delete(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("subDataPath"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    string dataFilePath = Path.Join(Program.pluginsRoot, ReqJSON["namespace"].Value<string>(), "data", ReqJSON["subDataPath"].Value<string>());
                    string dataFileDirname = Path.GetDirectoryName(dataFilePath);
                    if (!File.Exists(dataFilePath))
                    {
                        writer.Write("{}");
                        context.Response.StatusCode = 404;
                    }
                    else
                    {
                        File.Delete(dataFilePath);
                        writer.Write("{}");
                        context.Response.StatusCode = 200;
                    }
                }
                writer.Close();
                context.Response.Close();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                StreamWriter writer = new(context.Response.OutputStream);
                writer.Write("{}");
                context.Response.StatusCode = 400;
                writer.Close();
                context.Response.Close();
                return;
            }
        }
    }
}
