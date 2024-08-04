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
                if (!File.Exists(tPath))
                {
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                    return;
                }
                else
                {
                    Stream stream = context.Response.OutputStream;
                    byte[] returnByteArr = Encoding.UTF8.GetBytes(FileIO.ReadFile(tPath));
                    stream.Write(returnByteArr, 0, returnByteArr.Length);
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
    }
}
