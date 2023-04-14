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

namespace AmongUsCEEE.Patches
{

    [HarmonyPatch(typeof(ProgressTracker))]
    [HarmonyPatch("FixedUpdate")]
    class OverrideTrackerBar
    {
        public static bool Prefix(ProgressTracker __instance)
        {
            if (PlayerTask.PlayerHasTaskOfType<IHudOverrideTask>(PlayerControl.LocalPlayer))
            {
                __instance.TileParent.enabled = false;
                return false;
            }
            if (!__instance.TileParent.enabled)
            {
                __instance.TileParent.enabled = true;
            }
            GameData instance = GameData.Instance;
            if (instance && instance.TotalTasks > 0)
            {
                int num = (instance.AllPlayers.Count);
                num -= instance.AllPlayers.ToArray().ToList().Count((GameData.PlayerInfo p) => p.Disconnected);
                num -= instance.AllPlayers.ToArray().ToList().Count((GameData.PlayerInfo p) => !(p.Role.GetRole().DoesTasks));
                switch (GameManager.Instance.LogicOptions.GetTaskBarMode())
                {
                    case TaskBarMode.Normal:
                        break;
                    case TaskBarMode.MeetingOnly:
                        if (!MeetingHud.Instance)
                        {
                            __instance.TileParent.material.SetFloat("_Buckets", (float)num);
                            __instance.TileParent.material.SetFloat("_FullBuckets", __instance.curValue);
                        }
                        break;
                    case TaskBarMode.Invisible:
                        __instance.gameObject.SetActive(false);
                        __instance.TileParent.material.SetFloat("_Buckets", (float)num);
                        __instance.TileParent.material.SetFloat("_FullBuckets", __instance.curValue);
                        break;
                    default:
                        __instance.TileParent.material.SetFloat("_Buckets", (float)num);
                        __instance.TileParent.material.SetFloat("_FullBuckets", __instance.curValue);
                        break;
                }
                float b = (float)instance.CompletedTasks / (float)instance.TotalTasks * (float)num;
                __instance.curValue = Mathf.Lerp(__instance.curValue, b, Time.fixedDeltaTime * 2f);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    [HarmonyPatch("SetHudActive")]
    class HudActiveOverride
    {
        public static bool Prefix(HudManager __instance, bool isActive)
        {
            GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
            CustomRole role = data.Role.GetRole();
            __instance.AbilityButton.ToggleVisible(isActive);
            if (isActive)
            {
                __instance.UseButton.Refresh();
                __instance.AbilityButton.Refresh();
            }
            else
            {
                __instance.UseButton.ToggleVisible(isActive);
                __instance.PetButton.ToggleVisible(isActive);
            }
            __instance.ReportButton.ToggleVisible(isActive && role.CanDo(RoleSpecials.Report));
            __instance.KillButton.ToggleVisible(isActive && role.CanDo(RoleSpecials.Kill) && !data.IsDead);
            __instance.SabotageButton.ToggleVisible(isActive && role.CanDo(RoleSpecials.Sabotage));
            __instance.AdminButton.ToggleVisible(false); //look into wtf adminbutton is
            __instance.ImpostorVentButton.ToggleVisible(isActive && role.CanDo(RoleSpecials.Vent));
            __instance.TaskPanel.gameObject.SetActive(isActive);
            __instance.roomTracker.gameObject.SetActive(isActive);
            IVirtualJoystick virtualJoystick = __instance.joystick;
            if (virtualJoystick != null)
            {
                virtualJoystick.ToggleVisuals(isActive);
            }
            VirtualJoystick virtualJoystick2 = __instance.joystickR;
            if (virtualJoystick2 == null)
            {
                return false;
            }
            virtualJoystick2.ToggleVisuals(isActive);
            return false;
        }
    }

    [HarmonyPatch(typeof(Vent))]
    [HarmonyPatch("SetOutline")]
    class VentOutlineOverride
    {
        public static void Postfix(Vent __instance, bool mainTarget)
        {
            Color color = PlayerControl.LocalPlayer.Data.Role.GetRole().Color;
            __instance.myRend.material.SetColor("_OutlineColor", color);
            __instance.myRend.material.SetColor("_AddColor", mainTarget ? color : Color.clear);
        }
    }

    [HarmonyPatch(typeof(ReportButton))]
    [HarmonyPatch("DoClick")]
    class ReportButtonOverride
    {
        public static bool Prefix()
        {
            if (!PlayerControl.LocalPlayer.Data.Role.GetRole().CanDo(RoleSpecials.Report)) return false;
            return true;
        }
    }
}
