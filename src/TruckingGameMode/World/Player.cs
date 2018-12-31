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
                        SetSpawnInfo(NoTeam, 0, GetPlayerPositionVector3FromDatabase(), FetchPlayerAccountData().FacingAngle);
                        Spawn();
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
            if (PlayerClass == PlayerClasses.TruckDriver)
            {
                var randomIndex = new Random().Next(TruckerSpawn.TruckerSpawns.Count);
                SetSpawnInfo(0, Skin, TruckerSpawn.TruckerSpawns[randomIndex].Position,
                    TruckerSpawn.TruckerSpawns[randomIndex].Angle);
            }

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

        #endregion
    }
}