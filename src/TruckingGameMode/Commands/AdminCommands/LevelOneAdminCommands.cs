using System.Threading.Tasks;
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
    }
}