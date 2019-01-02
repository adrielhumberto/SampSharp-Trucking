using System;
using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using TruckingGameMode.Classes.Spawns;
using TruckingGameMode.Controllers;

namespace TruckingGameMode
{
    public class GameMode : BaseMode
    {
        protected override void LoadControllers(ControllerCollection controllers)
        {
            base.LoadControllers(controllers);

            controllers.Override(new PlayerController());
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

            #region Trucker Spawns
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn("LVA Freight Market", new Vector3(1470.9402f, 974.7820f, 10.8203f), 2.3979f));
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn("Rockshore East", new Vector3(2814.4407f, 971.6664f, 10.7500f), 170.4394f));
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn("Spinybed", new Vector3(2371.6941f, 2758.8669f, 10.8203f), 180.1616f));
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn("Whitewood Estates", new Vector3(1053.7681f, 2148.0027f, 10.8203f), 85.1938f));
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn("Red County", new Vector3(-49.6863f, -271.9140f, 6.6332f), 180.7621f));
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn("LS Docks", new Vector3(2728.5828f, -2394.4233f, 13.6328f), 185.8147f));
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn("LS Docks 2", new Vector3(2522.3501f, -2118.8274f, 13.5469f), 356.4000f));
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn("Flint County", new Vector3(-77.7715f, -1136.3878f, 1.0781f), 77.6688f));
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn("Fallen Tree", new Vector3(-516.2404f, -504.8649f, 25.5234f), 356.9146f));
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn("Doherty", new Vector3(-2136.6604f, -247.9970f, 36.4886f), 268.6135f));
            TruckerSpawn.TruckerSpawns.Add(new TruckerSpawn("Whetstone", new Vector3(-1561.8169f, -2734.3469f, 48.7435f), 154.3250f));
            #endregion

            #region Creating player classes

            //Trucker
            AddPlayerClass(258, new Vector3(0.0f, 0.0f, 0.0f), 0.0f); // id 0
            AddPlayerClass(190, new Vector3(0.0f, 0.0f, 0.0f), 0.0f); // id 1

            #endregion

            base.OnInitialized(e);
        }

        protected override void OnPlayerCommandText(BasePlayer player, CommandTextEventArgs e)
        {
            if (player.State != PlayerState.None && player.State != PlayerState.Wasted)
            {
                base.OnPlayerCommandText(player, e);
            }
            else player.SendClientMessage(Color.IndianRed,"You can't use commands while not spawned!");
        }
    }
}