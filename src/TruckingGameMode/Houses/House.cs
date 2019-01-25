using System.Collections.Generic;
using Dapper;
using GamemodeDatabase;
using GamemodeDatabase.Data;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.Streamer.World;

namespace TruckingGameMode.Houses
{
    public class House
    {
        public House(int dbId)
        {
            DbId = dbId;
        }

        public int DbId { get; }

        public DynamicTextLabel TextLabel { get; set; }
        public DynamicMapIcon MapIcon { get; set; }
        public DynamicPickup HousePickup { get; set; }

        public Vector3 Position => new Vector3(HouseData().PositionX, HouseData().PositionY, HouseData().PositionZ);

        public static List<House> Houses { get; } = new List<House>();

        public static HouseModel HouseData(int dbId)
        {
            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                return db.QueryFirst<HouseModel>(@"SELECT * FROM houses WHERE Id = @id", new {id = dbId});
            }
        }

        public HouseModel HouseData()
        {
            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                return db.QueryFirst<HouseModel>(@"SELECT * FROM houses WHERE Id = @id", new {id = DbId});
            }
        }
    }
}