using System;
using System.Threading.Tasks;
using BCrypt;
using Dapper;
using GamemodeDatabase;
using GamemodeDatabase.Data;
using MySql.Data.MySqlClient;
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
using TruckingGameMode.Houses;

namespace TruckingGameMode.World
{
    [PooledType]
    public class Player : BasePlayer
    {
        private Timer _updateMoneyTimer;
        public bool IsLogged;
        public PlayerClasses PlayerClass { get; set; }
        private int LoginTries { get; set; }
        public TruckerJobDetails CurrentJob { get; set; }
        public House CurrentHouse { get; set; }


        public override int Money
        {
            get => PlayerData().Money;
            set
            {
                using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                {
                    db.Execute(@"UPDATE players SET Money = @Money WHERE Name = @PName", new
                    {
                        Money = value,
                        PNAme = Name
                    });
                }

                base.Money = PlayerData().Money;
            }
        }

        public int TruckerJobs
        {
            get => PlayerData().TruckerJobs;
            set
            {
                using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                {
                    const string updateQuery =
                        @"UPDATE players SET TruckerJobs = TruckerJobs + @Value WHERE Name = @PName";
                    db.Execute(updateQuery, new
                    {
                        Value = value,
                        PName = Name
                    });
                }
            }
        }

        public PlayerModel PlayerData()
        {
            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                const string selectQuery = @"SELECT * FROM players WHERE Name = @PName";
                return db.QueryFirstOrDefault<PlayerModel>(selectQuery, new
                {
                    PName = Name
                });
            }
        }

        public PlayerBanModel PlayerBan()
        {
            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                const string selectQuery = @"SELECT * FROM bans WHERE Name = @BanName";
                return db.QueryFirstOrDefault<PlayerBanModel>(selectQuery, new
                {
                    BanName = Name
                });
            }
        }

        private Vector3 GetPlayerPositionVector3FromDatabase()
        {
            return new Vector3(PlayerData().PositionX, PlayerData().PositionY, PlayerData().PositionZ);
        }

        public static void SendMessageToAdmins(Color color, string message)
        {
            foreach (var admin in All)
                if (admin is Player adminData && adminData.PlayerData().AdminLevel > 0)
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
                        await Task.Delay(Config.KickDelay);
                        Kick();
                    }
                    else if (BCryptHelper.CheckPassword(ev.InputText, PlayerData().Password))
                    {
                        ToggleSpectating(false);
                        IsLogged = true;
                        SendClientMessageToAll(Color.DarkGray, $"* Player {Name} connected to server.");

                        _updateMoneyTimer = new Timer(500, true);
                        _updateMoneyTimer.Tick += (obj, evv) => { base.Money = Money; };

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
                    using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                    {
                        var salt = BCryptHelper.GenerateSalt(12);
                        var hash = BCryptHelper.HashPassword(ev.InputText, salt);
                        const string insertQuery = @"INSERT INTO players(Name, Password) VALUES (@PName, @Password)";
                        db.Execute(insertQuery, new
                        {
                            PName = Name,
                            Password = hash
                        });

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

            if (PlayerBan() != null)
                if (PlayerBan().BanTime <= DateTime.Now)
                    using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                    {
                        const string deleteQuery = @"DELETE FROM bans WHERE Name = @BanName";
                        db.Execute(deleteQuery, new
                        {
                            BanName = Name
                        });
                    }

            if (PlayerBan() is null)
            {
                if (PlayerData() is null)
                    RegisterPlayer();
                else
                    LoginPlayer();
            }
            else
            {
                var message = $"Name: {PlayerBan().Name}\n" +
                              $"Admin Name: {PlayerBan().AdminName}\n" +
                              $"Reason: {PlayerBan().Reason}\n" +
                              $"Issued at: {PlayerBan().IssuedTime:dd/MM/yyyy HH:mm}\n" +
                              $"Banned until: {PlayerBan().BanTime:dd/MM/yyyy HH:mm}";

                var infoDialog = new MessageDialog("Ban details", message, "OK");
                infoDialog.Show(this);
                await Task.Delay(Config.KickDelay);
                Kick();
            }

            base.OnConnected(e);
        }

        public override void OnKeyStateChanged(KeyStateChangedEventArgs e)
        {
            if (KeyUtils.HasPressed(e.NewKeys, e.OldKeys, Keys.LookBehind))
                GeneralCommands.OnEngineCommand(this);

            base.OnKeyStateChanged(e);
        }

        public override async void OnRequestClass(RequestClassEventArgs e)
        {
            if (!IsLogged)
            {
                SendClientMessage(Color.IndianRed, "You did not log in successfully. You have been kicked.");
                await Task.Delay(Config.KickDelay);
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
            dialogList.AddItem("Your house");
            dialogList.Show(this);

            if (PlayerClass == PlayerClasses.TruckDriver)
                dialogList.Response += (sender, ev) =>
                {
                    var spawnsList = TruckerSpawnModel.GetTruckerSpawnListNoTracking;
                    var randomIndex = new Random().Next(spawnsList.Count);
                    if (ev.DialogButton != DialogButton.Right)
                    {
                        switch (ev.ListItem)
                        {
                            case 0:
                            {
                                

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
                                SetSpawnInfo(0, Skin,
                                    new Vector3(spawnsList[randomIndex].X,
                                        spawnsList[randomIndex].Y,
                                        spawnsList[randomIndex].Z),
                                    spawnsList[randomIndex].Angle);
                                    Spawn();
                                    Position = GetPlayerPositionVector3FromDatabase();
                                    Angle = PlayerData().FacingAngle;
                            }
                                break;
                            case 3:
                            {
                                var ownedHouse = House.Houses.Find(x => x.HouseData().Owner == Name);
                                if (ownedHouse is null)
                                {
                                    SendClientMessage(Color.IndianRed, "You are not owning any house.");
                                    dialogList.Show(this);
                                }
                                else
                                {
                                    SetSpawnInfo(0, Skin, ownedHouse.Position, PlayerData().FacingAngle);
                                    Spawn();
                                }
                            }
                                break;
                        }
                    }
                    else
                    {
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

            if (PlayerClass == PlayerClasses.TruckDriver)
            {
                Color = PlayerClassesColors.TruckerColor;
            }

            base.OnSpawned(e);
        }

        public override void OnText(TextEventArgs e)
        {
            e.SendToPlayers = false;

            if (PlayerData().MuteTime > DateTime.Now)
            {
                SendClientMessage(Color.IndianRed,
                    DateTime.Now.Subtract(PlayerData().MuteTime).Minutes != 0
                        ? $"You are currently muted for {PlayerData().MuteTime.Subtract(DateTime.Now).Minutes} more minutes."
                        : $"You are currently muted for {PlayerData().MuteTime.Subtract(DateTime.Now).Seconds} more seconds.");
                return;
            }

            if (PlayerData().AdminLevel > 0)
                if (e.Text.StartsWith('@'))
                {
                    SendMessageToAdmins(Color.Gold, $"[AC] {Name}: {e.Text.Remove(0, 1)}");
                    return;
                }


            SendClientMessageToAll(Color.White, $"{Color}{Name}[ID:{Id}]: {Color.White}{e.Text}");

            base.OnText(e);
        }

        private void SavePlayerLastPosition()
        {
            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                const string updateQuery =
                    @"UPDATE players SET PositionX = @PositionX, PositionY = @PositionY, PositionZ = @PositionZ WHERE Name = @PName";
                db.Execute(updateQuery, new
                {
                    PositionX = Position.X,
                    PositionY = Position.Y,
                    PositionZ = Position.Z,
                    PName = Name
                });
            }
        }

        public override void OnDisconnected(DisconnectEventArgs e)
        {
            SavePlayerLastPosition();

            if (CurrentJob != null)
            {
                CurrentJob.Truck.Dispose();
                CurrentJob.Trailer.Dispose();
            }

            SendClientMessageToAll(Color.DarkGray, $"* Player {Name} left the server({e.Reason.ToString()}).");

            base.OnDisconnected(e);
        }

        public override void OnEnterVehicle(EnterVehicleEventArgs e)
        {
            if (e.Vehicle.Engine == false)
                SendClientMessage(Color.CadetBlue, "Press key 2 to start your vehicle engine.");

            base.OnEnterVehicle(e);
        }

        protected override void Dispose(bool disposing)
        {
            _updateMoneyTimer.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}