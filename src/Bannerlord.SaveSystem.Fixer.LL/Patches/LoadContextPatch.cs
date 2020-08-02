using Bannerlord.SaveSystem.Definitions;
using Bannerlord.SaveSystem.HarmonyPatch;

using HarmonyLib;

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using TSSL = TaleWorlds.SaveSystem.Load;

namespace Bannerlord.SaveSystem.Patches
{
    public static class LoadContextPatch
    {
        public static HarmonyPatchEntry GetObjectWithId_NullTypeDefinitionToNull { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(typeof(TSSL.LoadContext), "GetObjectWithId"),
            new HarmonyMethod(typeof(LoadContextPatch), nameof(GetObjectWithIdPostfix)),
            HarmonyPatchType.Postfix);
        public static HarmonyPatchEntry GetObjectWithId_NullTypeDefinitionToNull_Transpiler { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(typeof(TSSL.LoadContext), "GetObjectWithId"),
            new HarmonyMethod(typeof(LoadContextPatch), nameof(GetObjectWithIdTranspiler)),
            HarmonyPatchType.Transpiler);

        private static void GetObjectWithIdPostfix(TSSL.ObjectHeaderLoadData __result)
        {
            if (__result?.TypeDefinition is NullTypeDefinition)
                __result = null!;
        }
        private static IEnumerable<CodeInstruction> GetObjectWithIdTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            var list = instructions.ToList();

            // if (result?.TypeDefinition is NullTypeDefinition) return null;
            var jmpStart = ilGenerator.DefineLabel();
            var jmpNotNull = ilGenerator.DefineLabel();
            var jmpIsInst = ilGenerator.DefineLabel();
            var jmpOriginalFlow = ilGenerator.DefineLabel();
            list[4].operand = jmpStart; // TODO: Remove hardcoded indexes 
            list.InsertRange(list.Count - 2, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0) { labels = { jmpStart } },
                new CodeInstruction(OpCodes.Brtrue_S, jmpNotNull),

                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Br_S, jmpIsInst),

                new CodeInstruction(OpCodes.Ldloc_0) { labels = { jmpNotNull } }, 
                new CodeInstruction(OpCodes.Call, AccessTools.DeclaredPropertyGetter(typeof(TSSL.ObjectHeaderLoadData), "TypeDefinition")),
                new CodeInstruction(OpCodes.Isinst, typeof(NullTypeDefinition)) { labels = { jmpIsInst } }, // Basic
                new CodeInstruction(OpCodes.Brfalse_S, jmpOriginalFlow),

                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Ret),
            });
            list[^2].labels.Add(jmpOriginalFlow);
            //

            return list;
        }
    }
}