using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsCEEE.Patches
{
    [HarmonyPatch(typeof(PlayerControl))]
    [HarmonyPatch("FixedUpdate")]
    public class PlayerFixedUpdatePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!GameData.Instance)
            {
                return;
            }
            GameData.PlayerInfo data = __instance.Data;
            if (data == null || data.Role == null)
            {
                return;
            }

            CustomRole role = data.Role.GetRole();
            /*if (role.CanDo(RoleSpecials.Vent) && __instance.CanMove && !data.IsDead)
            {
                HudManager.Instance.ImpostorVentButton.SetEnabled();
            }
            else
            {
                HudManager.Instance.ImpostorVentButton.SetDisabled();
            }*/
        }
    }
}
