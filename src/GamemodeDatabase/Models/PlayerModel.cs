using System.ComponentModel.DataAnnotations;

namespace GamemodeDatabase.Models
{
    public class PlayerModel
    {
        public int Id { get; set; }

        [MaxLength(25)] public string Name { get; set; }
        [MaxLength(61)] public string Password { get; set; }

        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float FacingAngle { get; set; }  

        public byte AdminLevel { get; set; }
        public int Money { get; set; }
    }
}