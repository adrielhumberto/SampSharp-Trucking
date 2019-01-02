using System;
using System.Collections.Generic;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using TruckingGameMode.Classes.Spawns;

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

        [Command("listenradio")]
        public static void OnListenRadioCommand(BasePlayer sender)
        {
            var dialog = new ListDialog("Chose a radio station", "Chose", "Close");
            dialog.AddItem("Pro FM");
            dialog.AddItem("Radio Bandit");
            dialog.AddItem("Stop radio");
            dialog.Show(sender);

            dialog.Response += (obj, e) =>
            {
                if(e.DialogButton == DialogButton.Right)
                    return;

                switch (e.ListItem)
                {
                    case 0:
                        sender.PlayAudioStream("http://edge126.rdsnet.ro:84/profm/profm.mp3");
                        break;
                    case 1:
                        sender.PlayAudioStream("http://live.radiobandit.ro:8000/bandit.mp3");
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
            foreach (var spawn in TruckerSpawn.TruckerSpawns) dialogSpawnsList.Add(spawn.Name);
            dialogSpawnsList.Show(sender);
            dialogSpawnsList.Response += (obj, eve) =>
            {
                switch (eve.ListItem)
                {
                    default:
                    {
                        sender.Position = TruckerSpawn.TruckerSpawns[eve.ListItem].Position;
                        sender.Angle = TruckerSpawn.TruckerSpawns[eve.ListItem].Angle;
                    }
                        break;
                }
            };
        }
    }
}