using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Factories;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.World;

namespace TruckingGameMode.World
{
    [PooledType]
    public class Vehicle : BaseVehicle
    {
        #region Ovverides of create static functions
        public new static Vehicle Create(VehicleModelType vehicletype, Vector3 position, float rotation, int color1,
            int color2,
            int respawnDelay = -1, bool addAlarm = false)
        {
            var service = BaseMode.Instance.Services.GetService<IVehicleFactory>();

            return (Vehicle)service?.Create(vehicletype, position, rotation, color1, color2, respawnDelay, addAlarm);
        }

        public new static Vehicle CreateStatic(VehicleModelType vehicleType, Vector3 position, float rotation,
            int color1, int color2,
            int respawnDelay, bool addAlarm = false)
        {
            var service = BaseMode.Instance.Services.GetService<IVehicleFactory>();

            return (Vehicle)service?.CreateStatic(vehicleType, position, rotation, color1, color2, respawnDelay, addAlarm);
        }

        public new static Vehicle CreateStatic(VehicleModelType vehicleType, Vector3 position, float rotation,
            int color1, int color2)
        {
            var service = BaseMode.Instance.Services.GetService<IVehicleFactory>();

            return (Vehicle)service?.CreateStatic(vehicleType, position, rotation, color1, color2);
        }
        #endregion
    }
}