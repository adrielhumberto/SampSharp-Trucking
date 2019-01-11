using System;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using SampSharp.Streamer.World;
using TruckingGameMode.Classes.Jobs.Trucker.Definitions;
using TruckingGameMode.World;

namespace TruckingGameMode.Classes.Jobs.Trucker
{
    public class TruckerJobHandling
    {
        public static void TruckerJobCheckpoint_Enter(object sender, PlayerEventArgs e)
        {
            var player = e.Player as Player;
            if (player?.PlayerClass == PlayerClasses.TruckDriver)
            {
                var checkpoint = sender as DynamicCheckpoint;
                var jobLocation = TruckerJobLocation.JobLocations.Find(x => x.Checkpoint == checkpoint);

                CompletePlayerJob(player, jobLocation);

                var choseDialog = new ListDialog("Select an option", "Select", "Close");
                choseDialog.AddItem("Quick Job");
                choseDialog.AddItem("Freight Market");
                choseDialog.AddItem("Cargo Market");

                choseDialog.Show(player);
                choseDialog.Response += (obj, ev) =>
                {
                    if (ev.DialogButton == DialogButton.Right)
                        return;

                    if (ev.ListItem == 0)
                    {
                        var quickJobDialog = new TablistDialog("Chose a cargo", 4, "Select", "Back")
                        {
                            Style = DialogStyle.TablistHeaders
                        };

                        quickJobDialog.Add("Cargo", "Price/km", "Weight", "Destination");
                        foreach (var job in jobLocation.JobList)
                            quickJobDialog.Add(job.JobCargo.Name,
                                $"${job.MoneyAwarded}/km",
                                $"{job.CargoWeight}T",
                                job.EndLocation.Name);
                        quickJobDialog.Show(player);

                        quickJobDialog.Response += (objj, evv) =>
                        {
                            if (evv.DialogButton == DialogButton.Right)
                            {
                                choseDialog.Show(player);
                            }
                            else
                            {
                                if (player.CurrentJob is null)
                                {
                                    player.CurrentJob = jobLocation.JobList[evv.ListItem];

                                    jobLocation.JobList.Remove(player.CurrentJob);
                                    jobLocation.JobList.TrimExcess();

                                    if (jobLocation.JobList.Count == 0)
                                        jobLocation.JobList = TruckerJobDetails.GenerateJobList(jobLocation);

                                    player.CurrentJob.JobType = TruckerJobType.QuickJob;

                                    player.CurrentJob.Truck = BaseVehicle.Create(VehicleModelType.Roadtrain,
                                        jobLocation.SpawnPosition, jobLocation.SpawnRotation, 2, 3);

                                    player.CurrentJob.Trailer = BaseVehicle.Create(VehicleModelType.ArticleTrailer,
                                        jobLocation.SpawnPosition, jobLocation.SpawnRotation, 2, 3);

                                    player.PutInVehicle(player.CurrentJob.Truck);
                                    player.CurrentJob.Truck.Trailer = player.CurrentJob.Trailer;

                                    player.CurrentJob.Truck.Engine = true;
                                }
                                else
                                {
                                    player.SendClientMessage(Color.IndianRed, "You already have a job!");
                                }
                            }
                        };
                    }
                };
            }
        }

        public static void CompletePlayerJob(Player player, TruckerJobLocation location)
        {
            if (player.CurrentJob is null)
                return;

            if (player.CurrentJob.Truck != player.Vehicle)
            {
                player.SendClientMessage(Color.IndianRed, "You have a wrong truck.");
                return;
            }

            if (player.CurrentJob.Trailer != player.Vehicle.Trailer)
            {
                player.SendClientMessage(Color.IndianRed, "You have a wrong trailer.");
                return;
            }

            if (player.CurrentJob.EndLocation != location) return;

            var moneyEarned = player.CurrentJob.MoneyAwarded *
                              (player.CurrentJob.StartLocation.Position.DistanceTo(location.Position) / 100) +
                              player.CurrentJob.CargoWeight * 1.25;

            player.Money += (int) Math.Round(moneyEarned);
            player.SendClientMessage(Color.GreenYellow, $"You earned ${(int) Math.Round(moneyEarned)}");

            if (player.CurrentJob.JobType == TruckerJobType.QuickJob)
            {
                player.CurrentJob.Truck.Dispose();
                player.CurrentJob.Trailer.Dispose();
            }

            player.CurrentJob = null;
        }
    }
}