using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Reactor.Utilities.Extensions;
using AmongUs.GameOptions;

namespace AmongUsCEEE.Patches
{
    [HarmonyPatch(typeof(NormalGameManager))]
    [HarmonyPatch("InitComponents")]
    class SpawnCustomRoles
    {

        public static void Postfix()
        {
            Gamemode CurrentGamemode = ModLoader.CurrentGamemode; //placeholder
            List<RoleBehaviour> RBH = new List<RoleBehaviour>();
            for (int i = 0; i < CurrentGamemode.Roles.Count; i++)
            {
                CustomRole curRole = CurrentGamemode.Roles[i];
                RoleBehaviour rr = new GameObject().AddComponent<RootRole>();
                RBH.Add(rr);
                rr.Role = (RoleTypes)i;
                rr.TasksCountTowardProgress = curRole.DoesTasks;
                rr.CanVent = curRole.Specials.HasFlag(RoleSpecials.Vent);
                rr.CanUseKillButton = curRole.Specials.HasFlag(RoleSpecials.Kill);
                rr.DontDestroyOnLoad();
            }
            RoleManager.Instance.AllRoles = RBH.ToArray();
        }
    }
}
