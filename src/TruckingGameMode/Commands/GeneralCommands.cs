using SampSharp.GameMode.Definitions;
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

        [Command("flip")]
        public static void OnFlipCommand(BasePlayer sender)
        {
            if (sender.State != PlayerState.Driving)
            {
                sender.SendClientMessage(Color.IndianRed, "You need to drive a car!");
                return;
            }

            if (sender.InAnyVehicle)
                sender.PutCameraBehindPlayer();

            sender.Vehicle.Position = sender.Position;
            sender.Vehicle.Angle = 0.0f;
        }

        [Command("engine")]
        public static void OnEngineCommand(BasePlayer sender)
        {
            if(sender.State != PlayerState.Driving)
            {
                sender.SendClientMessage(Color.IndianRed, "You need to drive a car!");
                return;
            }

            sender.Vehicle.Engine = sender.Vehicle.Engine != true;
        }
    }
}