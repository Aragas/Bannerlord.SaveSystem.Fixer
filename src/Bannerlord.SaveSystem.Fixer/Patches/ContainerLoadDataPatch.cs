using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Bannerlord.SaveSystem.Patches
{
    /// <summary>
    /// Shouldn't be required if the fix is done right IMO
    /// </summary>
    public static class ContainerLoadDataPatch
    {
        public static MethodBase FillObjectMethod => AccessTools.Method(Type.GetType("TaleWorlds.SaveSystem.Load.ContainerLoadData, TaleWorlds.SaveSystem"), "FillObject");
        public static IEnumerable<CodeInstruction> FillObjectTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var list = instructions.ToList();

            // && dataToUse != null
            list.InsertRange(55, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, list[52].operand),
                new CodeInstruction(OpCodes.Brfalse_S, list[54].operand)
            });
            //

            return list;
        }
    }
}