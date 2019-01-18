using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using SampSharp.GameMode.World;
using TruckingGameMode.World;

namespace TruckingGameMode.Commands.AdminCommands.AdminCommandPermissions
{
    public class LevelOneAdminPermission : IPermissionChecker
    {
        public bool Check(BasePlayer player)
        {
            return player is Player playerData && playerData.PlayerData().AdminLevel == 1;
        }

        public string Message => "You need level 1 admin level to use this command";
    }
}