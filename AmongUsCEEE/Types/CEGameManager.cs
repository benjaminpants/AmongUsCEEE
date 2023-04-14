using Reactor.Utilities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsCEEE
{
    [RegisterInIl2Cpp]
    public class CEGameManager : GameManager
    {
        public override void InitComponents()
        {
            LogicFlow = new LogicGameFlowNormal(this);
            LogicMinigame = new LogicMinigame(this);
            LogicRoleSelection = new LogicRoleSelectionNormal(this);
            LogicUsables = new LogicUsablesBasic(this);
            LogicOptions = new LogicOptionsNormal(this);
            LogicComponents.Add(LogicFlow);
            LogicComponents.Add(LogicMinigame);
            LogicComponents.Add(LogicRoleSelection);
            LogicComponents.Add(LogicUsables);
            LogicComponents.Add(LogicOptions);
        }

        public CEGameManager(IntPtr ptr) : base(ptr)
        {

        }

        public override MapOptions GetMapOptions()
        {
            /*return new MapOptions()
            {
                Mode = MapOptions.Modes.CountOverlay,
                AllowMovementWhileMapOpen = true
            };*/
            return new MapOptions
            {
                Mode = ((PlayerControl.LocalPlayer.Data.Role.IsImpostor && !MeetingHud.Instance) ? MapOptions.Modes.Sabotage : MapOptions.Modes.Normal)
            };
        }

        public override bool IsNormal()
        {
            return true;
        }

        public override PlayerBodyTypes GetBodyType(PlayerControl player)
        {
            if (Constants.ShouldHorseAround())
            {
                return PlayerBodyTypes.Horse;
            }
            return PlayerBodyTypes.Normal;
        }
    }

}
