using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using SampSharp.GameMode.World;
using TruckingGameMode.World;

namespace TruckingGameMode.Commands.AdminCommands.AdminCommandPermissions
{
    public class LevelThreeAdminPermission : IPermissionChecker
    {
        public bool Check(BasePlayer player)
        {
            return player is Player playerData && playerData.PlayerData().AdminLevel == 3;
        }

        public string Message => "You need level 3 admin level to use this command";
    }
}