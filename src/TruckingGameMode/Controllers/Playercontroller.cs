using SampSharp.GameMode.Controllers;
using TruckingGameMode.World;

namespace TruckingGameMode.Controllers
{
    public class PlayerController : BasePlayerController
    {
        
        public override void RegisterTypes()
        {
            Player.Register<Player>();
        }
    }
}