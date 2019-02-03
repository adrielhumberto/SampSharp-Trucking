using System.IO;
using Newtonsoft.Json;

namespace GamemodeDatabase
{
    public static class DapperHelper
    {
        public static string ConnectionString { get; set; }


        public static void LoadConnectionString()
        {
            using (var r = new StreamReader(@"gamemode\netcoreapp2.2\Settings.json"))
            {
                var json = r.ReadToEnd();
                dynamic items = JsonConvert.DeserializeObject(json);
                ConnectionString = items["connectionString"];
            }
        }
    }
}