using System;
using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using TruckingGameMode.Controllers;
using TruckingGameMode.World;

namespace TruckingGameMode
{
    public class GameMode : BaseMode
    {
        private int _lastTimedMessage;

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

            #region Creating player classes

            //Trucker
            AddPlayerClass(258, new Vector3(0.0f, 0.0f, 0.0f), 0.0f); // id 0
            AddPlayerClass(190, new Vector3(0.0f, 0.0f, 0.0f), 0.0f); // id 1

            #endregion

            var timedMessagesTimer = new Timer(1000 * 60 * 2, true);
            timedMessagesTimer.Tick += (sender, ev) =>
            {
                BasePlayer.SendClientMessageToAll(Color.Wheat, TimedMessage.TimedMessages[_lastTimedMessage].Message);

                _lastTimedMessage++;

                if (_lastTimedMessage == TimedMessage.TimedMessages.Count)
                    _lastTimedMessage = 0;
            };

            base.OnInitialized(e);
        }

        protected override void OnPlayerCommandText(BasePlayer player, CommandTextEventArgs e)
        {
            if (player is Player playerData && playerData.IsLogged)
            {
                base.OnPlayerCommandText(player, e);
            }
        }
    }
}