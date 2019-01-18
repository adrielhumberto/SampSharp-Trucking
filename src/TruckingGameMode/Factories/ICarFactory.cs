using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Factories;
using TruckingGameMode.World;

namespace TruckingGameMode.Factories
{
    public interface ICarFactory : IVehicleFactory
    {
        new Vehicle Create(VehicleModelType vehicletype, Vector3 position, float rotation, int color1,
            int color2,
            int respawnDelay = -1, bool addAlarm = false);

        new Vehicle CreateStatic(VehicleModelType vehicletype, Vector3 position, float rotation, int color1,
            int color2,
            int respawnDelay, bool addAlarm = false);

        new Vehicle CreateStatic(VehicleModelType vehicletype, Vector3 position, float rotation, int color1,
            int color2);
    }
}