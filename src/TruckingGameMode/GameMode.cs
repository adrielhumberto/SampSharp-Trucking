using System;
using GamemodeDatabase;
using Microsoft.EntityFrameworkCore;
using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using SampSharp.Streamer.World;
using TruckingGameMode.Classes.Jobs.Trucker;
using TruckingGameMode.Controllers;
using TruckingGameMode.World;

namespace TruckingGameMode
{
    public class GameMode : BaseMode
    {
        private Timer _jobListRefresh;
        private int _lastTimedMessage;

        protected override void LoadControllers(ControllerCollection controllers)
        {
            base.LoadControllers(controllers);

            controllers.Override(new PlayerController());
        }

        protected override async void OnInitialized(EventArgs e)
        {
            #region GameMode settings

            ShowPlayerMarkers(PlayerMarkersMode.Global);
            ShowNameTags(true);
            ManualVehicleEngineAndLights();
            EnableStuntBonusForAll(false);
            DisableInteriorEnterExits();
            UsePlayerPedAnimations();

            SetGameModeText("Trucking#");
            SendRconCommand("hostname Trucking Evolved");

            #endregion

            #region Creating player classes

            //Trucker
            AddPlayerClass(258, new Vector3(0.0f, 0.0f, 0.0f), 0.0f); // id 0
            AddPlayerClass(190, new Vector3(0.0f, 0.0f, 0.0f), 0.0f); // id 1

            #endregion

            #region Timed info messages

            var timedMessagesTimer = new Timer(1000 * 60 * 2, true);
            timedMessagesTimer.Tick += (sender, ev) =>
            {
                BasePlayer.SendClientMessageToAll(Color.Wheat, TimedMessage.TimedMessages[_lastTimedMessage].Message);

                _lastTimedMessage++;

                if (_lastTimedMessage == TimedMessage.TimedMessages.Count)
                    _lastTimedMessage = 0;
            };

            #endregion

            #region JobLocations setup

            _jobListRefresh = new Timer(600000, true);
            _jobListRefresh.Tick += _jobListRefresh_Tick;

            foreach (var jobLocation in TruckerJobLocation.JobLocations)
            {
                jobLocation.MapIcon =
                    new DynamicMapIcon(jobLocation.Position, 51, MapIconType.Global, streamDistance: 150);
                jobLocation.Checkpoint = new DynamicCheckpoint(jobLocation.Position, 4f);

                jobLocation.JobList = TruckerJobDetails.GenerateJobList(jobLocation);

                jobLocation.Checkpoint.Enter += TruckerJobHandling.TruckerJobCheckpoint_Enter;
            }

            #endregion

            try
            {
                using (var db = new GamemodeContext())
                {
                    await db.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            base.OnInitialized(e);
        }

        private void _jobListRefresh_Tick(object sender, EventArgs e)
        {
            foreach (var jobLocation in TruckerJobLocation.JobLocations)
                jobLocation.JobList = TruckerJobDetails.GenerateJobList(jobLocation);
        }

        protected override void OnPlayerCommandText(BasePlayer player, CommandTextEventArgs e)
        {
            if (player is Player playerData && playerData.IsLogged) base.OnPlayerCommandText(player, e);
        }
    }
}