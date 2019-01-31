using SampSharp.GameMode.Controllers;
using TruckingGameMode.Display.TextDraws;

namespace TruckingGameMode.Controllers
{
    public class CustomPlayerTextDrawController : PlayerTextDrawController
    {
        public override void RegisterTypes()
        {
            TruckerJobTextDraw.Register<TruckerJobTextDraw>();
        }
    }
}