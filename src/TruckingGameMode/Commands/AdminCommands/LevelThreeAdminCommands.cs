using Dapper;
using GamemodeDatabase;
using MySql.Data.MySqlClient;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using TruckingGameMode.World;

namespace TruckingGameMode.Commands.AdminCommands
{
    [CommandGroup("admin", PermissionChecker = typeof(LevelThreeAdminCommands))]
    public class LevelThreeAdminCommands
    {
        [Command("setadmin", Shortcut = "setadmin")]
        public static void OnSetAdminCommand(BasePlayer sender, Player playerId, int level)
        {
            if (level < 0 || level > 3)
            {
                sender.SendClientMessage(Color.IndianRed, "Invalid admin level.");
                return;
            }

            if (sender.Id == playerId.Id)
            {
                sender.SendClientMessage(Color.IndianRed, "You can't set yourself admin.");
                return;
            }

            if (playerId.GetPlayerDataById().AdminLevel == 3)
            {
                sender.SendClientMessage(Color.IndianRed, "You can't set admin level to highest admin.");
                return;
            }

            using (var db = new MySqlConnection(DapperHelper.ConnectionString))
            {
                db.Execute(@"UPDATE players SET AdminLevel = @Level WHERE Id = @Id",
                    new {Level = level, Id = playerId.DbId});
            }

            playerId.SendClientMessage(Color.GreenYellow,
                level > playerId.GetPlayerDataById().AdminLevel
                    ? $"You got promoted to admin level {level} by {sender.Name}."
                    : $"You got demoted to admin level {level} by {sender.Name}.");

            sender.SendClientMessage(Color.GreenYellow, $"You set admin level {level} to {playerId.Name}.");
        }
    }
}