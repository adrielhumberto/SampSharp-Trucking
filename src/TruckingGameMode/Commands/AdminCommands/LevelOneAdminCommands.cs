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
        public static async void OnKickCommand(BasePlayer sender, BasePlayer target, string reason)
        {
            sender.SendClientMessage(Color.GreenYellow, $"You kicked {target.Name}");
            target.SendClientMessage(Color.IndianRed,
                $"You have been kicked from the server by {sender.Name}. Reason: {reason}");

            await Task.Delay(100);
            target.Kick();

            BasePlayer.SendClientMessageToAll(Color.IndianRed, $"{target.Name} has been kicked from the server.");
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
        public static void OnPortCommand(BasePlayer sender, BasePlayer target)
        {
            sender.Position = target.Position;

            sender.SendClientMessage(Color.GreenYellow, $"You teleported to {target.Name}.");
            target.SendClientMessage(Color.GreenYellow, $"Admin {sender.Name} teleported to you.");
        }

        [Command("get", Shortcut = "get")]
        public static void OnGetCommand(BasePlayer sender, BasePlayer target)
        {
            target.Position = sender.Position;

            target.SendClientMessage(Color.GreenYellow, $"You have been teleported to admin {sender.Name}.");
            sender.SendClientMessage(Color.GreenYellow, $"You teleported {target.Name} to you.");
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
        public static void OnTeleCommand(BasePlayer sender, BasePlayer target1, BasePlayer target2)
        {
            target1.Position = target2.Position;

            target1.SendClientMessage(Color.GreenYellow, $"You have been teleported to {target2.Name}.");
            target2.SendClientMessage(Color.GreenYellow, $"{target1.Name} have been teleported to you.");
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
        public static void OnFreezeCommand(BasePlayer sender, BasePlayer target)
        {
            target.ToggleControllable(false);
            target.SendClientMessage(Color.IndianRed, $"You have been freeze by admin {sender.Name}.");

            sender.SendClientMessage(Color.GreenYellow, $"You successfully freeze {target.Name}.");
        }

        [Command("unfreeze", Shortcut = "unfreeze")]
        public static void OnUnFreezeCommand(BasePlayer sender, BasePlayer target)
        {
            target.ToggleControllable(true);
            target.SendClientMessage(Color.GreenYellow, $"You have been un freeze by admin {sender.Name}.");

            sender.SendClientMessage(Color.GreenYellow, $"You successfully un freeze {target.Name}.");
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
        public static void OnWarnCommand(BasePlayer sender, BasePlayer target, string reason)
        {
            if (string.IsNullOrEmpty(reason))
                return;

            target.GameText($"Warning: {reason}", 5000, 4);
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
        public static async void OnBanCommand(BasePlayer sender, BasePlayer target, string reason, int days = 0)
        {
            string message;
            if (days == 0)
            {
                using (var db = new GamemodeContext())
                {
                    var ban = new PlayerBanModel
                    {
                        Name = target.Name,
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
                        Name = target.Name,
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

            target.SendClientMessage(Color.IndianRed, message);

            BasePlayer.SendClientMessageToAll(Color.Blue, $"{target.Name} has been banned from the server.");
            sender.SendClientMessage(Color.GreenYellow, $"You successfully banned {target.Name} from this server.");

            await Task.Delay(100);
            target.Kick();
        }

        [Command("mute", Shortcut = "mute")]
        public static void OnMuteCommand(BasePlayer sender, Player target, string reason, int minutes = 10)
        {
            if (minutes < 0 || minutes > 60)
            {
                sender.SendClientMessage(Color.IndianRed, "Minutes can't be less than 0 or more than 60.");
                return;
            }

            using (var db = new GamemodeContext())
            {
                target.FetchPlayerAccountData(db).MuteTime = DateTime.Now.AddMinutes(minutes);
                db.SaveChanges();
            }

            target.SendClientMessage(Color.IndianRed, $"You got muted for {minutes} minutes by admin {sender.Name} Reason: {reason}.");
            sender.SendClientMessage(Color.GreenYellow, $"You successfully muted {target.Name} for {minutes} minutes.");
        }
    }
}