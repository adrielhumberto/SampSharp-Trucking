using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt;
using GamemodeDatabase;
using GamemodeDatabase.Models;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using TruckingGameMode.Classes;
using TruckingGameMode.Classes.Spawns;
using TruckingGameMode.Commands;

namespace TruckingGameMode.World
{
    [PooledType]
    public class Player : BasePlayer
    {
        public PlayerClasses PlayerClass { get; set; }
        public int LoginTries { get; private set; }

        public PlayerModel FetchPlayerAccountData()
        {
            using (var db = new GamemodeContext())
            {
                return db.Players.FirstOrDefault(x => x.Name == Name);
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
                else if (ev.DialogButton == DialogButton.Right)
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
                switch (ev.DialogButton)
                {
                    case DialogButton.Left:
                    {
                        using (var db = new GamemodeContext())
                        {
                            var salt = BCryptHelper.GenerateSalt(12);
                            var hash = BCryptHelper.HashPassword(ev.InputText, salt);
                            var player = new PlayerModel
                            {
                                Password = hash,
                                Name = Name
                            };
                            db.Players.Add(player);
                            db.SaveChanges();

                            LoginPlayer();
                        }
                    }
                        break;
                    case DialogButton.Right:
                    {
                        Kick();
                    }
                        break;
                }
            };
        }

        #endregion

        #region Oveerrides of BasePlayer

        public override void OnConnected(EventArgs e)
        {
            SetWorldBounds(2500.0f, 1850.0f, 631.2963f, -454.9898f);

            ToggleSpectating(true);

            if (FetchPlayerAccountData() is null)
                RegisterPlayer();
            else
                LoginPlayer();


            base.OnConnected(e);
        }

        public override void OnKeyStateChanged(KeyStateChangedEventArgs e)
        {
            base.OnKeyStateChanged(e);

            if (e.NewKeys == Keys.LookBehind && e.OldKeys != Keys.LookBehind) GeneralCommands.OnEngineCommand(this);
        }

        public override void OnRequestClass(RequestClassEventArgs e)
        {
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

            var dialogList = new ListDialog("Select spawn location", "Select", "Quit");
            dialogList.AddItem("Random spawn");
            dialogList.AddItem("Select Spawn");
            dialogList.AddItem("Last location");
            dialogList.Show(this);

            if (PlayerClass == PlayerClasses.TruckDriver)
                dialogList.Response += (sender, ev) =>
                {
                    if (ev.DialogButton != DialogButton.Right)
                    {
                        switch (ev.ListItem)
                        {
                            case 0:
                            {
                                var randomIndex = new Random().Next(TruckerSpawn.TruckerSpawns.Count);
                                SetSpawnInfo(0, Skin, TruckerSpawn.TruckerSpawns[randomIndex].Position,
                                    TruckerSpawn.TruckerSpawns[randomIndex].Angle);
                                Spawn();
                                break;
                            }
                            case 1:
                            {
                                var dialogSpawnsList = new TablistDialog("Select spawn", 1, "Select", "Cancel");
                                foreach (var spawn in TruckerSpawn.TruckerSpawns) dialogSpawnsList.Add(spawn.Name);
                                dialogSpawnsList.Show(this);
                                dialogSpawnsList.Response += (obj, eve) =>
                                {
                                    switch (eve.ListItem)
                                    {
                                        default:
                                        {
                                            SetSpawnInfo(0, Skin, TruckerSpawn.TruckerSpawns[eve.ListItem].Position,
                                                TruckerSpawn.TruckerSpawns[eve.ListItem].Angle);
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
                        var randomIndex = new Random().Next(TruckerSpawn.TruckerSpawns.Count);
                        SetSpawnInfo(0, Skin, TruckerSpawn.TruckerSpawns[randomIndex].Position,
                            TruckerSpawn.TruckerSpawns[randomIndex].Angle);
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

            BaseVehicle.Create(VehicleModelType.Alpha, Position, 0.0f, 3, 3);

            base.OnSpawned(e);
        }

        public override void OnText(TextEventArgs e)
        {
            e.SendToPlayers = false;

            SendClientMessageToAll(Color.White, $"{Color}{Name}[ID:{Id}]: {Color.White}{e.Text}");

            base.OnText(e);
        }

        public override void OnDisconnected(DisconnectEventArgs e)
        {
            SavePlayerPosition();

            base.OnDisconnected(e);
        }

        public override void OnDeath(DeathEventArgs e)
        {
            SavePlayerPosition();

            base.OnDeath(e);
        }

        public override void OnCommandText(CommandTextEventArgs e)
        {
            if (State != PlayerState.None && State != PlayerState.Wasted)
                base.OnCommandText(e);
            else SendClientMessage(Color.IndianRed, "You can't use commands while not spawned!");
        }

        public override void OnEnterVehicle(EnterVehicleEventArgs e)
        {
            if(e.Vehicle.Engine == false)
                SendClientMessage(Color.BlueViolet, $"Press 2 key to start vehicle engine.");

            base.OnEnterVehicle(e);
        }

        #endregion
    }
}