using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GamemodeDatabase.Models
{
    public class TruckerSpawnModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Angle { get; set; }

        public static List<TruckerSpawnModel> GetTruckerSpawnListNoTracking
        {
            get
            {
                using (var db = new GamemodeContext())
                {
                    return db.TruckerSpawns.AsNoTracking().ToList();
                }
            }
        }
    }
}