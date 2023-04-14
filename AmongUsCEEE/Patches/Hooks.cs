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

    [HarmonyPatch(typeof(ChatController))]
    [HarmonyPatch("AddChat")]
    class TextPatch
    {
        public static void HandleChat(PlayerControl sourcePlayer, string chatText)
        {
            RegisterHandler.Call("onChat", ReturnHandler.Ignore, false, chatText);
        }
        public static void Postfix(ChatController __instance, PlayerControl sourcePlayer, string chatText)
        {
            //HandleChat(sourcePlayer,chatText);
        }
    }
}
