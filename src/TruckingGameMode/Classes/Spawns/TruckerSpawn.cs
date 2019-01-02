using System.Collections.Generic;
using SampSharp.GameMode;

namespace TruckingGameMode.Classes.Spawns
{
    public class TruckerSpawn
    {
        public static List<TruckerSpawn> TruckerSpawns = new List<TruckerSpawn>();

        public TruckerSpawn(string name, Vector3 position, float angle)
        {
            Name = name;
            Position = position;
            Angle = angle;
        }

        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public float Angle { get; set; }
    }
}