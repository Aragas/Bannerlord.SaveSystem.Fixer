using Bannerlord.SaveSystem.HarmonyPatch;
using Bannerlord.SaveSystem.Load;

using HarmonyLib;

using System.Collections.Generic;
using System.Reflection.Emit;

namespace Bannerlord.SaveSystem.Patches
{
    public static class LoadResultPatch
    {
        public static HarmonyPatchEntry InitializeObjects_CallExternalCallback { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(typeof(TaleWorlds.SaveSystem.Load.LoadResult), nameof(TaleWorlds.SaveSystem.Load.LoadResult.InitializeObjects)),
            new HarmonyMethod(typeof(LoadResultPatch), nameof(InitializeObjectsPrefix)),
            HarmonyPatchType.Prefix);

        public static HarmonyPatchEntry InitializeObjects_CallExternalCallback_Transpiler { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(typeof(TaleWorlds.SaveSystem.Load.LoadResult), nameof(TaleWorlds.SaveSystem.Load.LoadResult.InitializeObjects)),
            new HarmonyMethod(typeof(LoadResultPatch), nameof(InitializeObjectsTranspiler)),
            HarmonyPatchType.Transpiler);


        private static bool InitializeObjectsPrefix()
        {
            LoadContext.CallbackInitializator.InitializeObjects();
            return true;
        }
        private static IEnumerable<CodeInstruction> InitializeObjectsTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LoadResultPatch), nameof(LoadResultPatch.InitializeObjectsPrefix)));
            yield return new CodeInstruction(OpCodes.Pop);
            yield return new CodeInstruction(OpCodes.Ret);
        }
    }
}