using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AmongUsCEEE
{
    public class Team
    {
        public string ID = "undefinedteam";
        public string DisplayName = "UndefinedTeam";
        public Color32 Color = new Color32(255,255,255,255);
        public byte Layer = 255;
        public string TeamBlurb = "HEY YOU DEFINE A TEAM BLURHB!!!!";

        public bool CanSee(Team t)
        {
            if (Layer == 0) return false;
            if (Layer == 255) return true;
            return t.Layer == Layer;
        }

    }

    [Flags]
    public enum RoleSpecials
    {
        None = 0,
        Kill = 1,
        Sabotage = 2,
        Vent = 4,
        Report = 8,
        Ability = 16
    }

    public class CustomRole : Il2CppSystem.Object
    {
        public CustomRole() : base(ClassInjector.DerivedConstructorPointer<CustomRole>())
        {
            ClassInjector.DerivedConstructorBody(this);
        }

        public bool CanDo(RoleSpecials spec)
        {
            return Specials.HasFlag(spec);
        }

        public string ID = "undefined";
        public string SmallBlurb = "Define a blurb.";
        public string DisplayName = "Undefined";
        public string TaskText = "";
        public Color32 Color = new Color32(255, 255, 255, 255);
        public bool DoesTasks = true;
        public RoleSpecials Specials = RoleSpecials.None;
        public string TeamID => (Team != null) ? Team.ID : "undefined";

        public Team? Team;

        public Gamemode? ParentGamemode;

    }
}
