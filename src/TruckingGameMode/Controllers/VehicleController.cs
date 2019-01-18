using SampSharp.GameMode.Controllers;
using TruckingGameMode.World;

namespace TruckingGameMode.Controllers
{
    public class VehicleController : BaseVehicleController
    {
        public override void RegisterTypes()
        {
            Vehicle.Register<Vehicle>();
        }
    }
}