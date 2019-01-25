using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using SampSharp.GameMode.World;
using TruckingGameMode.World;

namespace TruckingGameMode.Commands.AdminCommands.AdminCommandPermissions
{
    public class LevelTwoAdminPermission : IPermissionChecker
    {
        public bool Check(BasePlayer player)
        {
            return player is Player playerData && playerData.PlayerData().AdminLevel >= 2;
        }

        public string Message => "You need level 2 admin level to use this command";
    }
}