using Bannerlord.SaveSystem.HarmonyPatch;

using HarmonyLib;

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

using BSSL = Bannerlord.SaveSystem.Load;
using TSS = TaleWorlds.SaveSystem;
using TSSL = TaleWorlds.SaveSystem.Load;

namespace Bannerlord.SaveSystem.Patches
{
    public static class SaveManagerPatch
    {
        public static HarmonyPatchEntry InitializeObjects_CallExternalCallback { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(typeof(TSS.SaveManager), nameof(TSS.SaveManager.Load), new[] { typeof(TSS.ISaveDriver), typeof(bool) }),
            new HarmonyMethod(typeof(SaveManagerPatch), nameof(LoadTranspiler)),
            HarmonyPatchType.Transpiler);


        private static IEnumerable<CodeInstruction> LoadTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            var originalConstructor = typeof(TSSL.LoadContext).GetMember(".ctor", AccessTools.all).First();
            var replacementConstructor = typeof(BSSL.LoadContext).GetMember(".ctor", AccessTools.all).First();

            var originalLoad = typeof(TSSL.LoadContext).GetMember("Load", AccessTools.all).First();
            var replacementLoad = typeof(BSSL.LoadContext).GetMember("Load", AccessTools.all).First();
            
            var originalGetRootObject = typeof(TSSL.LoadContext).GetMember("get_RootObject", AccessTools.all).First();
            var replacementGetRootObject = typeof(BSSL.LoadContext).GetMember("get_RootObject", AccessTools.all).First();

            var originalCreateLoadCallbackInitializator = typeof(TSSL.LoadContext).GetMember("CreateLoadCallbackInitializator", AccessTools.all).First();
            var replacementCreateLoadCallbackInitializator = typeof(BSSL.LoadContext).GetMember("CreateLoadCallbackInitializator", AccessTools.all).First();

            foreach (var instruction in instructions)
            {
                if (ReferenceEquals(instruction.operand, originalConstructor))
                    instruction.operand = replacementConstructor;

                if (ReferenceEquals(instruction.operand, originalLoad))
                    instruction.operand = replacementLoad;

                if (ReferenceEquals(instruction.operand, originalGetRootObject))
                    instruction.operand = replacementGetRootObject;

                if (ReferenceEquals(instruction.operand, originalCreateLoadCallbackInitializator))
                    instruction.operand = replacementCreateLoadCallbackInitializator;

                yield return instruction;
            }
        }
    }
}