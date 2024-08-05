using bedrock.NetHub.Service;
using bedrock.NetHub.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace bedrock.NetHub.Api
{
    public abstract class PermissionAPI
    {
        private static readonly PermissionsGroupManager PermissionsGroupManager = Program.GetPermissionsGroupManager();
        public static void CreateGroup(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    if (ReqJSON.ContainsKey("extendsFrom"))
                    {
                        PermissionsGroupManager.CreateGroup(ReqJSON["groupName"].Value<string>(), ReqJSON["groupName"].Value<string>());
                        writer.Write("{}");
                        context.Response.StatusCode = 200;
                    }
                    else
                    {
                        PermissionsGroupManager.CreateGroup(ReqJSON["groupName"].Value<string>(),"");
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

        public static void DeleteGroup(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName") || !ReqJSON["groupName"].Value<string>().Equals("default"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    PermissionsGroupManager.DeleteGroup(ReqJSON["groupName"].Value<string>());
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

        public static void GrantPermission(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName") || !ReqJSON.ContainsKey("permission"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    PermissionsGroupManager.GrantPermissionToGroup(ReqJSON["groupName"].Value<string>(), ReqJSON["permission"].Value<string>());
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

        public static void RevokePermission(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName") || !ReqJSON.ContainsKey("permission"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    PermissionsGroupManager.RevokePermissionFromGroup(ReqJSON["groupName"].Value<string>(), ReqJSON["permission"].Value<string>());
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

        public static void TestPermission(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("xuid") || !ReqJSON.ContainsKey("permission"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    bool res = PermissionsGroupManager.TestPermission(ReqJSON["xuid"].Value<string>(), ReqJSON["permission"].Value<string>());
                    writer.Write(JsonConvert.SerializeObject(new { data = res }));
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

        public static void AddPlayerToGroup(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("xuid") || !ReqJSON.ContainsKey("groupName"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    PermissionsGroupManager.AddPlayerToGroup(ReqJSON["xuid"].Value<string>(), ReqJSON["groupName"].Value<string>());
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

        public static void RemovePlayerFromGroup(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("xuid") || !ReqJSON.ContainsKey("groupName"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    PermissionsGroupManager.RemovePlayerFromGroup(ReqJSON["xuid"].Value<string>(), ReqJSON["groupName"].Value<string>());
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

        public static void ListGroupsOfPlayer(HttpListenerContext context)
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
                    List<string> groups = PermissionsGroupManager.GetGroupsOfPlayer(ReqJSON["xuid"].Value<string>());
                    writer.Write(JsonConvert.SerializeObject(groups));
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

        public static void ListPlayersInGroup(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    List<string> groups = PermissionsGroupManager.GetPlayersInGroup(ReqJSON["groupName"].Value<string>());
                    writer.Write(JsonConvert.SerializeObject(groups));
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

        public static void ListExplicitPermissions(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    List<string> permissions = PermissionsGroupManager.GetExplicitPermissionsOfGroup(ReqJSON["groupName"].Value<string>());
                    writer.Write(JsonConvert.SerializeObject(permissions));
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

        public static void ListAllPermissions(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    List<string> permissions = PermissionsGroupManager.GetAllPermissionsOfGroup(ReqJSON["groupName"].Value<string>());
                    writer.Write(JsonConvert.SerializeObject(permissions));
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

        public static void ListAllGroups(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                List<string> permissions = PermissionsGroupManager.GetGroups();
                writer.Write(JsonConvert.SerializeObject(permissions));
                context.Response.StatusCode = 200;
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
