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

    [HarmonyPatch(typeof(AchievementManager))]
    [HarmonyPatch("UpdateAchievementProgress")]
    class NoAchievementProgress
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(AchievementManager))]
    [HarmonyPatch("UnlockAchievement")]
    class NoAchievementUnlock
    {
        public static bool Prefix()
        {
            return false;
        }
    }
}
