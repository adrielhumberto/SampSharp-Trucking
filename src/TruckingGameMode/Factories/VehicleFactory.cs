using System.Linq;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Factories;
using SampSharp.GameMode.World;
using TruckingGameMode.World;

namespace TruckingGameMode.Factories
{
    internal class VehicleFactory : BaseVehicleFactory
    {
        public VehicleFactory(BaseMode gameMode) : base(gameMode)
        {
        }

        public new virtual Vehicle Create(VehicleModelType vehicletype, Vector3 position, float rotation, int color1,
            int color2,
            int respawnDelay = -1, bool addAlarm = false)
        {
            var id = new[] {449, 537, 538, 569, 570, 590}.Contains((int) vehicletype)
                ? BaseVehicleFactoryInternal.Instance.AddStaticVehicleEx((int) vehicletype, position.X, position.Y,
                    position.Z, rotation, color1,
                    color2,
                    respawnDelay, addAlarm)
                : BaseVehicleFactoryInternal.Instance.CreateVehicle((int) vehicletype, position.X, position.Y,
                    position.Z, rotation, color1, color2,
                    respawnDelay, addAlarm);

            return id == BaseVehicle.InvalidId ? null : (Vehicle) Vehicle.FindOrCreate(id);
        }

        public new virtual Vehicle CreateStatic(VehicleModelType vehicletype, Vector3 position, float rotation,
            int color1,
            int color2,
            int respawnDelay, bool addAlarm = false)
        {
            var id = BaseVehicleFactoryInternal.Instance.AddStaticVehicleEx((int) vehicletype, position.X, position.Y,
                position.Z, rotation, color1,
                color2,
                respawnDelay, addAlarm);

            return id == BaseVehicle.InvalidId ? null : (Vehicle) Vehicle.FindOrCreate(id);
        }

        public new virtual Vehicle CreateStatic(VehicleModelType vehicletype, Vector3 position, float rotation,
            int color1,
            int color2)
        {
            var id = BaseVehicleFactoryInternal.Instance.AddStaticVehicle((int) vehicletype, position.X, position.Y,
                position.Z, rotation, color1,
                color2);

            return id == BaseVehicle.InvalidId ? null : (Vehicle) BaseVehicle.FindOrCreate(id);
        }
    }
}