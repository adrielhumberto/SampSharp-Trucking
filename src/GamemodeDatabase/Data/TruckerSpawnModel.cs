using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace GamemodeDatabase.Data
{
    public class TruckerSpawnModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Angle { get; set; }

        public static List<TruckerSpawnModel> GetTruckerSpawnListNoTracking
        {
            get
            {
                using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                {
                    return db.Query<TruckerSpawnModel>("SELECT * FROM truckerspawns").ToList();
                }
            }
        }
    }
}