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

    [HarmonyPatch(typeof(LogicGameFlowNormal))]
    [HarmonyPatch("CheckEndCriteria")]
    class OverrideEndLogic
    {
        public static bool Prefix(LogicGameFlowNormal __instance)
        {
            if (!GameData.Instance)
            {
                return false;
            }
            bool IsSabotageEnd = false;
            bool IsTaskComplete = GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks;
            //there used to be a lot of copied stuff here but due to my incompetence its no longer here
            if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
            {
                ISystemType funnySystem = ShipStatus.Instance.Systems[SystemTypes.LifeSupp];
                LifeSuppSystemType lifeSuppSystemType = funnySystem.Cast<LifeSuppSystemType>();
                if (lifeSuppSystemType.Countdown < 0f)
                {
                    IsSabotageEnd = true;
                    lifeSuppSystemType.Countdown = 10000f;
                }
            }
            if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
            {
                ISystemType funnySystem = ShipStatus.Instance.Systems[SystemTypes.Laboratory];
                ICriticalSabotage critsystem = funnySystem.Cast<ICriticalSabotage>();
                if (critsystem.Countdown < 0f)
                {
                    IsSabotageEnd = true;
                    critsystem.ClearSabotage();
                }
            }
            else if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor))
            {
                ISystemType funnySystem = ShipStatus.Instance.Systems[SystemTypes.Reactor];
                ICriticalSabotage critsystem = funnySystem.Cast<ICriticalSabotage>();
                if (critsystem.Countdown < 0f)
                {
                    IsSabotageEnd = true;
                    critsystem.ClearSabotage();
                }
            }
            RegisterHandler.Call("CheckEndCriteria", ReturnHandler.IfTrue, true, IsSabotageEnd, IsTaskComplete);
            return false;
        }
    }
}
