using AmongUs.GameOptions;
using AmongUsCEEE.Scripting;
using Reactor.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AmongUsCEEE
{
    [RegisterInIl2Cpp]
    public class RootRole : RoleBehaviour //we cant define the role stuff here directly but we dont have to deal with annoying patching virtual methods
    {
        public override bool CanUse(IUsable console)
        {
            CustomRole role = Player.Data.Role.GetRole();
            Vent? actualvent = console.TryCast<Vent>();
            if (actualvent != null)
            {
                return role.CanDo(RoleSpecials.Vent);
            }
            if (console == null) return false;
            Console? actualconsole = console.TryCast<Console>();
            bool hastasks = role.DoesTasks;
            return (actualconsole != null) && (hastasks || actualconsole.AllowImpostor);
        }

        public override bool DidWin(GameOverReason gameOverReason)
        {
            return false;
        }

        public override bool IsDead => false;

        public override void AppendTaskHint(Il2CppSystem.Text.StringBuilder taskStringBuilder)
        {
            
        }

        public override void Deinitialize(PlayerControl targetPlayer)
        {
            CustomRole role = targetPlayer.Data.Role.GetRole();
            PlayerTask? playerTask = targetPlayer.myTasks.ToArray().ToList().FirstOrDefault((PlayerTask t) => t.name == "RoleText");
            if (playerTask != null)
            {
                targetPlayer.myTasks.Remove(playerTask);
                UnityEngine.Object.Destroy(playerTask.gameObject);
            }
        }

        public override bool IsAffectedByComms
        {
            get
            {
                return base.IsAffectedByComms;
            }
        }

        public override void Initialize(PlayerControl player)
        {
            CustomRole role = player.Data.Role.GetRole();
            this.Player = player;
            if (!player.AmOwner)
            {
                return;
            }
            if (role.CanDo(RoleSpecials.Kill))
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.Show();
                player.SetKillTimer(10f);
            }
            else
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.Hide();
            }
            if (role.CanDo(RoleSpecials.Sabotage))
            {
                DestroyableSingleton<HudManager>.Instance.SabotageButton.Show();
            }
            else
            {
                DestroyableSingleton<HudManager>.Instance.SabotageButton.Hide();
            }
            if (role.CanDo(RoleSpecials.Sabotage))
            {
                DestroyableSingleton<HudManager>.Instance.SabotageButton.Show();
            }
            else
            {
                DestroyableSingleton<HudManager>.Instance.SabotageButton.Hide();
            }
            if (role.CanDo(RoleSpecials.Vent))
            {
                DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.Show();
            }
            else
            {
                DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.Hide();
            }
            if (role.CanDo(RoleSpecials.Report))
            {
                DestroyableSingleton<HudManager>.Instance.ReportButton.Show();
            }
            else
            {
                DestroyableSingleton<HudManager>.Instance.ReportButton.Hide();
            }
            /*if (this.CanUseKillButton)
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.Show();
                DestroyableSingleton<HudManager>.Instance.SabotageButton.Show();
                DestroyableSingleton<HudManager>.Instance.AdminButton.Show();
                if (this.CanVent && GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.HideNSeek)
                {
                    DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.Show();
                }
                player.SetKillTimer(10f);
            }
            else
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.Hide();
                DestroyableSingleton<HudManager>.Instance.SabotageButton.Hide();
                DestroyableSingleton<HudManager>.Instance.AdminButton.Hide();
                DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.Hide();
            }*/
            PlayerNameColor.SetForRoleDirectly(player, this);
            this.InitializeAbilityButton();
        }

        public override bool IsValidTarget(GameData.PlayerInfo target)
        {
            if (!base.IsValidTarget(target)) return false;
            IReturnValue? result = RegisterHandler.Call("CanKill",ReturnHandler.IfNotNull, true, Player.Data.GetInteropData(), target.GetInteropData());
            if (result == null) return true;
            return result.GetBoolean();
        }

        public override void SpawnTaskHeader(PlayerControl playerControl)
        {
            CustomRole role = playerControl.Data.Role.GetRole();
            if (role.TaskText == "") return;
            if (playerControl != PlayerControl.LocalPlayer)
            {
                return;
            }
            ImportantTextTask taskText = PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl, 0);
            taskText.name = "RoleText";
            StringBuilder StB = new StringBuilder();
            StB.Append(((Color)role.Color).ToTextColor());
            StB.Append(role.TaskText);
            if (!role.DoesTasks)
            {
                StB.Append("\r\n");
                StB.Append(DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FakeTasks, Array.Empty<Il2CppSystem.Object>()));
                StB.Append("</color>");
            }
            else
            {
                StB.Append("</color>");
            }
            taskText.Text = StB.ToString();
            return;
        }

        public RootRole(IntPtr ptr) : base(ptr)
        {

        }
    }
}
