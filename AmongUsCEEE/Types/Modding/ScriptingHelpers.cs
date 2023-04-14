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
    public static class ScriptingHelpers
    {
        public static void RegisterEnum(this IScript script, Enum num)
        {
            string[] Names = Enum.GetNames(num.GetType());
            Array Values = Enum.GetValues(num.GetType());
            for (int i = 0; i < Names.Length; i++)
            {
                script.SetGlobal("Enum_" + num.GetType().Name + "_" + Names[i], Values.GetValue(i));
            }
        }

        public static T GetFlags<T>(this List<object> flags) where T : Enum
        {
            byte currentFlags = 0;
            foreach (object i in flags)
            {
                int flag = Convert.ToInt32(i);
                currentFlags |= (byte)flag;

            }
            return (T)Enum.ToObject(typeof(T), currentFlags);
        }

        public static T GetFlags<T>(this IReturnValue v) where T : Enum
        {
            List<object> flags = v.GetList();
            if (flags.Count == 0) return (T)Enum.ToObject(typeof(T), v.GetInt()); //if no flags, assume they added the values directly as a number
            return GetFlags<T>(flags);
        }
    }
}