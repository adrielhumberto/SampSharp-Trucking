using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace GamemodeDatabase.Data
{
    public class RadioModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public static List<RadioModel> GetRadioStations
        {
            get
            {
                using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                {
                    return db.Query<RadioModel>("SELECT * FROM radio").ToList();
                }
            }
        }
    }
}