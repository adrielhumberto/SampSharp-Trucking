using System;
using System.Collections.Generic;
using GamemodeDatabase.Data;
using TruckingGameMode.Classes.Jobs.Trucker.Definitions;
using TruckingGameMode.World;

namespace TruckingGameMode.Classes.Jobs.Trucker
{
    public class TruckerJobDetails
    {
        private TruckerJobDetails(TruckerJobLocation startLocation, TruckerJobLocation endLocation,
            TruckerCargoModel jobCargo, int moneyAwarded, int cargoWeight)
        {
            StartLocation = startLocation;
            EndLocation = endLocation;
            JobCargo = jobCargo;
            MoneyAwarded = moneyAwarded;
            CargoWeight = cargoWeight;
        }

        public TruckerJobLocation StartLocation { get; }
        public TruckerJobLocation EndLocation { get;  }

        public TruckerCargoModel JobCargo { get;}

        public int MoneyAwarded { get; }
        public int CargoWeight { get;  }

        public Vehicle Truck { get; set; }
        public Vehicle Trailer { get; set; }

        public TruckerJobType JobType { get; set; }

        public static List<TruckerJobDetails> GenerateJobList(TruckerJobLocation startPoint)
        {
            var list = new List<TruckerJobDetails>(10);
            var random = new Random();

            for (var i = 0; i < 10; i++)
            {
                var destination = TruckerJobLocation.JobLocations[random.Next(TruckerJobLocation.JobLocations.Count)];
                if (destination.Position.DistanceTo(startPoint.Position) <= 1.0)
                    destination = TruckerJobLocation.JobLocations[random.Next(TruckerJobLocation.JobLocations.Count)];

                list.Add(new TruckerJobDetails(startPoint,
                    destination,
                    TruckerCargoModel.GetCargoList[random.Next(TruckerCargoModel.GetCargoList.Count)],
                    random.Next(30, 140),
                    random.Next(1, 22)));
            }

            return list;
        }
    }
}