using System;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP;
using SampSharp.Streamer.World;
using TruckingGameMode.Classes.Jobs.Trucker.Definitions;
using TruckingGameMode.World;

namespace TruckingGameMode.Classes.Jobs.Trucker
{
    public static class TruckerJobHandling
    {
        public static void TruckerJobCheckpoint_Enter(object sender, PlayerEventArgs e)
        {
            var player = e.Player as Player;
            if (player?.PlayerClass != PlayerClasses.TruckDriver)
                return;

            var checkpoint = sender as DynamicCheckpoint;
            var jobLocation = TruckerJobLocation.JobLocations.Find(x => x.Checkpoint == checkpoint);

            if (player.CurrentJob != null)
                CompletePlayerJob(player, jobLocation);

            ShowJobMenuToPlayer(player, jobLocation);
        }

        private static void CompletePlayerJob(Player player, TruckerJobLocation location)
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

            player.TruckerJobs += 1;
            player.Score += 1;

            player.Money += (int) Math.Round(moneyEarned);
            player.SendClientMessage(Color.GreenYellow, $"You earned ${(int) Math.Round(moneyEarned)}");

            switch (player.CurrentJob.JobType)
            {
                case TruckerJobType.QuickJob:
                    player.CurrentJob.Truck.Dispose();
                    player.CurrentJob.Trailer.Dispose();
                    break;
                case TruckerJobType.FreightMarket:
                    player.CurrentJob.Trailer.Dispose();
                    break;
                case TruckerJobType.CargoMarket:
                    break;
                default:
                    break;
            }

            player.JobTextDraw.Hide();

            player.CurrentJob = null;
        }

        private static void ShowJobListToPlayer(Player player, TruckerJobLocation jobLocation, TruckerJobType type)
        {
            var selectJobDialog = new TablistDialog("Chose a cargo", 4, "Select", "Back")
            {
                Style = DialogStyle.TablistHeaders
            };

            selectJobDialog.Add("Cargo", "Price/km", "Weight", "Destination");

            foreach (var job in jobLocation.JobList)
                selectJobDialog.Add(job.JobCargo.Name,
                    $"${job.MoneyAwarded}/km",
                    $"{job.CargoWeight}T",
                    job.EndLocation.Name);

            selectJobDialog.Show(player);

            selectJobDialog.Response += (sender, e) => SelectJobDialog_Response(sender, e, player, type, jobLocation);
        }

        private static void SelectJobDialog_Response(object sender, DialogResponseEventArgs e, Player player,
            TruckerJobType type, TruckerJobLocation jobLocation)
        {
            switch (type)
            {
                case TruckerJobType.QuickJob when e.DialogButton == DialogButton.Right:
                    ShowJobMenuToPlayer(player, jobLocation);
                    break;
                case TruckerJobType.QuickJob when player.CurrentJob is null:
                {
                    player.CurrentJob = jobLocation.JobList[e.ListItem];

                    jobLocation.JobList.Remove(player.CurrentJob);
                    jobLocation.JobList.TrimExcess();

                    if (jobLocation.JobList.Count == 0)
                        jobLocation.JobList = TruckerJobDetails.GenerateJobList(jobLocation);

                    player.CurrentJob.JobType = TruckerJobType.QuickJob;

                    player.CurrentJob.Truck = Vehicle.Create(VehicleModelType.Roadtrain,
                        jobLocation.SpawnPosition, jobLocation.SpawnRotation, 2, 3);

                    player.CurrentJob.Trailer = Vehicle.Create(VehicleModelType.ArticleTrailer,
                        jobLocation.SpawnPosition, jobLocation.SpawnRotation, 2, 3);

                    player.PutInVehicle(player.CurrentJob.Truck);
                    player.CurrentJob.Truck.Trailer = player.CurrentJob.Trailer;

                    player.CurrentJob.Truck.Engine = true;

                    player.JobTextDraw.Text = $"You delivering {player.CurrentJob.JobCargo.Name} from {player.CurrentJob.StartLocation.Name} to {player.CurrentJob.EndLocation.Name}";
                    player.JobTextDraw.Show();
                    break;
                }
                case TruckerJobType.QuickJob:
                    player.SendClientMessage(Color.IndianRed, "You already have a job!");
                    break;
                case TruckerJobType.FreightMarket when e.DialogButton == DialogButton.Right:
                    ShowJobMenuToPlayer(player, jobLocation);
                    break;
                case TruckerJobType.FreightMarket when player.CurrentJob is null:
                {
                    if (player.Vehicle is null)
                    {
                        player.SendClientMessage(Color.IndianRed, "You are not driving any truck!");
                        return;
                    }

                    player.CurrentJob = jobLocation.JobList[e.ListItem];

                    jobLocation.JobList.Remove(player.CurrentJob);
                    jobLocation.JobList.TrimExcess();

                    if (jobLocation.JobList.Count == 0)
                        jobLocation.JobList = TruckerJobDetails.GenerateJobList(jobLocation);

                    player.CurrentJob.JobType = TruckerJobType.FreightMarket;

                    player.CurrentJob.Truck = (Vehicle) player.Vehicle;

                    player.CurrentJob.Trailer = Vehicle.Create(VehicleModelType.ArticleTrailer,
                        jobLocation.SpawnPosition, jobLocation.SpawnRotation, 2, 3);

                    player.CurrentJob.Truck.Trailer = player.CurrentJob.Trailer;

                    player.CurrentJob.Truck.Engine = true;

                    player.JobTextDraw.Text = $"You delivering {player.CurrentJob.JobCargo.Name} from {player.CurrentJob.StartLocation.Name} to {player.CurrentJob.EndLocation.Name}";
                    player.JobTextDraw.Show();
                        break;
                }
                case TruckerJobType.FreightMarket:
                    player.SendClientMessage(Color.IndianRed, "You already have a job!");
                    break;
                default:
                {
                    if (e.DialogButton == DialogButton.Right)
                    {
                        ShowJobMenuToPlayer(player, jobLocation);
                    }
                    else
                    {
                        if (player.CurrentJob is null)
                        {
                            if (player.Vehicle is null)
                            {
                                player.SendClientMessage(Color.IndianRed, "You are not driving any truck!");
                                return;
                            }

                            if (player.Vehicle.Trailer is null)
                            {
                                player.SendClientMessage(Color.IndianRed, "You are not having a trailer attached!");
                                return;
                            }

                            player.CurrentJob = jobLocation.JobList[e.ListItem];

                            jobLocation.JobList.Remove(player.CurrentJob);
                            jobLocation.JobList.TrimExcess();

                            if (jobLocation.JobList.Count == 0)
                                jobLocation.JobList = TruckerJobDetails.GenerateJobList(jobLocation);

                            player.CurrentJob.JobType = TruckerJobType.CargoMarket;

                            player.CurrentJob.Truck = (Vehicle) player.Vehicle;
                            player.CurrentJob.Trailer = (Vehicle) player.Vehicle.Trailer;

                            player.CurrentJob.Truck.Engine = true;

                            player.JobTextDraw.Text = $"You delivering {player.CurrentJob.JobCargo.Name} from {player.CurrentJob.StartLocation.Name} to {player.CurrentJob.EndLocation.Name}";
                            player.JobTextDraw.Show();
                            }
                        else
                        {
                            player.SendClientMessage(Color.IndianRed, "You already have a job!");
                        }
                    }

                    break;
                }
            }
        }

        private static void ShowJobMenuToPlayer(Player player, TruckerJobLocation jobLocation)
        {
            var jobMenuDialog = new ListDialog("Select an option", "Select", "Close");
            jobMenuDialog.AddItem("Quick Job");
            jobMenuDialog.AddItem("Freight Market");
            jobMenuDialog.AddItem("Cargo Market");

            jobMenuDialog.Show(player);

            jobMenuDialog.Response += (sender, e) => ChoseDialog_Response(sender, e, player, jobLocation);
        }

        private static void ChoseDialog_Response(object sender, DialogResponseEventArgs e, Player player,
            TruckerJobLocation jobLocation)
        {
            if (e.DialogButton == DialogButton.Right)
                return;

            switch (e.ListItem)
            {
                case 0:
                    ShowJobListToPlayer(player, jobLocation, TruckerJobType.QuickJob);
                    break;
                case 1:
                    ShowJobListToPlayer(player, jobLocation, TruckerJobType.FreightMarket);
                    break;
                case 2:
                    ShowJobListToPlayer(player, jobLocation, TruckerJobType.CargoMarket);
                    break;
            }
        }
    }
}