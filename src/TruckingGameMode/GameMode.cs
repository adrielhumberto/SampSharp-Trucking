using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using GamemodeDatabase;
using GamemodeDatabase.Data;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using SampSharp.Streamer.World;
using TruckingGameMode.Classes.Jobs.Trucker;
using TruckingGameMode.Controllers;
using TruckingGameMode.Houses;
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
            controllers.Override(new VehicleController());
        }

        protected override void OnInitialized(EventArgs e)
        {
            #region Database checking

            try
            {
                using (var db = new MySqlConnection(DapperHelper.ConnectionString))
                {
                    db.Open();
                    db.Close();
                }
            }
            catch (DbException)
            {
                Console.WriteLine("Connection with the database can't be established.");
                Console.WriteLine("Server shutting down.");
                Environment.Exit(1);
            }

            #endregion

            #region GameMode settings

            ShowPlayerMarkers(PlayerMarkersMode.Global);
            ShowNameTags(true);
            ManualVehicleEngineAndLights();
            EnableStuntBonusForAll(false);
            DisableInteriorEnterExits();
            UsePlayerPedAnimations();

            SetGameModeText("Trucking#");
            SendRconCommand("hostname Trucking Evolved");
            SendRconCommand("language English");

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

            #region Houses

            foreach (var house in HouseModel.GetAllHouses())
                if (house.Owned == 0)
                    House.Houses.Add(new House(house.Id)
                    {
                        MapIcon =
                            new DynamicMapIcon(new Vector3(house.PositionX, house.PositionY, house.PositionZ), 31),
                        TextLabel = new DynamicTextLabel(
                            string.Format(StaticTexts.TextHouseForSale, Color.LightGreen, Color.White, house.Id, house.Price, house.MaxLevel),
                            Color.Teal, new Vector3(house.PositionX, house.PositionY, house.PositionZ + 1.0), 5.0f),
                        HousePickup = new DynamicPickup(1273, 1,
                            new Vector3(house.PositionX, house.PositionY, house.PositionZ), 10.0f)
                    });
                else
                    House.Houses.Add(new House(house.Id)
                    {
                        MapIcon =
                            new DynamicMapIcon(new Vector3(house.PositionX, house.PositionY, house.PositionZ), 32),
                        TextLabel = new DynamicTextLabel(
                            string.Format(StaticTexts.TextHouseOwned,
                                Color.LightGreen, Color.White, house.Id, house.Owner, house.Level),
                            Color.Teal, new Vector3(house.PositionX, house.PositionY, house.PositionZ + 1.0), 10.0f),
                        HousePickup = new DynamicPickup(1272, 1,
                            new Vector3(house.PositionX, house.PositionY, house.PositionZ), 10.0f)
                    });

            #endregion

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

        protected override void OnRconLoginAttempt(RconLoginAttemptEventArgs e)
        {
            foreach (var player in BasePlayer.All)
            {
                if(player.IP.Equals(e.IP))
                    player.Kick();
            }
            base.OnRconLoginAttempt(e);
        }
    }
}