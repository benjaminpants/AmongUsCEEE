using AmongUsCEEE.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AmongUsCEEE
{

    public enum ReturnHandler
    {
        Ignore, //ignore return values, always return null
        IfNotNull, //if the return value is not null, return
        IfTrue, //if the return value is true, return
        IfFalse //if the return value is false, return
    }

    public static class RegisterHandler
    {
        public static Dictionary<string,List<IFunction>> Functions = new Dictionary<string,List<IFunction>>();
        public static void AddKey(string key)
        {
            if (!Functions.ContainsKey(key))
            {
                Functions.Add(key, new List<IFunction>());
            }
        }
        public static void AddFunction(string key, IFunction f)
        {
            AddKey(key);
            Functions[key].Insert(0,f); //order by newest added to oldest addest, so functions added by builtin get called last
        }
        public static IReturnValue? Call(string key, ReturnHandler rh, bool defaultToGamemode, params object[] parms)
        {
            AddKey(key);
            if (Functions[key].Count == 0) return null;
            foreach (IFunction f in Functions[key]) 
            {
                IReturnValue? v = f.Call(parms);
                if (v == null) continue;
                switch (rh)
                {
                    case ReturnHandler.Ignore:
                        break;
                    case ReturnHandler.IfNotNull:
                        if (v != null)
                        {
                            return v;
                        }
                        break;
                    case ReturnHandler.IfFalse:
                        if (!v.GetBoolean())
                        {
                            return null;
                        }
                        break;
                    case ReturnHandler.IfTrue:
                        if (v.GetBoolean())
                        {
                            return null;
                        }
                        break;
                    default:
                        throw new NotImplementedException(rh.ToString());
                }
            }
            if (defaultToGamemode)
            {
                return ModLoader.CurrentGamemode.Script.CallFunction(key,parms);
            }
            return null;
        }
    }


    public static class ModLoader
    {
        public static List<IScriptingLanguage> Languages = new List<IScriptingLanguage>() { 
            new LuaLangage()
        };

        public static IScriptingLanguage? GetLanguage(string id)
        {
            return Languages.Find(d => d.ID == id);
        }

        public static Dictionary<string, Dictionary<string,object>> Globals = new Dictionary<string, Dictionary<string, object>>();

        public static List<Mod> Mods { get; private set; } = new List<Mod>();

        public static Mod? CurrentLoadingMod; //the mod that is currently loading

        public static Gamemode CurrentGamemode => Mods[0].Gamemodes[0]; //placeholder

        public static string CurrentLoadingGamemode = "none"; //the gamemode that is currently loading. See: CurrentGamemode for the actual current gamemode

        public static string CurrentPath = "";

        public static Dictionary<string,string> LoadConfig(string path)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            string str = File.ReadAllText(path);
            str = str
                .Replace("\r\n","N!W!P!H")
                .Replace("\r", "N!W!P!H")
                .Replace("\n", "N!W!P!H")
                .Replace("N!W!P!H", "\n");
            string[] splitNL = str.Split('\n');
            foreach (string line in splitNL)
            {
                string[] splitEquals = line.Split("=",StringSplitOptions.TrimEntries);
                if (splitEquals.Length < 2)
                {
                    throw new IndexOutOfRangeException("Uneven split while reading config file!");
                }
                dict.Add(splitEquals[0].ToLower(), splitEquals[1]);
            }
            return dict;
        }

        public static void LoadMod(string directory)
        {
            Mod mod = new Mod();
            Dictionary<string, string> conf = LoadConfig(Path.Combine(directory, "mod.conf"));
            mod.Name = conf["name"];
            mod.Author = conf["author"];
            mod.folderName = Path.GetDirectoryName(directory);
            Debug.Log(mod.ToString());
            CurrentLoadingMod = mod;

            foreach (IScriptingLanguage scrL in Languages)
            {
                string initScript = Path.Combine(directory, "init" + scrL.Extension);
                if (File.Exists(initScript))
                {
                    mod.GlobalScript = scrL.LoadScript(initScript, directory);
                    break;
                }
            }
            /*string globalscrDir = Path.Combine(directory, "globalScripts");
            string gamemodeDir = Path.Combine(directory, "gamemodes");
            if (Directory.Exists(globalscrDir))
            {
                foreach (IScriptingLanguage scrL in Languages)
                {
                    Debug.Log("Scanning with: " + scrL.ID);
                    mod.GlobalScripts.AddRange(scrL.GetScriptsInFolder(globalscrDir));
                }
            }*/
            Mods.Add(mod);
        }

        public static bool LoadMods()
        {
            Mods = new List<Mod>();
            Globals = new Dictionary<string, Dictionary<string, object>>();
            foreach (IScriptingLanguage scrL in Languages)
            {
                Globals.Add(scrL.ID,scrL.GetGlobals());
            }
            string path = Path.Combine(CEExtensions.GetGameDirectory(),"Mods");
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                if (!Path.GetDirectoryName(directory).StartsWith("builtin")) continue;
                if (!File.Exists(Path.Combine(directory, "mod.conf"))) continue;
                CurrentPath = directory;
                LoadMod(directory);
            }
            foreach (string directory in directories) 
            {
                if (Path.GetDirectoryName(directory).StartsWith("builtin")) continue;
                if (!File.Exists(Path.Combine(directory, "mod.conf"))) continue;
                CurrentPath = directory;
                LoadMod(directory);
            }
            CurrentPath = "";
            return true;
        }
    }




    public class Mod
    {
        public override string ToString()
        {
            return folderName + " | " + Name + " | " + Author;
        }
        public Gamemode? GetGamemodeWithID(string id)
        {
            return Gamemodes.Find(d => d.ID == id);
        }
        public string folderName = "invalidfolder";
        public string Name = "undefined";
        public string Author = "unknown author";
        public IScript GlobalScript; //scripts that should be running 100% of the time
        public List<Gamemode> Gamemodes = new List<Gamemode>(); //gamemodes!!
    }

    public class Gamemode
    {
        public string ID = "undefinedgamemode";
        public string Name = "Unnamed Gamemode";

        public List<CustomRole> Roles = new List<CustomRole>();
        public List<Team> Teams = new List<Team>();

        public IScript Script; //marked as nullable here, but this should pretty much never be null outside of loading.

        public Gamemode(string id, string nm, IScript scr) 
        {
            ID = id;
            Name = nm;
            Script = scr;
        }
    }
}
