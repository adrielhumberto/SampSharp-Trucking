using System;
using System.Linq;
using System.Threading.Tasks;
using GamemodeDatabase;
using GamemodeDatabase.Models;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
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
        public static async void OnKickCommand(BasePlayer sender, BasePlayer playerId, string reason)
        {
            sender.SendClientMessage(Color.GreenYellow, $"You kicked {playerId.Name}");
            playerId.SendClientMessage(Color.IndianRed,
                $"You have been kicked from the server by {sender.Name}. Reason: {reason}");

            await Task.Delay(100);
            playerId.Kick();

            BasePlayer.SendClientMessageToAll(Color.IndianRed, $"{playerId.Name} has been kicked from the server.");
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
        public static async void OnPortLocCommand(BasePlayer sender, float x, float y, float z)
        {
            sender.Position = new Vector3(x, y, z);
            await Task.Delay(100);

            sender.SendClientMessage(Color.GreenYellow, $"You teleported to {sender.Position.ToString()}");
        }

        [Command("networkstats", Shortcut = "networkstats")]
        public static void OnNetworkStatsCommand(BasePlayer sender)
        {
            sender.SendClientMessage(Color.White, $"{Server.NetworkStats}");
        }

        [Command("freeze", Shortcut = "freeze")]
        public static void OnFreezeCommand(BasePlayer sender, BasePlayer playerId)
        {
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
        public static void OnWarnCommand(BasePlayer sender, BasePlayer playerId, string reason)
        {
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
            {
                if (car.Model == VehicleModelType.ArticleTrailer || car.Model == VehicleModelType.ArticleTrailer2 ||
                    car.Model == VehicleModelType.ArticleTrailer3 || car.Model == VehicleModelType.PetrolTrailer)
                {
                    if (BaseVehicle.All.All(vehicle => vehicle.Trailer != car)) car.Respawn();
                }
                else if (BasePlayer.All.All(player => player.Vehicle != car))
                {
                    car.Respawn();
                }
            }

            BasePlayer.SendClientMessageToAll(Color.GreenYellow, $"Admin {sender.Name} respawned all unused vehicles.");
        }

        [Command("ban", Shortcut = "ban")]
        public static async void OnBanCommand(BasePlayer sender, BasePlayer playerId, string reason, int days = 0)
        {
            string message;
            if (days == 0)
            {
                using (var db = new GamemodeContext())
                {
                    var ban = new PlayerBanModel
                    {
                        Name = playerId.Name,
                        AdminName = sender.Name,
                        Reason = reason,
                        BanTime = DateTime.MaxValue,
                        IssuedTime = DateTime.Now
                    };
                    await db.Bans.AddAsync(ban);
                    await db.SaveChangesAsync();
                }

                message = $"Admin {sender.Name} banned you permanently from this server. Reason: {reason}.";
            }
            else
            {
                using (var db = new GamemodeContext())
                {
                    var ban = new PlayerBanModel
                    {
                        Name = playerId.Name,
                        AdminName = sender.Name,
                        Reason = reason,
                        BanTime = DateTime.Now.AddDays(days),
                        IssuedTime = DateTime.Now
                    };
                    await db.Bans.AddAsync(ban);
                    await db.SaveChangesAsync();
                }

                message = $"Admin {sender.Name} banned you for {days} days from this server. Reason: {reason}.";
            }

            playerId.SendClientMessage(Color.IndianRed, message);

            BasePlayer.SendClientMessageToAll(Color.Blue, $"{playerId.Name} has been banned from the server.");
            sender.SendClientMessage(Color.GreenYellow, $"You successfully banned {playerId.Name} from this server.");

            await Task.Delay(100);
            playerId.Kick();
        }

        [Command("mute", Shortcut = "mute")]
        public static void OnMuteCommand(BasePlayer sender, Player playerId, string reason, int minutes = 10)
        {
            if (minutes < 0 || minutes > 60)
            {
                sender.SendClientMessage(Color.IndianRed, "Minutes can't be less than 0 or more than 60.");
                return;
            }

            using (var db = new GamemodeContext())
            {
                playerId.FetchPlayerAccountData(db).MuteTime = DateTime.Now.AddMinutes(minutes);
                db.SaveChanges();
            }

            playerId.SendClientMessage(Color.IndianRed, $"You got muted for {minutes} minutes by admin {sender.Name} Reason: {reason}.");
            sender.SendClientMessage(Color.GreenYellow, $"You successfully muted {playerId.Name} for {minutes} minutes.");
        }

        [Command("unmute", Shortcut = "unmute")]
        public static void OnUnMuteCommand(BasePlayer sender, Player playerId)
        {
            if (playerId.FetchPlayerAccountData().MuteTime < DateTime.Now)
            {
                sender.SendClientMessage(Color.IndianRed, $"{playerId.Name} is not currently muted.");
                return;
            }

            using (var db = new GamemodeContext())
            {
                playerId.FetchPlayerAccountData(db).MuteTime = DateTime.Now;
                db.SaveChanges();
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
            sender.SendClientMessage(Color.GreenYellow, $"You successfully changed the weather to weather id: {weatherId}.");
        }
    }
}