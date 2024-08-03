using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace bedrock.NetHub.Utils
{
    public static class JObjectExtensions
    {
        public static T ToStruct<T>(this JObject jObject) where T : struct
        {
            T result = Activator.CreateInstance<T>();

            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (jObject.TryGetValue(property.Name, out JToken token))
                {
                    Console.WriteLine(property.Name);
                    property.SetValue(result, token.ToObject(property.PropertyType));
                }
            }
            return result;
        }
    }

    public abstract class TypeCast
    {
        public static List<int> VersionStringToArray(string versionString)
        {
            string[] sList = versionString.Split('.');
            if (sList.Length == 1 ) {
                if (int.TryParse(sList[0], out int value)) {
                    return new List<int>(value);
                }
                else {
                    return [];
                }
            }
            else
            {
                List<int> IntArray = [];
                foreach (string s in sList)
                {
                    if (int.TryParse(s, out int value))
                    {
                        IntArray.Add(value);
                    }
                    else
                    {
                        return [];
                    }
                }
                return IntArray;
            }
        }
    }
}
