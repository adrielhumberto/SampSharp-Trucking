using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt;
using GamemodeDatabase;
using GamemodeDatabase.Models;
using Microsoft.EntityFrameworkCore;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.Tools;
using SampSharp.GameMode.World;
using TruckingGameMode.Classes;
using TruckingGameMode.Classes.Jobs.Trucker;
using TruckingGameMode.Commands;

namespace TruckingGameMode.World
{
    [PooledType]
    public class Player : BasePlayer
    {
        public bool IsLogged;
        public PlayerClasses PlayerClass { get; set; }
        private int LoginTries { get; set; }
        public TruckerJobDetails CurrentJob { get; set; }

        public override int Money
        {
            get => FetchPlayerAccountData().Money;
            set
            {
                using (var db = new GamemodeContext())
                {
                    FetchPlayerAccountData(db).Money = value;
                    db.SaveChanges();
                }

                base.Money = FetchPlayerAccountData().Money;
            }
        }

        public int TruckerJobs
        {
            get => FetchPlayerAccountData().TruckerJobs;
            set
            {
                using (var db = new GamemodeContext())
                {
                    FetchPlayerAccountData(db).TruckerJobs += value;
                    db.SaveChanges();
                }
            }
        }

        public PlayerModel FetchPlayerAccountData()
        {
            using (var db = new GamemodeContext())
            {
                return db.Players.AsNoTracking().FirstOrDefault(x => x.Name == Name);
            }
        }

        public PlayerBanModel FetchBanDetails()
        {
            using (var db = new GamemodeContext())
            {
                return db.Bans.FirstOrDefault(x => x.Name == Name);
            }
        }

        public PlayerModel FetchPlayerAccountData(GamemodeContext db)
        {
            return db.Players.FirstOrDefault(x => x.Name == Name);
        }

        private Vector3 GetPlayerPositionVector3FromDatabase()
        {
            return new Vector3(FetchPlayerAccountData().PositionX, FetchPlayerAccountData().PositionY,
                FetchPlayerAccountData().PositionZ);
        }

        public void SavePlayerPosition()
        {
            using (var db = new GamemodeContext())
            {
                FetchPlayerAccountData(db).PositionX = Position.X;
                FetchPlayerAccountData(db).PositionY = Position.Y;
                FetchPlayerAccountData(db).PositionZ = Position.Z;
                FetchPlayerAccountData(db).FacingAngle = Angle;

                db.SaveChanges();
            }
        }

        public static void SendMessageToAdmins(Color color, string message)
        {
            foreach (var admin in All)
                if (admin is Player adminData && adminData.FetchPlayerAccountData().AdminLevel > 0)
                    admin.SendClientMessage(color, message);
        }

        #region Player registration system

        private void LoginPlayer()
        {
            var message = $"Insert your password. Tries left: {LoginTries}/{Config.MaximumLoginTries}";
            var dialog = new InputDialog("Login", message, true, "Login", "Cancel");
            dialog.Show(this);
            dialog.Response += async (sender, ev) =>
            {
                if (ev.DialogButton == DialogButton.Left)
                {
                    if (LoginTries >= Config.MaximumLoginTries)
                    {
                        SendClientMessage(Color.OrangeRed, "You exceed maximum login tries. You have been kicked!");
                        await Task.Delay(300);
                        Kick();
                    }
                    else if (BCryptHelper.CheckPassword(ev.InputText, FetchPlayerAccountData().Password))
                    {
                        ToggleSpectating(false);
                        IsLogged = true;
                        base.Money = Money;
                    }
                    else
                    {
                        LoginTries++;
                        SendClientMessage(Color.Red, "Wrong password");
                        dialog.Message =
                            $"Wrong password! Retype your password! Tries left: {LoginTries}/{Config.MaximumLoginTries}";
                        LoginPlayer();
                    }
                }
                else
                {
                    Kick();
                }
            };
        }

        private void RegisterPlayer()
        {
            var dialog = new InputDialog("Register", "Input your password", true, "Register", "Cancel");
            dialog.Show(this);
            dialog.Response += (sender, ev) =>
            {
                if (ev.DialogButton == DialogButton.Left)
                    using (var db = new GamemodeContext())
                    {
                        var salt = BCryptHelper.GenerateSalt(12);
                        var hash = BCryptHelper.HashPassword(ev.InputText, salt);
                        var player = new PlayerModel
                        {
                            Password = hash,
                            Name = Name,
                            JoinedDate = DateTime.Now
                        };
                        db.Players.Add(player);
                        db.SaveChanges();

                        LoginPlayer();
                    }
                else
                    Kick();
            };
        }

        #endregion

        #region Oveerrides of BasePlayer

        public override async void OnConnected(EventArgs e)
        {
            SetWorldBounds(2500.0f, 1850.0f, 631.2963f, -454.9898f);

            ToggleSpectating(true);

            if (FetchBanDetails() != null)
                if (FetchBanDetails().BanTime <= DateTime.Now)
                    using (var db = new GamemodeContext())
                    {
                        db.Bans.Remove(FetchBanDetails());
                        db.SaveChanges();
                    }


            if (FetchBanDetails() is null)
            {
                if (FetchPlayerAccountData() is null)
                    RegisterPlayer();
                else
                    LoginPlayer();
            }
            else
            {
                var message = $"Name: {FetchBanDetails().Name}\n" +
                              $"Admin Name: {FetchBanDetails().AdminName}\n" +
                              $"Reason: {FetchBanDetails().Reason}\n" +
                              $"Issued at: {FetchBanDetails().IssuedTime:dd/MM/yyyy HH:mm}\n" +
                              $"Banned until: {FetchBanDetails().BanTime:dd/MM/yyyy HH:mm}";

                var infoDialog = new MessageDialog("Ban details", message, "OK");
                infoDialog.Show(this);
                await Task.Delay(100);
                Kick();
            }


            base.OnConnected(e);
        }

        public override void OnKeyStateChanged(KeyStateChangedEventArgs e)
        {
            if(KeyUtils.HasPressed(e.NewKeys, e.OldKeys, Keys.LookBehind))
                GeneralCommands.OnEngineCommand(this);

            base.OnKeyStateChanged(e);
        }

        public override async void OnRequestClass(RequestClassEventArgs e)
        {
            if (!IsLogged)
            {
                SendClientMessage(Color.IndianRed, "You did not log in successfully. You have been kicked.");
                await Task.Delay(100);
                Kick();
            }


            VirtualWorld = 1;

            #region Class setup

            if (e.ClassId == 0 || e.ClassId == 1)
            {
                Position = new Vector3(-2123.3848f, -218.5014f, 35.3203f);
                Angle = 240f;
                CameraPosition = new Vector3(-2115.0784f, -220.5014f, 38.3203f);
                SetCameraLookAt(new Vector3(-2123.3848f, -218.5014f, 35.3203f));

                GameText("Truck driver", 3000, 4);
                PlayerClass = PlayerClasses.TruckDriver;
            }

            #endregion


            base.OnRequestClass(e);
        }

        public override void OnRequestSpawn(RequestSpawnEventArgs e)
        {
            e.PreventSpawning = true;

            var dialogList = new ListDialog("Select spawn location", "Select", "Random");
            dialogList.AddItem("Random spawn");
            dialogList.AddItem("Select Spawn");
            dialogList.AddItem("Last location");
            dialogList.Show(this);

            if (PlayerClass == PlayerClasses.TruckDriver)
                dialogList.Response += (sender, ev) =>
                {
                    var spawnsList = TruckerSpawnModel.GetTruckerSpawnListNoTracking;
                    if (ev.DialogButton != DialogButton.Right)
                    {
                        switch (ev.ListItem)
                        {
                            case 0:
                            {
                                var randomIndex = new Random().Next(spawnsList.Count);

                                SetSpawnInfo(0, Skin,
                                    new Vector3(spawnsList[randomIndex].X,
                                        spawnsList[randomIndex].Y,
                                        spawnsList[randomIndex].Z),
                                    spawnsList[randomIndex].Angle);

                                Spawn();
                                break;
                            }
                            case 1:
                            {
                                var dialogSpawnsList = new TablistDialog("Select spawn", 1, "Select", "Back");
                                foreach (var spawn in spawnsList)
                                    dialogSpawnsList.Add(spawn.Name);
                                dialogSpawnsList.Show(this);
                                dialogSpawnsList.Response += (obj, eve) =>
                                {
                                    if (eve.DialogButton == DialogButton.Right)
                                    {
                                        dialogList.Show(this);
                                        return;
                                    }


                                    switch (eve.ListItem)
                                    {
                                        default:
                                        {
                                            SetSpawnInfo(0, Skin, new Vector3(
                                                    spawnsList[eve.ListItem].X,
                                                    spawnsList[eve.ListItem].Y,
                                                    spawnsList[eve.ListItem].Z),
                                                spawnsList[eve.ListItem].Angle);
                                            Spawn();
                                        }
                                            break;
                                    }
                                };
                            }
                                break;
                            case 2:
                            {
                                SetSpawnInfo(0, Skin, GetPlayerPositionVector3FromDatabase(),
                                    FetchPlayerAccountData().FacingAngle);
                                Spawn();
                            }
                                break;
                        }
                    }
                    else
                    {
                        var randomIndex = new Random().Next(spawnsList.Count);

                        SetSpawnInfo(0, Skin,
                            new Vector3(spawnsList[randomIndex].X,
                                spawnsList[randomIndex].Y,
                                spawnsList[randomIndex].Z),
                            spawnsList[randomIndex].Angle);

                        Spawn();
                    }
                };

            base.OnRequestSpawn(e);
        }

        public override void OnSpawned(SpawnEventArgs e)
        {
            VirtualWorld = 0;
            Interior = 0;

            ToggleClock(false);
            ResetWeapons();

            var message = string.Empty;

            if (PlayerClass == PlayerClasses.TruckDriver)
            {
                Color = PlayerClassesColors.TruckerColor;
                message = $"{Name} joined truck driver class.";
            }

            SendClientMessageToAll(Color, message);

            base.OnSpawned(e);
        }

        public override void OnText(TextEventArgs e)
        {
            e.SendToPlayers = false;

            if (FetchPlayerAccountData().MuteTime > DateTime.Now)
            {
                SendClientMessage(Color.IndianRed,
                    DateTime.Now.Subtract(FetchPlayerAccountData().MuteTime).Minutes != 0
                        ? $"You are currently muted for {FetchPlayerAccountData().MuteTime.Subtract(DateTime.Now).Minutes} more minutes."
                        : $"You are currently muted for {FetchPlayerAccountData().MuteTime.Subtract(DateTime.Now).Seconds} more seconds.");
                return;
            }

            if (FetchPlayerAccountData().AdminLevel > 0)
                if (e.Text.StartsWith('@'))
                {
                    SendMessageToAdmins(Color.Gold, $"[AC] {Name}: {e.Text.Remove(0, 1)}");
                    return;
                }


            SendClientMessageToAll(Color.White, $"{Color}{Name}[ID:{Id}]: {Color.White}{e.Text}");

            base.OnText(e);
        }

        public override void OnDisconnected(DisconnectEventArgs e)
        {
            SavePlayerPosition();

            using (var db = new GamemodeContext())
            {
                FetchPlayerAccountData(db).LastActive = DateTime.Now;
                db.SaveChanges();
            }

            if (CurrentJob != null)
            {
                CurrentJob.Truck.Dispose();
                CurrentJob.Trailer.Dispose();
            }

            base.OnDisconnected(e);
        }

        public override void OnDeath(DeathEventArgs e)
        {
            SavePlayerPosition();

            base.OnDeath(e);
        }

        public override void OnEnterVehicle(EnterVehicleEventArgs e)
        {
            if (e.Vehicle.Engine == false)
                SendClientMessage(Color.CadetBlue, "Press key 2 to start your vehicle engine.");

            base.OnEnterVehicle(e);
        }

        #endregion
    }
}