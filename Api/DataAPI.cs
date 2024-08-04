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
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("subDataPath"))
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                    return;
                }
                else
                {
                    string dataFilePath = Path.Join(Program.pluginsRoot, ReqJSON["namespace"].Value<string>(), "data", ReqJSON["subDataPath"].Value<string>());
                    string dataFileDirname = Path.GetDirectoryName(dataFilePath);
                    if (!File.Exists(dataFilePath))
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                        return;
                    }
                    else
                    {
                        Stream stream = context.Response.OutputStream;
                        byte[] returnByteArr = Encoding.UTF8.GetBytes(FileIO.ReadFile(dataFilePath));
                        stream.Write(returnByteArr, 0, returnByteArr.Length);
                        context.Response.StatusCode = 200;
                        context.Response.Close();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                context.Response.StatusCode = 400;
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
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("subDataPath") || !ReqJSON.ContainsKey("data"))
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                    return;
                }
                else
                {
                    string dataFilePath = Path.Join(Program.pluginsRoot, ReqJSON["namespace"].Value<string>(), "data", ReqJSON["subDataPath"].Value<string>());
                    string dataFileDirname = Path.GetDirectoryName(dataFilePath);
                    FileIO.EnsureFile(dataFilePath);
                    FileIO.WriteFile(dataFilePath,ReqJSON["data"].Value<string>());
                    context.Response.StatusCode = 200;
                    context.Response.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                context.Response.StatusCode = 400;
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
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("subDataPath"))
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                    return;
                }
                else
                {
                    string dataFilePath = Path.Join(Program.pluginsRoot, ReqJSON["namespace"].Value<string>(), "data", ReqJSON["subDataPath"].Value<string>());
                    string dataFileDirname = Path.GetDirectoryName(dataFilePath);
                    if (!File.Exists(dataFilePath))
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                        return;
                    }
                    else
                    {
                        File.Delete(dataFilePath);
                        context.Response.StatusCode = 200;
                        context.Response.Close();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                context.Response.StatusCode = 400;
                context.Response.Close();
                return;
            }
        }
    }
}
