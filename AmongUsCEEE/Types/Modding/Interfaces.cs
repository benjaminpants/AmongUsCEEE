using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsCEEE.Scripting
{

    public interface IReturnValue
    {
        public string GetString();
        public int GetInt();
        public float GetFloat();
        public double GetDouble();
        public object GetRaw(); //get the raw value(whether it be a float, a double, etc)
        public List<object> GetList();
        public bool GetBoolean();
        public IFunction GetFunction();
        public object? GetClass(string name);
        public Dictionary<string, object> GetDictionary();

    }

    public interface IScript
    {
        public IReturnValue? CallFunction(string functionName, params object[] parms);
        public void SetGlobal(string globalName, object value);
    }

    public interface IFunction
    {
        public IReturnValue? Call(params object[] parms);
    }

    public class EmptyFunction : IFunction
    {
        public IReturnValue? Call(params object[] parms)
        {
            return null;
        }
    }
    public interface IScriptingLanguage
    {
        public List<IScript> GetScriptsInFolder(string path, string rootP);

        public IScript LoadScript(string path, string rootP);

        public Dictionary<string, object> GetGlobals();

        public string ID { get; }
        public string Extension { get; }
    }
}
