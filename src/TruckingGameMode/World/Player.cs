using System;
using SampSharp.GameMode;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.World;
using TruckingGameMode.Enums;
using TruckingGameMode.Spawns;
using TruckingGameMode.TempData;

namespace TruckingGameMode.World
{
    [PooledType]
    public class Player : BasePlayer
    {
        public PlayerClasses PlayerClass { get; set; }

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

            base.OnSpawned(e);
        }
    }
}