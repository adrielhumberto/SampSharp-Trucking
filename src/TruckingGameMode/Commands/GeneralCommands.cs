using System.Linq;
using BCrypt;
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
using TruckingGameMode.World;

namespace TruckingGameMode.Commands
{
    public class GeneralCommands
    {
        [Command("changepass")]
        public static async void OnChangePassCommand(Player sender)
        {
            var newPasswordDialog =
                new InputDialog("Change password", "Input the new password", true, "Accept", "close");
            await newPasswordDialog.ShowAsync(sender);

            newPasswordDialog.Response += async (obj, ev) =>
            {
                if (ev.DialogButton == DialogButton.Right)
                    return;

                var salt = BCryptHelper.GenerateSalt(12);
                var hash = BCryptHelper.HashPassword(ev.InputText, salt);

                using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                {
                    const string updatequery = @"UPDATE players SET Password = @Password WHERE Name = @PName";
                    await db.ExecuteAsync(updatequery, new
                    {
                        Password = hash,
                        PName = sender.Name
                    });
                }
            };
        }

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
            var radioDialog = new TablistDialog("Chose a radio station", 2, "Listen", "Cancel")
            {
                Style = DialogStyle.TablistHeaders
            };

            radioDialog.Add("#", "Name");

            foreach (var radio in RadioModel.GetRadioStations()) radioDialog.Add($"{radio.Id}", $"{radio.Name}");
            radioDialog.Show(sender);

            radioDialog.Response += (obj, e) =>
            {
                if (e.DialogButton == DialogButton.Right)
                    return;

                switch (e.ListItem)
                {
                    default:
                        sender.PlayAudioStream(RadioModel.GetRadioStations()[e.ListItem].Url);
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
        public static void OnAssistCommand(Player sender)
        {
            if (sender.Vehicle == null)
            {
                sender.SendClientMessage(Color.IndianRed, "You are not driving any vehicle!");
                return;
            }

            if (sender.Vehicle.Health >= 999)
            {
                sender.SendClientMessage(Color.IndianRed, "Your vehicle is not damaged.");
                return;
            }

            var repairPayment = (int) (1000.0f - sender.Vehicle.Health) * 5;

            if (sender.Money <= repairPayment)
            {
                sender.SendClientMessage(Color.IndianRed,
                    $"You don't have enough money to repair the car. You need ${repairPayment - sender.Money} more.");
                return;
            }

            sender.Money -= repairPayment;
            sender.SendClientMessage(Color.GreenYellow, $"You payed ${repairPayment} to repair your car.");

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

        [Command("report")]
        public static void OnReportCommand(BasePlayer sender, BasePlayer playerId, string reason)
        {
            Report.Reports.Add(new Report
            {
                IssuerName = sender.Name,
                ReportedName = playerId.Name,
                Message = reason
            });

            sender.SendClientMessage(Color.GreenYellow, "You report has been submitted to our admins!");

            foreach (var admin in BasePlayer.All)
                if (admin is Player adminData && adminData.PlayerData().AdminLevel > 0)
                    adminData.SendClientMessage(Color.Red, $"New report from {sender.Name}. Type /reports to view it!");
        }
    }
}