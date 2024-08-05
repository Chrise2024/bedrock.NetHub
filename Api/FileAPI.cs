using bedrock.NetHub.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace bedrock.NetHub.Api
{
    public abstract class FileAPI
    {
        public static void Read(HttpListenerContext context)
        {
            try
            {
                StreamReader sr = new(context.Request.InputStream);
                string tPath = sr.ReadToEnd();
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (!File.Exists(tPath))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 404;
                }
                else
                {
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
