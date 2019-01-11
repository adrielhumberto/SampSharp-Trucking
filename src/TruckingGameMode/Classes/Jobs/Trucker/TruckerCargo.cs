using System.Collections.Generic;

namespace TruckingGameMode.Classes.Jobs.Trucker
{
    public class TruckerCargo
    {
        public static List<TruckerCargo> Cargoes = new List<TruckerCargo>
        {
            new TruckerCargo("Food"),
            new TruckerCargo("Canned Food"),
            new TruckerCargo("Fish"),
            new TruckerCargo("Mouse pads")
        };

        public TruckerCargo(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}