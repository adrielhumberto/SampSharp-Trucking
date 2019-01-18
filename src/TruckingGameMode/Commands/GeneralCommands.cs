using System.Linq;
using GamemodeDatabase.Models;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using TruckingGameMode.World;

namespace TruckingGameMode.Commands
{
    public class GeneralCommands
    {
        [Command("pm")]
        public static void OnPmCommand(BasePlayer sender, BasePlayer playerId, string message)
        {
            if (sender.Id == playerId.Id)
            {
                sender.SendClientMessage(Color.IndianRed, "You can't pm yourself!");
                return;
            }

            playerId.SendClientMessage(Color.White,
                $"{Color.Gray}PM from {sender.Name}({sender.Id}): {Color.White}{message}");
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
            if (sender.State != PlayerState.Driving)
            {
                sender.SendClientMessage(Color.IndianRed, "You need to drive a car!");
                return;
            }

            sender.Vehicle.Engine = sender.Vehicle.Engine != true;
        }

        [Command("radio")]
        public static void OnListenRadioCommand(BasePlayer sender)
        {
            var dialog = new ListDialog("Chose a radio station", "Chose", "Close");
            dialog.AddItem("Pro FM");
            dialog.AddItem("Radio Bandit");
            dialog.AddItem("TruckersFM");
            dialog.AddItem("SimulatorRadio");
            dialog.AddItem("TruckSim FM");
            dialog.AddItem("Stop radio");
            dialog.Show(sender);

            dialog.Response += (obj, e) =>
            {
                if (e.DialogButton == DialogButton.Right)
                    return;

                switch (e.ListItem)
                {
                    case 0:
                        sender.PlayAudioStream("http://edge126.rdsnet.ro:84/profm/profm.mp3");
                        break;
                    case 1:
                        sender.PlayAudioStream("http://live.radiobandit.ro:8000/bandit.mp3");
                        break;
                    case 2:
                        sender.PlayAudioStream("https://radio.truckers.fm");
                        break;
                    case 3:
                        sender.PlayAudioStream("https://simulatorradio.stream/stream?1547465711053");
                        break;
                    case 4:
                        sender.PlayAudioStream("https://trucksim.fm/stream/audio.mp3");
                        break;
                    default:
                        sender.StopAudioStream();
                        break;
                }
            };
        }

        [Command("reclass")]
        public static void OnReClassCommand(BasePlayer sender)
        {
            sender.ForceClassSelection();
            sender.Health = 0.0f;
        }

        [Command("rescue")]
        public static void OnRescueCommand(BasePlayer sender)
        {
            var dialogSpawnsList = new TablistDialog("Select spawn", 1, "Select", "Cancel");
            var spawnsList = TruckerSpawnModel.GetTruckerSpawnListNoTracking;
            foreach (var spawn in spawnsList)
                dialogSpawnsList.Add(spawn.Name);
            dialogSpawnsList.Show(sender);
            dialogSpawnsList.Response += (obj, eve) =>
            {
                if (eve.DialogButton == DialogButton.Right)
                    return;

                switch (eve.ListItem)
                {
                    default:
                    {
                        sender.Position = new Vector3(spawnsList[eve.ListItem].X,
                            spawnsList[eve.ListItem].Y,
                            spawnsList[eve.ListItem].Z);
                        sender.Angle = spawnsList[eve.ListItem].Angle;
                    }
                        break;
                }
            };
        }

        [Command("assist")]
        public static void OnAssistCommand(BasePlayer sender)
        {
            if (sender.Vehicle == null)
                sender.SendClientMessage(Color.IndianRed, "You are not driving any vehicle!");

            sender.Vehicle?.Repair();
        }

        [Command("admins")]
        public static void OnAdminsCommand(BasePlayer sender)
        {
            var listDialog = new ListDialog("Admin List", "Ok");
            foreach (var admin in BasePlayer.All)
            {
                var adminData = admin as Player;
                if (adminData?.PlayerData().AdminLevel > 0)
                    listDialog.AddItem(adminData.Name);
            }

            listDialog.Show(sender);
        }

        [Command("eject")]
        public static void OnEjectCommand(BasePlayer sender, BasePlayer playerId)
        {
            if (sender.Vehicle != null || sender.Vehicle?.Driver == sender)
            {
                if (sender.Vehicle.Passengers.Any(x => x.Name == playerId.Name))
                {
                    playerId.Position = playerId.Position + Vector3.Up;
                    playerId.SendClientMessage(Color.IndianRed, "You have been eject from the car.");
                }
                else
                {
                    sender.SendClientMessage(Color.IndianRed,
                        $"{playerId.Name} is not in your car or you can't eject yourself.");
                }
            }
            else
            {
                sender.SendClientMessage(Color.IndianRed, "You are not in any car or you are not the driver.");
            }
        }

        [Command("givemoney")]
        public static void OnGiveMoneyCommand(Player sender, Player playerId, int money)
        {
            if (money <= 0)
            {
                sender.SendClientMessage(Color.IndianRed, "Money given can't be 0 or negative number.");
                return;
            }

            if (sender.Money < money)
            {
                sender.SendClientMessage(Color.IndianRed, "You don't have enough money.");
                return;
            }

            playerId.Money += money;
            sender.Money -= money;

            playerId.SendClientMessage(Color.GreenYellow, $"You received ${money} from {sender.Name}.");
            sender.SendClientMessage(Color.GreenYellow, $"You successfully given ${money} to {playerId.Name}.");
        }

        [Command("detach")]
        public static void OnDetachCommand(BasePlayer sender)
        {
            if (sender.Vehicle == null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are not driving any vehicle.");
                return;
            }

            if (sender.Vehicle.HasTrailer)
            {
                sender.Vehicle.Trailer = null;
                sender.SendClientMessage(Color.GreenYellow, "Trailer detached successfully.");
            }
            else
            {
                sender.SendClientMessage(Color.IndianRed, "Your vehicle doesn't have any trailer.");
            }
        }
    }
}