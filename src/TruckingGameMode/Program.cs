using SampSharp.Core;

namespace TruckingGameMode
{
    class Program
    {
        static void Main()
        {
            new GameModeBuilder()
                .Use<GameMode>()
                .Run();
        }
    }
}
