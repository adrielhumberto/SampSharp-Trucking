using System.Collections.Generic;

namespace TruckingGameMode
{
    public class TimedMessage
    {
        public static List<TimedMessage> TimedMessages = new List<TimedMessage>
        {
            new TimedMessage("Beware of speed traps (60kph in the city, 90kph on roads, 120kph on highways)"),
            new TimedMessage("You want to refuel your vehicle? Park on a refuel-pickup and honk the horn")
        };


        public TimedMessage(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}