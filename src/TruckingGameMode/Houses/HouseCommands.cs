using System.Linq;
using Dapper;
using GamemodeDatabase;
using GamemodeDatabase.Data;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
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

            var house = House.Houses.Find(x => x.Position.DistanceTo(sender.Position) < 4.0f);
            if (house is null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are not close to any house.");
                return;
            }

            if (house.HouseData().Owned)
            {
                sender.SendClientMessage(Color.IndianRed, "The house is already owned.");
                return;
            }

            if (sender.Money <= house.HouseData().Price)
            {
                sender.SendClientMessage(Color.IndianRed,
                    "You don't have enough money to buy the house.");
                return;
            }


            sender.Money -= house.HouseData().Price;
            sender.SendClientMessage(Color.GreenYellow,
                $"You bought house id {house.DbId} for ${house.HouseData().Price}.");

            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                db.Execute(@"UPDATE houses SET Owned = true, Owner = @Owner WHERE Id = @Id", new
                {
                    Owner = sender.Name,
                    Id = house.DbId
                });
            }

            house.UpdateHouseVisuals();
        }

        [Command("enter")]
        public static void OnEnterCommand(Player sender)
        {
            if (sender.CurrentHouse != null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are already in a house.");
                return;
            }

            var house = House.Houses.Find(x => x.Position.DistanceTo(sender.Position) < 4.0f);
            if (house is null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are not near a house.");
                return;
            }


            if (house.HouseData().Owned == false)
            {
                sender.SendClientMessage(Color.IndianRed, "The house is not owned.");
                return;
            }


            var houseInterior = HouseInteriorModel.GetHouseInteriors()
                .Find(x => x.Level == house.HouseData().Level);
            sender.Interior = houseInterior.InteriorId;
            sender.Position = new Vector3(houseInterior.PositionX, houseInterior.PositionY,
                houseInterior.PositionZ);
            sender.Angle = houseInterior.Angle;
            sender.CurrentHouse = house;
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

        [Command("housemenu")]
        public static void OnHouseMenuCommand(Player sender)
        {
            if (sender.CurrentHouse is null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are not in your house.");
                return;
            }

            var menuDialog = new ListDialog("Chose an option", "Chose", "Cancel");
            menuDialog.AddItem("Upgrade house");
            menuDialog.AddItem("Sell house");
            menuDialog.Show(sender);

            menuDialog.Response += (obj, e) =>
            {
                if(e.DialogButton == DialogButton.Right)
                    return;

                if (e.ListItem == 0)
                {
                    if (sender.CurrentHouse.HouseData().Level >= Config.MaxHouseLevel)
                    {
                        sender.SendClientMessage(Color.IndianRed, "House is already at maximum level.");
                        return;
                    }


                    var upgradePrice = ((sender.CurrentHouse.HouseData().Price * (sender.CurrentHouse.HouseData().Level + 1)) / 100) *
                                       Config.HouseUpgradePercent;

                    if (sender.Money < upgradePrice)
                    {
                        sender.SendClientMessage(Color.IndianRed, $"You don't have enough money. You need ${upgradePrice}.");
                        return;
                    }

                    sender.Money -= upgradePrice;
                    sender.SendClientMessage(Color.GreenYellow, $"You upgraded your house for ${upgradePrice}.");

                    using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                    {
                        db.Execute(@"UPDATE houses SET Level = Level + 1 WHERE Id = @Id", new {Id = sender.CurrentHouse.DbId});
                    }

                    var houseInterior = HouseInteriorModel.GetHouseInteriors()
                        .Find(x => x.Level == sender.CurrentHouse.HouseData().Level);
                    sender.Interior = houseInterior.InteriorId;
                    sender.Position = new Vector3(houseInterior.PositionX, houseInterior.PositionY,
                        houseInterior.PositionZ);
                    sender.Angle = houseInterior.Angle;

                    sender.CurrentHouse.UpdateHouseVisuals();
                }
                else
                {

                    var sellPrice = sender.CurrentHouse.CalculateSellPrice();
                    sender.Money += sellPrice;
                    sender.SendClientMessage(Color.GreenYellow, $"You sold your house for ${sellPrice}.");

                    using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                    {
                        db.Execute(@"UPDATE houses SET Owned = false, Level = 1, Owner = '' WHERE Id = @Id", new {Id = sender.CurrentHouse.DbId});
                    }

                    sender.CurrentHouse.UpdateHouseVisuals();

                    sender.Position = sender.CurrentHouse.Position;
                    sender.Interior = 0;
                    sender.CurrentHouse = null;
                }
            };
        }

        #endregion

        #region Admin Commands

        [Command("createhouse", PermissionChecker = typeof(LevelTwoAdminPermission))]
        public static async void OnCreateHouseCommand(BasePlayer sender, int price, int maxLevel)
        {
            if (maxLevel < 1 || maxLevel > Config.MaxHouseLevel)
            {
                sender.SendClientMessage(Color.IndianRed, "Invalid max level.");
                return;
            }


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
                houseDbId = await db.QuerySingleAsync<int>(@"SELECT LAST_INSERT_ID()");
            }

            House.Houses.Add(new House(houseDbId)
            {
                MapIcon =
                    new DynamicMapIcon(new Vector3(sender.Position.X, sender.Position.Y, sender.Position.Z), 31),
                TextLabel = new DynamicTextLabel(
                    string.Format(StaticTexts.TextHouseForSale, Color.LightGreen, Color.White, houseDbId, price,
                        maxLevel),
                    Color.Teal, new Vector3(sender.Position.X, sender.Position.Y, sender.Position.Z + 1.0), 5.0f),
                HousePickup = new DynamicPickup(1273, 1,
                    new Vector3(sender.Position.X, sender.Position.Y, sender.Position.Z), 10.0f)
            });
        }

        [Command("deletehouse", PermissionChecker = typeof(LevelTwoAdminPermission))]
        public static void OnDeleteHouseCommand(BasePlayer sender)
        {
            var house = House.Houses.Find(x => x.Position.DistanceTo(sender.Position) < 4.0f);
            if (house is null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are not near any house.");
                return;
            }

            if (house.HouseData().Owned)
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
        }

        [Command("evicthouse", PermissionChecker = typeof(LevelTwoAdminPermission))]
        public static void OnEvictHouseCommand(BasePlayer sender)
        {
            var house = House.Houses.Find(x => x.Position.DistanceTo(sender.Position) < 4.0f);
            if (house is null)
            {
                sender.SendClientMessage(Color.IndianRed, "You a re not near any house.");
                return;
            }

            if (house.HouseData().Owned == false)
            {
                sender.SendClientMessage(Color.IndianRed, "This house its not owned by anyone.");
                return;
            }

            var basePlayer = BasePlayer.All.First(x => x.Name == house.HouseData().Owner);
            basePlayer.SendClientMessage(Color.IndianRed,
                $"You have been evicted from your house id {house.DbId} by an admin.");

            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                db.Execute(@"UPDATE houses SET Owned = false, Owner = ' ' WHERE Id = @id",
                    new {id = house.DbId});
            }

            sender.SendClientMessage(Color.GreenYellow, $"You evicted the owner of house id {house.DbId}.");

            house.UpdateHouseVisuals();
        }

        [Command("gotohouse", PermissionChecker = typeof(LevelTwoAdminPermission))]
        public static void OnGoToHouseCommand(BasePlayer sender, int houseId)
        {
            var house = House.Houses.Find(x => x.DbId == houseId);

            if (house is null)
            {
                sender.SendClientMessage(Color.IndianRed, "The house doesn't exist.");
                return;
            }

            sender.Position = house.Position;
            sender.SendClientMessage(Color.GreenYellow, $"You have been teleported to house id {houseId}.");
        }

        #endregion
    }
}