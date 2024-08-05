using bedrock.NetHub.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace bedrock.NetHub.Api
{
    public abstract class XuidAPI
    {
        public static void GetXuidByName(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("name"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    writer.Write(Program.GetXuidManager().GetXuidByName(ReqJSON["name"].Value<string>()));
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

        public static void GetNameByXuid(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("xuid"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    writer.Write(Program.GetXuidManager().GetNameByXuid(ReqJSON["xuid"].Value<string>()));
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
