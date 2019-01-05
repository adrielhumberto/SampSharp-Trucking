using System.Threading.Tasks;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using TruckingGameMode.Commands.AdminCommands.AdminCommandPermissions;

namespace TruckingGameMode.Commands.AdminCommands
{
    [CommandGroup("admin", PermissionChecker = typeof(LevelOneAdminPermission))]
    public class LevelOneAdminCommands
    {
        [Command("kick", Shortcut = "kick")]
        public static async void OnKickCommand(BasePlayer sender, BasePlayer target, string reason)
        {
            sender.SendClientMessage(Color.GreenYellow, $"You kicked {target.Name}");
            target.SendClientMessage(Color.IndianRed, $"You have been kicked from the server by {sender.Name}. Reason: {reason}");

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
            for (var i = 0; i < 100; i++)
            {
                BasePlayer.SendClientMessageToAll(Color.White, "");
            }
        }

        [Command("announce", Shortcut = "ann")]
        public static void OnAnnounceCommand(BasePlayer sender, string message)
        {
            if(string.IsNullOrEmpty(message))
                return;

            BasePlayer.GameTextForAll(message, 5000, 4);
        }

        [Command("port")]
        public static void OnPortCommand(BasePlayer sender, BasePlayer target)
        {
            sender.Position = target.Position;

            sender.SendClientMessage(Color.GreenYellow, $"You teleported to {target.Name}.");
            target.SendClientMessage(Color.GreenYellow, $"Admin {sender.Name} teleported to you.");
        }

        [Command("get")]
        public static void OnGetCommand(BasePlayer sender, BasePlayer target)
        {
            target.Position = sender.Position;

            target.SendClientMessage(Color.GreenYellow, $"You have been teleported to admin {sender.Name}.");
            sender.SendClientMessage(Color.GreenYellow, $"You teleported {target.Name} to you.");
        }

        [Command("jetpack")]
        public static void OnJetpackCommand(BasePlayer sender)
        {
            sender.SpecialAction = SpecialAction.Usejetpack;
        }

        [Command("nos")]
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

        [Command("tele")]
        public static void OnTeleCommand(BasePlayer sender, BasePlayer target1, BasePlayer target2)
        {
            target1.Position = target2.Position;

            target1.SendClientMessage(Color.GreenYellow, $"You have been teleported to {target2.Name}.");
            target2.SendClientMessage(Color.GreenYellow, $"{target1.Name} have been teleported to you.");
        }
    }
}