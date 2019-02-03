using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using GamemodeDatabase;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using TruckingGameMode.Commands.AdminCommands.AdminCommandPermissions;
using TruckingGameMode.World;

namespace TruckingGameMode.Commands.AdminCommands
{
    [CommandGroup("admin", PermissionChecker = typeof(LevelOneAdminPermission))]
    public class LevelOneAdminCommands
    {
        [Command("kick", Shortcut = "kick")]
        public static async void OnKickCommand(BasePlayer sender, Player playerId, string reason)
        {
            if (playerId.GetPlayerDataById().AdminLevel > 0)
            {
                sender.SendClientMessage(Color.IndianRed, "You can't kick other admin.");
                return;
            }

            sender.SendClientMessage(Color.GreenYellow, $"You kicked {playerId.Name}");
            playerId.SendClientMessage(Color.IndianRed,
                $"You have been kicked from the server by {sender.Name}. Reason: {reason}");

            await Task.Delay(Config.KickDelay);
            playerId.Kick();
        }

        [Command("repair", Shortcut = "repair")]
        public static void OnRepairCommand(BasePlayer sender)
        {
            if (sender.Vehicle == null || sender.Vehicle?.Driver != sender)
            {
                sender.SendClientMessage(Color.Wheat, "You are not driving a car.");
                return;
            }

            sender.Vehicle?.Repair();
        }

        [Command("clearchat", Shortcut = "cc")]
        public static void OnClearChatCommand(BasePlayer sender)
        {
            for (var i = 0; i < 100; i++) BasePlayer.SendClientMessageToAll(Color.White, "");
        }

        [Command("announce", Shortcut = "ann")]
        public static void OnAnnounceCommand(BasePlayer sender, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            BasePlayer.GameTextForAll(message, 5000, 4);
        }

        [Command("port", Shortcut = "port")]
        public static void OnPortCommand(BasePlayer sender, BasePlayer playerId)
        {
            sender.Position = playerId.Position;

            sender.SendClientMessage(Color.GreenYellow, $"You teleported to {playerId.Name}.");
            playerId.SendClientMessage(Color.GreenYellow, $"Admin {sender.Name} teleported to you.");
        }

        [Command("get", Shortcut = "get")]
        public static void OnGetCommand(BasePlayer sender, BasePlayer playerId)
        {
            playerId.Position = sender.Position;

            playerId.SendClientMessage(Color.GreenYellow, $"You have been teleported to admin {sender.Name}.");
            sender.SendClientMessage(Color.GreenYellow, $"You teleported {playerId.Name} to you.");
        }

        [Command("jetpack", Shortcut = "jetpack")]
        public static void OnJetpackCommand(BasePlayer sender)
        {
            sender.SpecialAction = SpecialAction.Usejetpack;
        }

        [Command("nos", Shortcut = "nos")]
        public static void OnNosCommand(BasePlayer sender)
        {
            if (sender.Vehicle?.Driver != sender || sender.Vehicle == null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are not driving a car.");
                return;
            }

            sender.Vehicle?.AddComponent(1010);
            sender.SendClientMessage(Color.GreenYellow, "You added NOS to your car.");
        }

        [Command("tele", Shortcut = "tele")]
        public static void OnTeleCommand(BasePlayer sender, BasePlayer playerId1, BasePlayer playerId2)
        {
            playerId1.Position = playerId2.Position;

            playerId1.SendClientMessage(Color.GreenYellow, $"You have been teleported to {playerId2.Name}.");
            playerId2.SendClientMessage(Color.GreenYellow, $"{playerId1.Name} have been teleported to you.");
        }

        [Command("portloc", Shortcut = "portloc")]
        public static async void OnPortLocCommand(BasePlayer sender, float x, float y, float z, int interior = 0)
        {
            sender.Position = new Vector3(x, y, z);
            sender.Interior = interior;
            await Task.Delay(Config.KickDelay);

            sender.SendClientMessage(Color.GreenYellow, $"You teleported to {sender.Position.ToString()}");
        }

        [Command("networkstats", Shortcut = "networkstats")]
        public static void OnNetworkStatsCommand(BasePlayer sender)
        {
            sender.SendClientMessage(Color.White, $"{Server.NetworkStats}");
        }

        [Command("freeze", Shortcut = "freeze")]
        public static void OnFreezeCommand(BasePlayer sender, Player playerId)
        {
            if (playerId.GetPlayerDataById().AdminLevel > 0)
            {
                sender.SendClientMessage(Color.IndianRed, "You can't freeze other admin.");
                return;
            }

            playerId.ToggleControllable(false);
            playerId.SendClientMessage(Color.IndianRed, $"You have been freeze by admin {sender.Name}.");

            sender.SendClientMessage(Color.GreenYellow, $"You successfully freeze {playerId.Name}.");
        }

        [Command("unfreeze", Shortcut = "unfreeze")]
        public static void OnUnFreezeCommand(BasePlayer sender, BasePlayer playerId)
        {
            playerId.ToggleControllable(true);
            playerId.SendClientMessage(Color.GreenYellow, $"You have been un freeze by admin {sender.Name}.");

            sender.SendClientMessage(Color.GreenYellow, $"You successfully un freeze {playerId.Name}.");
        }

        [Command("nosall", Shortcut = "nosall")]
        public static void OnNosAllCommand(BasePlayer sender)
        {
            foreach (var player in BasePlayer.All)
            {
                if (player.Vehicle is null) continue;

                player.Vehicle.AddComponent(1010);
                player.SendClientMessage(Color.GreenYellow, $"Admin {sender.Name} give NOS to all cars.");
            }

            sender.SendClientMessage(Color.GreenYellow, "You successfully give NOS to all used cars.");
        }

        [Command("warn", Shortcut = "warn")]
        public static void OnWarnCommand(BasePlayer sender, Player playerId, string reason)
        {
            if (playerId.GetPlayerDataById().AdminLevel > 0)
            {
                sender.SendClientMessage(Color.IndianRed, "You can't warn other admin.");
                return;
            }

            if (string.IsNullOrEmpty(reason))
                return;

            playerId.GameText($"Warning: {reason}", 5000, 4);
        }

        [Command("skin", Shortcut = "skin")]
        public static void OnSkinCommand(BasePlayer sender, int skin)
        {
            if (skin < 0 || skin == 74 || skin > 311)
            {
                sender.SendClientMessage(Color.IndianRed, "Invalid skin id.");
                return;
            }

            sender.Skin = skin;
            sender.SendClientMessage(Color.GreenYellow, $"You set yourself skin id: {skin}.");
        }

        [Command("respawnallcars", Shortcut = "respawnallcars")]
        public static void OnResPawnAllCarsCommand(BasePlayer sender)
        {
            foreach (var car in BaseVehicle.All)
                if (car.Model == VehicleModelType.ArticleTrailer || car.Model == VehicleModelType.ArticleTrailer2 ||
                    car.Model == VehicleModelType.ArticleTrailer3 || car.Model == VehicleModelType.PetrolTrailer)
                {
                    if (BaseVehicle.All.All(vehicle => vehicle.Trailer != car)) car.Respawn();
                }
                else if (BasePlayer.All.All(player => player.Vehicle != car))
                {
                    car.Respawn();
                }

            BasePlayer.SendClientMessageToAll(Color.GreenYellow, $"Admin {sender.Name} respawned all unused vehicles.");
        }

        [Command("ban", Shortcut = "ban")]
        public static async void OnBanCommand(BasePlayer sender, Player playerId, string reason, int days = 0)
        {
            if (playerId.GetPlayerDataById().AdminLevel > 0)
            {
                sender.SendClientMessage(Color.IndianRed, "You can't ban other admin.");
                return;
            }

            string message;
            if (days == 0)
            {
                using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                {
                    const string insertQuery =
                        @"INSERT INTO bans (Name, AdminName, Reason, BanTime) VALUES (@BanName, @AdminName, @Reason, @BanTime)";
                    await db.ExecuteAsync(insertQuery, new
                    {
                        BanName = playerId.Name,
                        AdminName = sender.Name,
                        Reason = reason,
                        BanTime = DateTime.MaxValue
                    });
                }

                message = $"Admin {sender.Name} banned you permanently from this server. Reason: {reason}.";
            }
            else
            {
                using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                {
                    const string insertQuery =
                        @"INSERT INTO bans (Name, AdminName, Reason, BanTime) VALUES (@BanName, @AdminName, @Reason, @BanTime)";
                    await db.ExecuteAsync(insertQuery, new
                    {
                        BanName = playerId.Name,
                        AdminName = sender.Name,
                        Reason = reason,
                        BanTime = DateTime.MaxValue
                    });
                }

                message = $"Admin {sender.Name} banned you for {days} days from this server. Reason: {reason}.";
            }

            playerId.SendClientMessage(Color.IndianRed, message);

            sender.SendClientMessage(Color.GreenYellow, $"You successfully banned {playerId.Name} from this server.");

            await Task.Delay(Config.KickDelay);
            playerId.Kick();
        }

        [Command("mute", Shortcut = "mute")]
        public static void OnMuteCommand(BasePlayer sender, Player playerId, string reason, int minutes = 10)
        {
            if (playerId.GetPlayerDataById().AdminLevel > 0)
            {
                sender.SendClientMessage(Color.IndianRed, "You can't mute other admin.");
                return;
            }

            if (minutes < 0 || minutes > 60)
            {
                sender.SendClientMessage(Color.IndianRed, "Minutes can't be less than 0 or more than 60.");
                return;
            }

            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                db.Execute(@"UPDATE players SET MuteTime = @MuteTime WHERE Id = @Id", new
                {
                    MuteTime = DateTime.Now.AddMinutes(minutes),
                    Id = playerId.DbId
                });
            }

            playerId.SendClientMessage(Color.IndianRed,
                $"You got muted for {minutes} minutes by admin {sender.Name} Reason: {reason}.");
            sender.SendClientMessage(Color.GreenYellow,
                $"You successfully muted {playerId.Name} for {minutes} minutes.");
        }

        [Command("unmute", Shortcut = "unmute")]
        public static void OnUnMuteCommand(BasePlayer sender, Player playerId)
        {
            if (playerId.GetPlayerDataById().MuteTime < DateTime.Now)
            {
                sender.SendClientMessage(Color.IndianRed, $"{playerId.Name} is not currently muted.");
                return;
            }

            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                const string updateQuery = @"UPDATE players SET MuteTime = @MuteTime WHERE Id = @Id";
                db.Execute(updateQuery, new
                {
                    MuteTime = DateTime.Now,
                    Id = playerId.DbId
                });
            }

            sender.SendClientMessage(Color.GreenYellow, $"You successfully unmuted {playerId.Name}.");
            playerId.SendClientMessage(Color.GreenYellow, $"You have been unmuted by admin {sender.Name}.");
        }

        [Command("setweather", Shortcut = "setweather")]
        public static void OnSetWeatherCommand(BasePlayer sender, int weatherId)
        {
            if (weatherId < 0 || weatherId > 50)
            {
                sender.SendClientMessage(Color.IndianRed, "Wrong weather id. Must be between 0 and 50 .");
                return;
            }

            Server.SetWeather(weatherId);
            sender.SendClientMessage(Color.GreenYellow,
                $"You successfully changed the weather to weather id: {weatherId}.");
        }

        [Command("reports", Shortcut = "reports")]
        public static void OnReportsCommand(BasePlayer sender)
        {
            var dialog = new TablistDialog("Reports list", 3, "Select", "Close") {Style = DialogStyle.TablistHeaders};
            var rowId = 0;
            dialog.Add("ID", "Issuer", "Reported");
            foreach (var report in Report.Reports)
            {
                dialog.Add(rowId.ToString(), report.IssuerName, report.ReportedName);
                rowId++;
            }

            dialog.Show(sender);
            dialog.Response += (objSender, ev) =>
            {
                if (ev.DialogButton == DialogButton.Right)
                    return;

                var report = Report.Reports[ev.ListItem];
                var messageDialog = new MessageDialog($"Report of: {report.ReportedName}",
                    $"Reason of reporting:\n{report.Message}", "Delete", "Close");
                messageDialog.Show(sender);

                foreach (var player in BasePlayer.All)
                    if (player.Name == report.IssuerName)
                        player.SendClientMessage(Color.GreenYellow, $"Your report has been view by {sender.Name}");

                messageDialog.Response += (sender1, ev1) =>
                {
                    if (ev1.DialogButton == DialogButton.Right)
                        return;

                    Report.Reports.Remove(report);
                    Report.Reports.TrimExcess();

                    foreach (var player in BasePlayer.All)
                        if (player.Name == report.IssuerName)
                            player.SendClientMessage(Color.IndianRed, $"Your report was closed by: {sender.Name}");
                };
            };
        }

        [Command("vehiclespawn", "vehicle", Shortcut = "v")]
        public static void OnCarCommand(BasePlayer sender, int model)
        {
            if (sender.Vehicle != null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are already in a car.");
                return;
            }

            if (model < 400 || model > 611)
            {
                sender.SendClientMessage(Color.IndianRed, "Invalid vehicle model.");
                return;
            }

            var car = Vehicle.Create((VehicleModelType) model, sender.Position, 90.0f, 2, 2);
            car.Engine = true;
            car.AdminSpawned = true;
            sender.PutInVehicle(car, 0);
        }

        [Command("disposecar", Shortcut = "disposecar")]
        public static void OnDisposeCarCommand(BasePlayer sender, Vehicle carId)
        {
            if (carId is null)
            {
                sender.SendClientMessage(Color.IndianRed, "The car doesn't exist.");
                return;
            }

            if (!carId.AdminSpawned)
            {
                sender.SendClientMessage(Color.IndianRed, "This car is not spawned by an admin.");
                return;
            }

            carId.Dispose();
            sender.SendClientMessage(Color.GreenYellow, "Car have been disposed successfully.");
        }

        [Command("disposeallcars", Shortcut = "disposeallcars")]
        public static void OnDisposeAllCarsCommand(BasePlayer sender)
        {
            foreach (var car in BaseVehicle.All)
                if (car is Vehicle customCar && customCar.AdminSpawned)
                    customCar.Dispose();
            sender.SendClientMessage(Color.GreenYellow, "All admin spawned cars have been disposed.");
        }

        [Command("repairallcars", Shortcut = "repairallcars")]
        public static void OnRepairAllCommand(BasePlayer sender)
        {
            foreach (var car in BaseVehicle.All)
                car.Repair();

            BasePlayer.SendClientMessageToAll(Color.GreenYellow, $"Admin {sender.Name} repaired all cars.");
        }

        [Command("healall", Shortcut = "healall")]
        public static void OnHealAllCommand(BasePlayer sender)
        {
            foreach (var player in BasePlayer.All) player.Health = 100.0f;

            BasePlayer.SendClientMessageToAll(Color.GreenYellow, $"Admin {sender.Name} healed all players.");
        }

        [Command("portvehicle", Shortcut = "portvehicle")]
        public static void OnPortVehicleCommand(BasePlayer sender, BaseVehicle vehicleId)
        {
            sender.Position = vehicleId.Position + new Vector3(0, 0, 3);
            sender.SendClientMessage(Color.GreenYellow, $"You got teleported to vehicle {vehicleId.Id}.");
        }
    }
}