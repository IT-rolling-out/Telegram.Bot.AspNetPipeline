using System;
using System.IO;
using Newtonsoft.Json.Linq;
using File = System.IO.File;

namespace IRO.Samples.Standalone
{
    public static class BotTokenResolver
    {
        public static string GetToken()
        {
            return LoadJson()["token"].ToObject<string>();
        }

        public static string GetSecondToken()
        {
            return LoadJson()["secondToken"].ToObject<string>();
        }

        static JToken LoadJson()
        {
            try
            {
                string jsonStr = null;
                try
                {
                    var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\..\\",
                        "test_token.json"));
                    jsonStr = File.ReadAllText(path);
                }
                catch
                {
                    var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\..\\..\\",
                        "test_token.json"));
                    jsonStr = File.ReadAllText(path);
                }
                var jToken = JToken.Parse(jsonStr);
                return jToken;
            }
            catch (Exception ex)
            {
                throw new Exception("Wrong token. Please, check 'test_token.json' exists in solution folder.", ex);
            }
        }
    }
}
