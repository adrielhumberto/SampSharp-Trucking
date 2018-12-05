using System;
using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.Definitions;
using TruckingGameMode.Controllers;
using TruckingGameMode.Spawns;

namespace TruckingGameMode
{
    public class GameMode : BaseMode
    {
        protected override void LoadControllers(ControllerCollection controllers)
        {
            base.LoadControllers(controllers);

            controllers.Remove<BasePlayerController>();
            controllers.Add(new PlayerController());
        }

        protected override void OnInitialized(EventArgs e)
        {
            #region GameMode settings

            ShowPlayerMarkers(PlayerMarkersMode.Global);
            ShowNameTags(true);
            ManualVehicleEngineAndLights();
            EnableStuntBonusForAll(false);
            DisableInteriorEnterExits();
            UsePlayerPedAnimations();

            #endregion

            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn(new Vector3(1491.6395f, 974.1372f, 10.8489f), 91.1944f));
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn(new Vector3(1482.0189f, 1010.8044f, 10.8203f), 104.8128f));
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn(new Vector3(1523.9229f, 1027.7963f, 10.8203f), 271.0261f));

            #region Creating player classes

            //Trucker
            AddPlayerClass(258, new Vector3(0.0f, 0.0f, 0.0f), 0.0f); // id 0
            AddPlayerClass(190, new Vector3(0.0f, 0.0f, 0.0f), 0.0f); // id 1

            #endregion


            base.OnInitialized(e);
        }
    }
}