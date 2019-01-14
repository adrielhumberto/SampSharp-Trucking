using System;
using System.ComponentModel.DataAnnotations;

namespace GamemodeDatabase.Models
{
    public class PlayerBanModel
    {
        public int Id { get; set; }

        [MaxLength(25)] public string Name { get; set; }
        [MaxLength(25) ]public string AdminName { get; set; }
        public string Reason { get; set; }

        public DateTime BanTime { get; set; }
        public DateTime IssuedTime { get; set; }
    }
}