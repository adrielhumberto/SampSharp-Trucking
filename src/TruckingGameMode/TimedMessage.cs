using System;
using System.Collections.Generic;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;

namespace TruckingGameMode
{
    public class TimedMessage
    {
        private static int _lastTimedMessage;

        private static readonly List<TimedMessage> TimedMessages = new List<TimedMessage>
        {
            new TimedMessage("Always drive on the right side of the road and follow the speed limit."),
            new TimedMessage("Don't give your account password to anyone. Admins will never ask you for your account details."),
            new TimedMessage("Remember: Using mods/scripts that will give you advantages is illegal."),
            new TimedMessage("Add our server to favorites for faster reconnect.")
        };


        private TimedMessage(string message)
        {
            Message = message;
        }

        private string Message { get; }

        public static void TimedMessagesTimer_Tick(object sender, EventArgs e)
        {
            BasePlayer.SendClientMessageToAll(Color.Blue, TimedMessages[_lastTimedMessage].Message);

            _lastTimedMessage++;

            if (_lastTimedMessage == TimedMessages.Count)
                _lastTimedMessage = 0;
        }
    }
}