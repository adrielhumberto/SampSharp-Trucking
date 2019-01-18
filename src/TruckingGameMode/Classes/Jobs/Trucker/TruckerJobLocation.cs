using System.Collections.Generic;
using SampSharp.GameMode;
using SampSharp.Streamer.World;

namespace TruckingGameMode.Classes.Jobs.Trucker
{
    public class TruckerJobLocation
    {
        public static readonly List<TruckerJobLocation> JobLocations = new List<TruckerJobLocation>
        {
            new TruckerJobLocation("LVA Freight Market", new Vector3(1422.2754, 1052.8458, 10.6228),
                new Vector3(1416.0120, 1060.9197, 10.8130), 86.3904f),
            new TruckerJobLocation("LVA Freight Depot", new Vector3(1703.5624, 997.9811, 10.6261),
                new Vector3(1709.1815, 982.9061, 10.8203), 95.5498f),
            new TruckerJobLocation("Randolph Industrial Estate", new Vector3(1663.5758, 728.6444, 10.6265),
                new Vector3(1663.6227, 723.9113, 10.8203), 92.6572f),
            new TruckerJobLocation("Rockshore East Construction Site", new Vector3(2706.9246, 854.7203, 9.5651),
                new Vector3(2706.7993, 836.0386, 9.8438), 1.1864f),
            new TruckerJobLocation("Rockshore East", new Vector3(2844.0618, 956.3353, 10.5553),
                new Vector3(2838.9233, 976.7101, 10.7500), 179.1613f),
            new TruckerJobLocation("Creek Mall", new Vector3(2867.3020, 2576.5039, 10.6278),
                new Vector3(2869.6960, 2591.2410, 10.6719), 358.3896f),
            new TruckerJobLocation("K.A.C.C. Military Fuels", new Vector3(2523.7703, 2819.7188, 10.8203),
                new Vector3(2540.1360, 2782.7917, 10.8203), 95.1873f),
            new TruckerJobLocation("Whitewood Estates Butchery", new Vector3(979.0724, 2094.6133, 10.6282),
                new Vector3(980.1901, 2128.8977, 10.8203), 264.7254f),
            new TruckerJobLocation("Whitewood Estates Depot", new Vector3(1125.4618, 1964.5760, 10.6270),
                new Vector3(1135.9349, 1963.7217, 11.8304), 180.1999f),
            new TruckerJobLocation("Montgomery Depot", new Vector3(1219.4106, 189.2402, 19.7579),
                new Vector3(1209.5533, 187.3647, 20.4985), 335.0435f),
            new TruckerJobLocation("Montgomery Sprunk", new Vector3(1337.4369, 285.2093, 19.3691),
                new Vector3(1345.9575, 281.4611, 19.5615), 251.0696f),
            new TruckerJobLocation("Blueberry Xoomer", new Vector3(219.1559, 24.8225, 2.3838),
                new Vector3(225.6191, 25.0932, 2.5781), 270.4964f),
            new TruckerJobLocation("Blueberry Avery Constructions", new Vector3(313.4286, -231.3550, 1.3369),
                new Vector3(319.4592, -228.6643, 1.5011), 359.7739f),
            new TruckerJobLocation("Blueberry Cargo Bay", new Vector3(65.2834, -284.5645, 1.3834),
                new Vector3(53.6009, -278.7103, 1.6622), 351.9638f),
            new TruckerJobLocation("Ocean Docks Cargo Bay", new Vector3(2174.4465, -2266.1685, 13.1817),
                new Vector3(2215.9895, -2225.1279, 14.5571), 316.5438f),
            new TruckerJobLocation("Ocean Docks Port", new Vector3(2773.5259, -2417.2981, 13.4457),
                new Vector3(2771.3586, -2415.9053, 14.5406), 90.0084f),
            new TruckerJobLocation("Commerce Depot", new Vector3(1656.1073, -1806.4148, 13.3540),
                new Vector3(1646.8478, -1820.8857, 13.5358), 93.3399f),
            new TruckerJobLocation("Flint County", new Vector3(-64.5422, -1120.2345, 0.9040),
                new Vector3(-63.2126, -1130.7264, 1.0781), 65.5979f),
            new TruckerJobLocation("Fallen Tree", new Vector3(-575.8359, -501.7957, 25.3301),
                new Vector3(-592.3750, -507.9968, 25.5234), 0.7608f),
            new TruckerJobLocation("Easter Bay Chemicals", new Vector3(-1034.7004, -625.9224, 31.8127),
                new Vector3(-1025.5551, -624.6477, 32.0078), 4.5208f),
            new TruckerJobLocation("Easter Basin", new Vector3(-1693.8966, 25.8589, 3.3625),
                new Vector3(-1695.5153, 24.0928, 3.5547), 314.7004f),
            new TruckerJobLocation("Doherty", new Vector3(-2169.6521, -210.9558, 35.1273),
                new Vector3(-2163.7515, -219.3817, 35.3265), 270.8568f),
            new TruckerJobLocation("Doherty Construction Site", new Vector3(-2099.5127, 209.2432, 35.1053),
                new Vector3(-2099.5127, 209.2432, 35.3001), 270.8568f),
            new TruckerJobLocation("Angel Pine Sawmill", new Vector3(-2005.6653, -2411.9958, 30.4303),
                new Vector3(-1996.1210, -2422.3716, 30.6250), 48.1214f),
            new TruckerJobLocation("Whetstone Farm", new Vector3(-1417.4879, -1471.2158, 101.4137),
                new Vector3(-1422.6233, -1466.2675, 101.6355), 273.7001f),
            new TruckerJobLocation("Flint Range Farm", new Vector3(-376.8838, -1419.6680, 25.5321),
                new Vector3(-377.1396, -1429.2871, 25.7266), 0.8076f),
            new TruckerJobLocation("Green Palms Oil Refinery", new Vector3(261.4585, 1454.2505, 10.3920),
                new Vector3(269.9253, 1462.7909, 10.5859), 177.8659f),
            new TruckerJobLocation("Area 69", new Vector3(135.5378, 1946.4089, 19.1626),
                new Vector3(142.7180, 1956.8165, 19.4389), 359.4326f),
            new TruckerJobLocation("Las Payasadas Peacker feed'n'seeds", new Vector3(-317.7792, 2662.4277, 62.8628),
                new Vector3(-297.3721, 2662.5303, 62.7291), 86.0820f),
            new TruckerJobLocation("Bayside Marina", new Vector3(-2286.5066, 2283.7893, 4.7797),
                new Vector3(-2292.2388, 2269.2307, 4.9844), 357.0945f),
            new TruckerJobLocation("Spinybed", new Vector3(2345.8867, 2754.7688, 10.8203),
                new Vector3(2358.3904, 2764.9429, 10.8203), 177.6179f)
        };


        private TruckerJobLocation(string name, Vector3 position, Vector3 spawnPosition, float spawnRotation)
        {
            Name = name;
            Position = position;
            SpawnPosition = spawnPosition;
            SpawnRotation = spawnRotation;
        }

        public string Name { get; }
        public Vector3 Position { get;  }

        public Vector3 SpawnPosition { get; }
        public float SpawnRotation { get;  }

        public DynamicMapIcon MapIcon { get; set; }
        public DynamicCheckpoint Checkpoint { get; set; }

        public List<TruckerJobDetails> JobList { get; set; }
    }
}