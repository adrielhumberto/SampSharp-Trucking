using System.Collections.Generic;
using Dapper;
using GamemodeDatabase;
using GamemodeDatabase.Data;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.SAMP;
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

        public void UpdateHouseVisuals()
        {
            if (HouseData().Owned)
            {
                HousePickup.ModelId = 1272;
                MapIcon.Type = 32;
                TextLabel.Text = string.Format(StaticTexts.TextHouseOwned,
                    Color.LightGreen, Color.White, DbId, HouseData().Owner, HouseData().Level);
            }
            else
            {
                HousePickup.ModelId = 1273;
                MapIcon.Type = 31;
                TextLabel.Text = string.Format(StaticTexts.TextHouseForSale,
                    Color.LightGreen, Color.White, DbId, HouseData().Price, HouseData().MaxLevel);
            }
        }

        public int CalculateSellPrice()
        {
            // Calculate 50% of the original buying price (base-price for selling)
            var sellPrice = HouseData().Price / 2;
            // Calculate the number of upgrades applied to the house
            var numUpgrades = HouseData().Level - 1;
            // Also calculate 50% for each upgrade, based on the percentage for upgrading the house
            var upgradePrice = HouseData().Price / 100 * Config.HouseUpgradePercent * numUpgrades;

            // Add 50% of the upgrade-price to the sell-price
            sellPrice = sellPrice + upgradePrice;

            return sellPrice;
        }
    }
}