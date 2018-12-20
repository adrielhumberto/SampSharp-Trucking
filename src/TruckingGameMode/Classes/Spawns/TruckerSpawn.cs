using System.Collections.Generic;
using SampSharp.GameMode;

namespace TruckingGameMode.Classes.Spawns
{
    public class TruckerSpawn
    {
        public static List<TruckerSpawn> TruckerSpawns = new List<TruckerSpawn>();

        public TruckerSpawn(Vector3 position, float angle)
        {
            Position = position;
            Angle = angle;
        }

        public Vector3 Position { get; set; }
        public float Angle { get; set; }
    }
}