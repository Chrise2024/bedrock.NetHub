using bedrock.NetHub.service;
using bedrock.NetHub.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace bedrock.NetHub.Api
{
    public abstract class LogAPI
    {
        public static void Log(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("content"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    Logger.GeneralLog(ReqJSON["namespace"].Value<string>(), ReqJSON["content"].Value<string>());
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
    }
}
