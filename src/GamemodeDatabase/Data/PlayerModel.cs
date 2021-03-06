﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GamemodeDatabase.Data
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
        public int Score { get; set; }
        public float Health { get; set; }
        public float Armour { get; set; }
        public int TruckerJobs { get; set; }

        public DateTime LastActive { get; set; }
        public DateTime JoinedDate { get; set; }
        public DateTime MuteTime { get; set; }
    }
}