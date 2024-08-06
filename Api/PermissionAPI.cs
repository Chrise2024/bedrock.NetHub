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
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    if (ReqJSON.ContainsKey("extendsFrom"))
                    {
                        PermissionsGroupManager.CreateGroup(ReqJSON["groupName"].Value<string>(), ReqJSON["extendsFrom"].Value<string>());
                        Http.WriteRequest(context, 200, "{}");
                    }
                    else
                    {
                        PermissionsGroupManager.CreateGroup(ReqJSON["groupName"].Value<string>(),"");
                        Http.WriteRequest(context, 200, "{}");
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void DeleteGroup(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName") || !ReqJSON["groupName"].Value<string>().Equals("default"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    PermissionsGroupManager.DeleteGroup(ReqJSON["groupName"].Value<string>());
                    Http.WriteRequest(context, 200, "{}");
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void GrantPermission(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName") || !ReqJSON.ContainsKey("permission"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    PermissionsGroupManager.GrantPermissionToGroup(ReqJSON["groupName"].Value<string>(), ReqJSON["permission"].Value<string>());
                    Http.WriteRequest(context, 200, "{}");
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void RevokePermission(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName") || !ReqJSON.ContainsKey("permission"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    PermissionsGroupManager.RevokePermissionFromGroup(ReqJSON["groupName"].Value<string>(), ReqJSON["permission"].Value<string>());
                    Http.WriteRequest(context, 200, "{}");
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void TestPermission(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("xuid") || !ReqJSON.ContainsKey("permission"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    bool res = PermissionsGroupManager.TestPermission(ReqJSON["xuid"].Value<string>(), ReqJSON["permission"].Value<string>());
                    Http.WriteRequest<bool>(context, 200, res);
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void AddPlayerToGroup(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("xuid") || !ReqJSON.ContainsKey("groupName"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    PermissionsGroupManager.AddPlayerToGroup(ReqJSON["xuid"].Value<string>(), ReqJSON["groupName"].Value<string>());
                    Http.WriteRequest(context, 200, "{}");
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void RemovePlayerFromGroup(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("xuid") || !ReqJSON.ContainsKey("groupName"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    PermissionsGroupManager.RemovePlayerFromGroup(ReqJSON["xuid"].Value<string>(), ReqJSON["groupName"].Value<string>());
                    Http.WriteRequest(context, 200, "{}");
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void ListGroupsOfPlayer(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("xuid"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    Http.WriteRequest<List<string>>(context, 200, PermissionsGroupManager.GetGroupsOfPlayer(ReqJSON["xuid"].Value<string>()));
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void ListPlayersInGroup(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    Http.WriteRequest<List<string>>(context, 200, PermissionsGroupManager.GetPlayersInGroup(ReqJSON["groupName"].Value<string>()));
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void ListExplicitPermissions(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    Http.WriteRequest<List<string>>(context, 200, PermissionsGroupManager.GetExplicitPermissionsOfGroup(ReqJSON["groupName"].Value<string>()));
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void ListAllPermissions(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("groupName"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    Http.WriteRequest(context, 200, new { permissions = PermissionsGroupManager.GetAllPermissionsOfGroup(ReqJSON["groupName"].Value<string>()) });
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void ListAllGroups(HttpListenerContext context)
        {
            try
            {
                Http.WriteRequest<List<string>>(context, 200, PermissionsGroupManager.GetGroups());
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }
    }
}
