using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmongUsCEEE.Scripting;
using MoonSharp;
using MoonSharp.Interpreter;
using Reactor.Networking;
using UnityEngine;

namespace AmongUsCEEE
{
    public static class StandardizedGlobals
    {
        public static bool AddRole(string gamemode, IReturnValue v)
        {
            if (ModLoader.CurrentLoadingMod == null) return false;
            Dictionary<string,object> Values = v.GetDictionary();
            if (Values.Keys.Count == 0) return false; //empty dictionary
#pragma warning disable CS8601 // Possible null reference assignment.
            if (!(Values.TryGetValue("id", out object? id) 
                && Values.TryGetValue("name", out object? name) 
                && Values.TryGetValue("color", out object? color)
                && Values.TryGetValue("team", out object? team)
                && Values.TryGetValue("smallblurb", out object? tinyblurb)
                && Values.TryGetValue("doestasks", out object? doestasks)
                && Values.TryGetValue("abilities", out object? abilities))) return false; //make sure all values are valid
            CustomRole CR = new CustomRole();
            CR.DisplayName = (string)name;
            CR.ID = (string)id;
            if (color is ColorData)
            {
                CR.Color = (Color32)(ColorData)color;
            }
            else
            {
                return false;
            }
            CR.Specials = ((List<object>)abilities).GetFlags<RoleSpecials>();
            UnityEngine.Debug.Log(CR.Specials.ToString());
            CR.DoesTasks = (bool)doestasks;
            CR.SmallBlurb = tinyblurb.ToString();
            if (Values.TryGetValue("tasktext", out object? tasktext)) //this isn't mandatory
            {
                CR.TaskText = (string)tasktext;
            }
#pragma warning restore CS8601 // Possible null reference assignment.
            Gamemode? gameParent = ModLoader.CurrentLoadingMod.GetGamemodeWithID(gamemode);
            if (gameParent == null) return false; //if the gamemode doesn't exist (usually it means this function was called outside of gamemode initialization) return false
            CR.ParentGamemode = gameParent;
            CR.Team = gameParent.Teams.Find(f => f.ID == (string)team);
            gameParent.Roles.Add(CR);
            UnityEngine.Debug.Log("Succesfully added role:" + CR.ID);

            return true;
        }

        public static bool AddTeam(string gamemode, IReturnValue v)
        {
            if (ModLoader.CurrentLoadingMod == null) return false;
            Dictionary<string, object> Values = v.GetDictionary();
            if (Values.Keys.Count == 0) return false; //empty dictionary
#pragma warning disable CS8601 // Possible null reference assignment.
            if (!(Values.TryGetValue("id", out object? id) 
                && Values.TryGetValue("name", out object? name) 
                && Values.TryGetValue("color", out object? color)
                && Values.TryGetValue("layer", out object? layer)
                && Values.TryGetValue("teamblurb", out object? teamblurb))) return false; //make sure all values are valid
            Team CT = new Team();
            CT.DisplayName = name.ToString();
            CT.ID = id.ToString();
            CT.TeamBlurb = teamblurb.ToString();
            if (color is ColorData)
            {
                CT.Color = (Color32)(ColorData)color;
            }
            else
            {
                return false;
            }
#pragma warning restore CS8601 // Possible null reference assignment.
            CT.Layer = (byte)(double)layer; //convert to double since thats the actual class, then convert to byte
            Gamemode? gameParent = ModLoader.CurrentLoadingMod.GetGamemodeWithID(gamemode);
            if (gameParent == null) return false; //if the gamemode doesn't exist (usually it means this function was called outside of gamemode initialization) return false
            gameParent.Teams.Add(CT);
            return true;
        }

        public static void AddGamemode(string id, string name, string path)
        {
            if (ModLoader.CurrentLoadingMod == null) return;
            foreach (IScriptingLanguage scrL in ModLoader.Languages)
            {
                if (path.EndsWith(scrL.Extension))
                {
                    Gamemode GM = new Gamemode(id, name, null);
                    ModLoader.CurrentLoadingMod.Gamemodes.Add(GM);
                    ModLoader.CurrentLoadingGamemode = id;
                    IScript script = scrL.LoadScript(path,ModLoader.CurrentPath);
                    GM.Script = script;
                    return;
                }
            }
        }

        public static ColorData CreateColor(byte r, byte g, byte b, byte a = byte.MaxValue)
        {
            return new ColorData(r,g,b,a);
        }
    }
}