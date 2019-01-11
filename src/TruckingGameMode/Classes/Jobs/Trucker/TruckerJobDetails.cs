﻿using System;
using System.Collections.Generic;
using SampSharp.GameMode.World;
using TruckingGameMode.Classes.Jobs.Trucker.Definitions;

namespace TruckingGameMode.Classes.Jobs.Trucker
{
    public class TruckerJobDetails
    {
        public TruckerJobDetails(TruckerJobLocation startLocation, TruckerJobLocation endLocation, TruckerCargo jobCargo, int moneyAwarded, int cargoWeight)
        {
            StartLocation = startLocation;
            EndLocation = endLocation;
            JobCargo = jobCargo;
            MoneyAwarded = moneyAwarded;
            CargoWeight = cargoWeight;
        }

        public TruckerJobLocation StartLocation { get; set; }
        public TruckerJobLocation EndLocation { get; set; }

        public TruckerCargo JobCargo { get; set; }

        public int MoneyAwarded { get; set; }
        public int CargoWeight { get; set; }

        public BaseVehicle Truck { get; set; }
        public BaseVehicle Trailer { get; set; }

        public TruckerJobType JobType { get; set; }

        public static List<TruckerJobDetails> GenerateJobList(TruckerJobLocation startPoint)
        {
            var list = new List<TruckerJobDetails>(10);
            var randomMoney = new Random();
            var randomWeight = new Random();
            var randomDestination = new Random();
            var randomCargo = new Random();

            for (var i = 0; i < 10; i++)
            {
                var destination = TruckerJobLocation.JobLocations[randomDestination.Next(TruckerJobLocation.JobLocations.Count)];
                if(destination.Position.DistanceTo(startPoint.Position) <= 1.0)
                    destination = TruckerJobLocation.JobLocations[randomDestination.Next(TruckerJobLocation.JobLocations.Count)];

                list.Add(new TruckerJobDetails(startPoint, 
                        destination, 
                        TruckerCargo.Cargoes[randomCargo.Next(TruckerCargo.Cargoes.Count)], 
                        randomMoney.Next(30, 140), 
                        randomWeight.Next(1, 22)));
            }

            return list;
        }
    }
}