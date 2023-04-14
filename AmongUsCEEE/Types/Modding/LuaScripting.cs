using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp;
using MoonSharp.Interpreter;

namespace AmongUsCEEE.Scripting
{
    public class LuaReturn : IReturnValue
    {

        public LuaReturn(DynValue v) 
        {
            Value = v;
        }

        public DynValue Value { private set; get; }

        private object getDynValue(DynValue v)
        {
            switch (v.Type)
            {
                case DataType.Boolean:
                    return v.Boolean;
                case DataType.String:
                    return v.String;
                case DataType.Number:
                    return v.Number;
                case DataType.Table:
                    Table tab = v.Table;
                    if (tab.Length == 0)
                    {
                        Dictionary<string, object> tableList = new Dictionary<string, object>();
                        foreach (DynValue key in tab.Keys)
                        {
                            if (key.Type != DataType.String) continue;
                            tableList.Add(key.String, getDynValue(tab.Get(key)));
                        }
                        return tableList;
                    }
                    else
                    {
                        List<object> tableList = new List<object>();
                        foreach (DynValue key in tab.Keys)
                        {
                            if (key.Type != DataType.Number) continue;
                            tableList.Add(getDynValue(tab.Get(key)));
                        }
                        return tableList;
                    }
                case DataType.Function:
                    return new LuaFunction(Value.Function);
                case DataType.Nil:
                    return "NIL";
                case DataType.UserData:
                    return v.UserData.Object;
            }
            return "NIL/UNIMPLEMENTED";
        }

        public List<object> GetList()
        {
            if (Value.Type != DataType.Table) return new List<object>();
            Table tab = Value.Table;
            if (tab.Length == 0) return new List<object>();
            List<object> tableList = new List<object>();
            foreach (DynValue key in tab.Keys)
            {
                if (key.Type != DataType.Number) continue;
                tableList.Add(getDynValue(tab.Get(key)));
            }
            return tableList;
        }
        public bool GetBoolean()
        {
            if (Value.Type != DataType.Boolean) return false;
            return Value.Boolean;
        }
        public Dictionary<string, object> GetDictionary()
        {
            if (Value.Type != DataType.Table) return new Dictionary<string, object>();
            Table tab = Value.Table;
            Dictionary<string, object> tableList = new Dictionary<string, object>();
            foreach (DynValue key in tab.Keys)
            {
                if (key.Type != DataType.String) continue;
                tableList.Add(key.String, getDynValue(tab.Get(key)));
            }
            return tableList;
        }
        public double GetDouble()
        {
            if (Value.Type != DataType.Number) return 0;
            return Value.Number;
        }
        public float GetFloat()
        {
            if (Value.Type != DataType.Number) return 0;
            return (float)Value.Number;
        }
        public int GetInt()
        {
            if (Value.Type != DataType.Number) return 0;
            return (int)Value.Number;
        }
        public object GetRaw()
        {
            return getDynValue(Value);
        }
        public string GetString()
        {
            if (Value.Type != DataType.String) return "INVALID";
            return Value.String;
        }
        public IFunction GetFunction()
        {
            if (Value.Type != DataType.Function) return new EmptyFunction();
            return new LuaFunction(Value.Function);
        }
        public object? GetClass(string name)
        {
            if (Value.Type != DataType.UserData) return null;
            UserData uD = Value.UserData;
            if (uD.Descriptor.Name != name) return null;
            return uD.Object;
        }
    }

    public class LuaFunction : IFunction
    {
        public Closure Clos;
        public LuaFunction(Closure c)
        {
            Clos = c;
        }
        public IReturnValue? Call(params object[] parms)
        {
            DynValue v = Clos.Call(parms);
            if (v.Type == DataType.Nil) return null;
            return new LuaReturn(v);
        }
    }

    public class LuaScript : IScript
    {
        public Script Script;
        public LuaScript(string scr, string internalPath)
        {
            Script script = new Script();
            Script = script;
            SetGlobals();
            SetGlobal("CE_GetPath", delegate ()
            {
                return internalPath;
            });
            this.RegisterEnum(new RoleSpecials());
            script.DoString(scr);
        }
        public IReturnValue? CallFunction(string functionName, params object[] parms)
        {
            return new LuaReturn(Script.Call(Script.Globals[functionName], parms));
        }

        public void SetGlobal(string globalName, object value)
        {
            Script.Globals[globalName] = value;
        }

        public void SetGlobals()
        {
            foreach (KeyValuePair<string, object> kvp in ModLoader.Globals["Lua"])
            {
                SetGlobal(kvp.Key, kvp.Value);
            }
        }
    }

    public class LuaLangage : IScriptingLanguage
    {
        public string ID => "Lua";

        public string Extension => ".lua";

        public static DynValue AddOrGetGlobalTable(string name)
        {
            if (ModLoader.Globals["Lua"].TryGetValue(name, out object val))
            {
                if (val.GetType() == typeof(DynValue))
                {
                    return (DynValue)val;
                }
            }
            DynValue vt = DynValue.NewPrimeTable();
            ModLoader.Globals["Lua"].Add(name, vt);
            return vt;
        }
        public static DynValue AddOrGetGlobal(string name, DynValue v = null)
        {
            if (ModLoader.Globals["Lua"].TryGetValue(name, out object val))
            {
                if (val.GetType() == typeof(DynValue))
                {
                    return (DynValue)val;
                }
            }
            ModLoader.Globals["Lua"].Add(name, v);
            return v;
        }

        public LuaLangage()
        {
            Script.DefaultOptions.DebugPrint = delegate(string str)
            {
                UnityEngine.Debug.Log(str);
            };
        }

        public Dictionary<string, object> GetGlobals()
        {
            Dictionary<string, object> globs = new Dictionary<string, object>();

            globs.Add("CE_SetOrGetUserGlobal", (Func<string,DynValue,DynValue>)AddOrGetGlobal);

            globs.Add("CE_SetOrGetUserGlobalTable", (Func<string, DynValue>)AddOrGetGlobalTable);

            globs.Add("CE_CreateColor", (Func<byte, byte, byte, byte, ColorData>)StandardizedGlobals.CreateColor);

            globs.Add("CE_Register", delegate(string name, Closure c)
            {
                RegisterHandler.AddFunction(name,new LuaFunction(c));
            });

            globs.Add("CE_GetCurrentLoadingGamemode", delegate()
            {
                return ModLoader.CurrentLoadingGamemode;
            });

            globs.Add("CE_AddGamemode", delegate(string id, string name, string path)
            {
                StandardizedGlobals.AddGamemode(id, name, path);
            });

            globs.Add("CE_AddRole", delegate (DynValue v)
            {
                return StandardizedGlobals.AddRole(ModLoader.CurrentLoadingGamemode, new LuaReturn(v));
            });

            globs.Add("CE_AddTeam", delegate (DynValue v)
            {
                return StandardizedGlobals.AddTeam(ModLoader.CurrentLoadingGamemode, new LuaReturn(v));
            });

            return globs;
        }

        public List<IScript> GetScriptsInFolder(string path, string rootP)
        {
            List<IScript> Scripts = new List<IScript>();
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                if (file.ToLower().EndsWith(".lua"))
                {
                    Scripts.Add(LoadScript(file, rootP));
                }
            }
            return Scripts;
        }

        public IScript LoadScript(string path, string rootP)
        {
            string data = File.ReadAllText(path);
            return new LuaScript(data, rootP);
        }
    }

}
