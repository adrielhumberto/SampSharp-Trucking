using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace GamemodeDatabase.Data
{
    public class HouseInteriorModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float Angle { get; set; }

        public int InteriorId { get; set; }

        public static List<HouseInteriorModel> GetHouseInteriors()
        {
            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                return db.Query<HouseInteriorModel>(@"SELECT * FROM houseinteriors").ToList();
            }
        }
    }
}