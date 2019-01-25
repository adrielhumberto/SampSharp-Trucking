using System.Linq;
using Dapper;
using GamemodeDatabase;
using GamemodeDatabase.Data;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using SampSharp.Streamer.World;
using TruckingGameMode.Commands.AdminCommands.AdminCommandPermissions;
using TruckingGameMode.World;

namespace TruckingGameMode.Houses
{
    public class HouseCommands
    {
        #region Player commands

        [Command("buyhouse")]
        public static void OnBuyHouseCommand(Player sender)
        {
            var ownedHouse = House.Houses.Find(x => x.HouseData().Owner == sender.Name);
            if (ownedHouse != null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are already owning a house.");
                return;
            }

            var houseId = 0;

            foreach (var house in House.Houses)
                if (sender.Position.DistanceTo(house.Position) < 4.0f)
                {
                    if (house.HouseData().Owned == 0)
                    {
                        if (sender.Money >= house.HouseData().Price)
                        {
                            houseId = house.DbId;
                            sender.Money -= house.HouseData().Price;
                            sender.SendClientMessage(Color.GreenYellow,
                                $"You bought house id {house.DbId} for ${house.HouseData().Price}.");
                            break;
                        }

                        sender.SendClientMessage(Color.IndianRed,
                            "You don't have enough money to buy the house.");
                        return;
                    }

                    sender.SendClientMessage(Color.IndianRed, "The house is already owned.");
                    return;
                }
                else
                {
                    sender.SendClientMessage(Color.IndianRed, "You are not close to any house.");
                    return;
                }

            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                db.Execute(@"UPDATE houses SET Owned = 1, Owner = @Owner WHERE Id = @Id", new
                {
                    Owner = sender.Name,
                    Id = houseId
                });
            }

            var houseToUpdate = House.Houses.Find(x => x.DbId == houseId);
            houseToUpdate.HousePickup.ModelId = 1272;
            houseToUpdate.MapIcon.Type = 32;
            houseToUpdate.TextLabel.Text =
                $"ID: {houseId}\nHouse Owner: {sender.Name}\nHouse Level: {houseToUpdate.HouseData().Level}\nType /enter to enter the house";
        }

        [Command("enter")]
        public static void OnEnterCommand(Player sender)
        {
            if (sender.CurrentHouse != null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are already in a house.");
                return;
            }

            foreach (var house in House.Houses)
            {
                if (house.HouseData().Owned == 0)
                {
                    sender.SendClientMessage(Color.IndianRed, "The house is not owned.");
                    return;
                }

                if (sender.Position.DistanceTo(house.Position) < 4.0f)
                {
                    var houseInterior = HouseInteriorModel.GetHouseInteriors()
                        .Find(x => x.Level == house.HouseData().Level);
                    sender.Interior = houseInterior.InteriorId;
                    sender.Position = new Vector3(houseInterior.PositionX, houseInterior.PositionY,
                        houseInterior.PositionZ);
                    sender.Angle = houseInterior.Angle;
                    sender.CurrentHouse = house;
                }
                else
                {
                    sender.SendClientMessage(Color.IndianRed, "You are not near any house.");
                    return;
                }
            }
        }

        [Command("exit")]
        public static void OnExitCommand(Player sender)
        {
            if (sender.CurrentHouse is null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are not in any house.");
                return;
            }

            sender.Interior = 0;
            sender.Position = sender.CurrentHouse.Position;

            sender.CurrentHouse = null;
        }

        [Command("gohome")]
        public static void OnGoHomeCommand(Player sender)
        {
            var house = House.Houses.Find(x => x.HouseData().Owner == sender.Name);
            if (house is null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are not owning any house.");
                return;
            }

            sender.Position = house.Position;
            sender.SendClientMessage(Color.GreenYellow, "You got teleported to your house.");
        }

        #endregion

        #region Admin Commands

        [Command("createhouse", PermissionChecker = typeof(LevelTwoAdminPermission))]
        public static async void OnCreateHouseCommand(BasePlayer sender, int price, int maxLevel)
        {
            int houseDbId;
            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                await db.ExecuteAsync(
                    @"INSERT INTO houses (PositionX, PositionY, PositionZ, MaxLevel, Price) VALUES (@PositionX, @PositionY, @PositionZ, @MaxLevel, @Price)",
                    new
                    {
                        PositionX = sender.Position.X,
                        PositionY = sender.Position.Y,
                        PositionZ = sender.Position.Z,
                        MaxLevel = maxLevel,
                        Price = price
                    });
                houseDbId = db.QuerySingle<int>(@"SELECT LAST_INSERT_ID()");
            }

            House.Houses.Add(new House(houseDbId)
            {
                MapIcon =
                    new DynamicMapIcon(new Vector3(sender.Position.X, sender.Position.Y, sender.Position.Z), 31),
                TextLabel = new DynamicTextLabel(
                    $"ID: {houseDbId}\nHouse Price: ${price:##,###}\nHouse Max Level: {maxLevel}\nType /buyhouse to buy it.",
                    Color.Teal, new Vector3(sender.Position.X, sender.Position.Y, sender.Position.Z + 1.0), 5.0f),
                HousePickup = new DynamicPickup(1273, 1,
                    new Vector3(sender.Position.X, sender.Position.Y, sender.Position.Z), 10.0f)
            });
        }

        [Command("deletehouse", PermissionChecker = typeof(LevelTwoAdminPermission))]
        public static void OnDeleteHouseCommand(BasePlayer sender)
        {
            foreach (var house in House.Houses)
            {
                if (!(sender.Position.DistanceTo(house.Position) < 4.0f)) continue;

                if (house.HouseData().Owned == 1)
                {
                    var basePlayer = BasePlayer.All.First(x => x.Name == house.HouseData().Owner);
                    basePlayer.SendClientMessage(Color.IndianRed,
                        $"Your house with id {house.DbId} has been deleted by an admin.");
                }

                using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                {
                    db.Execute(@"DELETE FROM houses WHERE Id = @id", new {id = house.DbId});
                }

                sender.SendClientMessage(Color.GreenYellow, $"You successfully deleted house id {house.DbId}.");

                house.HousePickup.Dispose();
                house.MapIcon.Dispose();
                house.TextLabel.Dispose();
                House.Houses.Remove(house);
                House.Houses.TrimExcess();

                break;
            }
        }

        [Command("evicthouse", PermissionChecker = typeof(LevelTwoAdminPermission))]
        public static void OnEvictHouseCommand(BasePlayer sender)
        {
            foreach (var house in House.Houses)
            {
                if (!(sender.Position.DistanceTo(house.Position) < 4.0f)) continue;

                if (house.HouseData().Owned == 1)
                {
                    var basePlayer = BasePlayer.All.First(x => x.Name == house.HouseData().Owner);
                    basePlayer.SendClientMessage(Color.IndianRed,
                        $"You have been evicted from your house id {house.DbId} by an admin.");

                    using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                    {
                        db.Execute(@"UPDATE houses SET Owned = 0, Owner = ' ' WHERE Id = @id",
                            new {id = house.DbId});
                    }

                    sender.SendClientMessage(Color.GreenYellow, $"You evicted the owner of house id {house.DbId}.");

                    house.HousePickup.ModelId = 1273;
                    house.MapIcon.Type = 31;
                    house.TextLabel.Text =
                        $"ID: {house.DbId}\nHouse Price: ${house.HouseData().Price:##,###}\nHouse Max Level: {house.HouseData().MaxLevel}\nType /buyhouse to buy it.";
                }
                else
                {
                    sender.SendClientMessage(Color.IndianRed, "This house its not owned by anyone.");
                    return;
                }
            }
        }

        #endregion
    }
}