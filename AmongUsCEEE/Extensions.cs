using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmongUsCEEE
{
    public static class Extensions
    {
        public static CustomRole GetRole(this RoleBehaviour me)
        {
            return ModLoader.CurrentGamemode.Roles[(int)me.Role];
        }

        public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this List<T> list)
        {
            var casted = new Il2CppSystem.Collections.Generic.List<T>();

            foreach (var element in list)
            {
                casted.Add(element);
            }

            return casted;
        }
    }
}
