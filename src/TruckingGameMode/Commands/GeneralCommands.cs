using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;

namespace TruckingGameMode.Commands
{
    public class GeneralCommands
    {
        [Command("pm")]
        public static void OnPmCommand(BasePlayer sender, BasePlayer receiver, string message)
        {
            if (sender.Id == receiver.Id)
            {
                sender.SendClientMessage(Color.IndianRed, "You can't pm yourself!");
                return;
            }

            receiver.SendClientMessage(Color.White, $"{Color.Gray}PM from {sender.Name}({sender.Id}): {Color.White}{message}");
        }
    }
}