using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.IL2CPP;
using UnityEngine;
using HarmonyLib;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using UnityEngine.SceneManagement;
using Reactor.Utilities.Attributes;
using System.Reflection;
using System.Diagnostics;
using AmongUs.GameOptions;
using InnerNet;
using MoonSharp.Interpreter;
using Reactor.Utilities.Extensions;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Rewired;
using Il2CppSystem.Linq;
using Il2CppInterop.Runtime.Injection;

namespace AmongUsCEEE
{
    //TODO: for meeting hud, disable guardian angel stuff


    [BepInPlugin("mtm101.rulerp.moogus.amongusce", "Among Us: CEEE", "0.0.0.0")]
    [BepInProcess("Among Us.exe")]
    public class AmongUsCEEE : BasePlugin
    {
        public Harmony Harmony { get; } = new Harmony("mtm101.rulerp.moogus.amongusce");

        public override void Load()
        {
            Harmony.PatchAll();
            ClassInjector.RegisterTypeInIl2Cpp(typeof(CustomRole));
            UserData.RegisterAssembly(typeof(AmongUsCEEE).Assembly); //register all lua stuff
        }
    }


    [HarmonyPatch(typeof(VersionShower))]
    [HarmonyPatch("Start")]
    class TextPatch
    {

        public static void Postfix(VersionShower __instance)
        {
            //DestroyableSingleton<ModManager>.Instance.ShowModStamp(); //reactor handles this
            __instance.text.text = "Among Us: CE";
            if (ModLoader.Mods.Count == 0)
            {
                ModLoader.LoadMods();
            }


            //__instance.text.text += "\nMods:" + ModLoader.Mods.Count;

        }
    }

    /*[HarmonyPatch(typeof(TranslationController))]
    [HarmonyPatch("GetString")]
    class CustomStrings
    {
        public static bool Prefix(StringNames id, object[] parts, ref string __result)
        {
            if ((int) id == int.MaxValue)
            {
                if (parts.Length == 0) throw new ArgumentException("Custom string called with no custom parts!");
#pragma warning disable CS8601 // Possible null reference assignment.
                __result = parts[0].ToString();
#pragma warning restore CS8601 // Possible null reference assignment.
                return false;
            }
            return true;
        }
    }*/

    [HarmonyPatch(typeof(RoleManager))]
    [HarmonyPatch("SelectRoles")]
    class AssignPatch
    {

        public static bool Prefix(RoleManager __instance)
        {

            Il2CppSystem.Collections.Generic.List<ClientData> list = new Il2CppSystem.Collections.Generic.List<ClientData>();
            AmongUsClient.Instance.GetAllClients(list);
            List<GameData.PlayerInfo> list2 = (from c in list.ToArray().ToList()
                                               where c.Character != null
                                               where c.Character.Data != null
                                               where !c.Character.Data.Disconnected && !c.Character.Data.IsDead
                                               orderby c.Id
                                               select c.Character.Data).ToList<GameData.PlayerInfo>();

            //this is for dummies, might decide to remove this
            foreach (GameData.PlayerInfo playerInfo in GameData.Instance.AllPlayers)
            {
                if (playerInfo.Object != null && playerInfo.Object.isDummy)
                {
                    list2.Add(playerInfo);
                }
            }
            /*IGameOptions currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
            int adjustedNumImpostors = GameOptionsManager.Instance.CurrentGameOptions.GetAdjustedNumImpostors(list2.Count);*/

            List<GameData.PlayerInfo> playersConverted = list2.ToArray().ToList();
            foreach (GameData.PlayerInfo pf in playersConverted)
            {
                pf.Object.RpcSetRole(RoleTypes.Crewmate);
            }

            return false;

        }
    }

    [HarmonyPatch(typeof(IntroCutscene))]
    [HarmonyPatch("SelectTeamToShow")]
    class OverrideIntroTeams
    {
        public static bool Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> __result)
        {
            //.WrapToIl2Cpp().Cast<Il2CppSystem.Collections.Generic.List<PlayerControl>>();
            System.Linq.IOrderedEnumerable<PlayerControl> pcL = (from pcd in GameData.Instance.AllPlayers.ToArray().ToList()
                where !pcd.Disconnected
                where (PlayerControl.LocalPlayer.Data.Role.GetRole().Team.CanSee(pcd.Role.GetRole().Team) || (pcd.Object == PlayerControl.LocalPlayer))
                select pcd.Object).OrderBy(delegate (PlayerControl pc)
                {
                    if (!(pc == PlayerControl.LocalPlayer))
                    {
                        return 1;
                    }
                    return 0;
                });
            __result = pcL.ToList().ToIl2CppList();
            return false;
        }
    }

    [HarmonyPatch(typeof(IntroCutscene))]
    [HarmonyPatch("BeginCrewmate")]
    class OverrideTeamName
    {
        public static void Postfix(IntroCutscene __instance)
        {
            Team myTeam = PlayerControl.LocalPlayer.Data.Role.GetRole().Team;
            __instance.TeamTitle.text = myTeam.DisplayName;
            __instance.ImpostorText.text = myTeam.TeamBlurb;
            __instance.TeamTitle.color = myTeam.Color;
            __instance.BackgroundBar.material.SetColor("_Color", myTeam.Color);
        }
    }

    [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__35))]
    [HarmonyPatch(nameof(IntroCutscene._ShowRole_d__35.MoveNext))]
    class OverrideRoleName
    {
        public static void Postfix(IntroCutscene._ShowRole_d__35 __instance)
        {
            IntroCutscene instance = __instance.__4__this;
            CustomRole myRole = PlayerControl.LocalPlayer.Data.Role.GetRole();
            instance.BackgroundBar.material.SetColor("_Color", myRole.Color);
            instance.RoleText.text = myRole.DisplayName;
            instance.RoleText.color = myRole.Color;
            instance.RoleBlurbText.color = myRole.Color;
            instance.RoleBlurbText.text = myRole.SmallBlurb;
            instance.YouAreText.color = myRole.Color;
            instance.YouAreText.color = myRole.Color;
        }
    }

}
