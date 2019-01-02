using System.Collections.Generic;
using SampSharp.GameMode;

namespace TruckingGameMode.Classes.Spawns
{
    public class TruckerSpawn
    {
        public static List<TruckerSpawn> TruckerSpawns = new List<TruckerSpawn>()
        {
            new TruckerSpawn("LVA Freight Market", new Vector3(1470.9402f, 974.7820f, 10.8203f), 2.3979f),
            new TruckerSpawn("Rockshore East", new Vector3(2814.4407f, 971.6664f, 10.7500f), 170.4394f),
            new TruckerSpawn("Spinybed", new Vector3(2371.6941f, 2758.8669f, 10.8203f), 180.1616f),
            new TruckerSpawn("Whitewood Estates", new Vector3(1053.7681f, 2148.0027f, 10.8203f), 85.1938f),
            new TruckerSpawn("Red County", new Vector3(-49.6863f, -271.9140f, 6.6332f), 180.7621f),
            new TruckerSpawn("LS Docks", new Vector3(2728.5828f, -2394.4233f, 13.6328f), 185.8147f),
            new TruckerSpawn("LS Docks 2", new Vector3(2522.3501f, -2118.8274f, 13.5469f), 356.4000f),
            new TruckerSpawn("Flint County", new Vector3(-77.7715f, -1136.3878f, 1.0781f), 77.6688f),
            new TruckerSpawn("Fallen Tree", new Vector3(-516.2404f, -504.8649f, 25.5234f), 356.9146f),
            new TruckerSpawn("Doherty", new Vector3(-2136.6604f, -247.9970f, 36.4886f), 268.6135f),
            new TruckerSpawn("Whetstone", new Vector3(-1561.8169f, -2734.3469f, 48.7435f), 154.3250f)
        };

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