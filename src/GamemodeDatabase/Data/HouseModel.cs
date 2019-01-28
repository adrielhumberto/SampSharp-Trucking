using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace GamemodeDatabase.Data
{
    public class HouseModel
    {
        public int Id { get; set; }

        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public byte Level { get; set; }
        public byte MaxLevel { get; set; }
        public byte Owned { get; set; }

        public int Price { get; set; }

        public string Owner { get; set; }

        public static List<HouseModel> GetAllHouses
        {
            get
            {
                using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                {
                    return db.Query<HouseModel>("SELECT * FROM houses").ToList();
                }
            }
        }
    }
}