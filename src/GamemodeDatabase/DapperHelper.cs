using Newtonsoft.Json;
using System.IO;

namespace GamemodeDatabase
{
    public static class DapperHelper
    {
        public static string ConnectionString
        {
            get
            {
                using (var r = new StreamReader(@"gamemode\netcoreapp2.2\Settings.json"))
                {
                    var json = r.ReadToEnd();
                    dynamic items = JsonConvert.DeserializeObject(json);
                    return items["connectionString"];
                }
            }
        }
    }
}