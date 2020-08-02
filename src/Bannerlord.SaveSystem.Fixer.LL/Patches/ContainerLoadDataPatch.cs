using Bannerlord.SaveSystem.HarmonyPatch;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Bannerlord.SaveSystem.Patches
{
    /// <summary>
    /// Shouldn't be required if the fix is done right IMO
    /// </summary>
    public static class ContainerLoadDataPatch
    {
        public static HarmonyPatchEntry FillObject_CheckForNull { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(Type.GetType("TaleWorlds.SaveSystem.Load.ContainerLoadData, TaleWorlds.SaveSystem"), "FillObject"),
            new HarmonyMethod(typeof(ContainerLoadDataPatch), nameof(FillObjectTranspiler)),
            HarmonyPatchType.Transpiler);

        public static HarmonyPatchEntry FillObject_Debug { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(Type.GetType("TaleWorlds.SaveSystem.Load.ContainerLoadData, TaleWorlds.SaveSystem"), "FillObject"),
            new HarmonyMethod(typeof(ContainerLoadDataPatch), nameof(FillObjectFinalizer)),
            HarmonyPatchType.Finalizer);


        private static IEnumerable<CodeInstruction> FillObjectTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            var list = instructions.ToList();

            var jmpOriginalFlow = ilGenerator.DefineLabel();
            list[209].labels.Add(jmpOriginalFlow);

            
            // if (dataToUse != null)
            var index1 = 196; // TODO: Remove hardcoded indexes 
            var dataToUseQueueValue = list[index1 - 1].operand;
            list.InsertRange(index1, new[] // TODO: Remove hardcoded indexes 
            {
                new CodeInstruction(OpCodes.Ldloc_S, dataToUseQueueValue),
                new CodeInstruction(OpCodes.Brfalse_S, jmpOriginalFlow)
            });
            //

            /*
            // if (dataToUse != null)
            var index2 = 159; // TODO: Remove hardcoded indexes 
            var dataToUseArrayValue = list[index2 - 1].operand;
            list.InsertRange(159, new[] // TODO: Remove hardcoded indexes 
            {
                new CodeInstruction(OpCodes.Ldloc_S, dataToUseArrayValue),
                new CodeInstruction(OpCodes.Brfalse_S, jmpOriginalFlow)
            });
            //
            */

            // && dataToUse != null
            var index3 = 122; // TODO: Remove hardcoded indexes 
            var dataToUseDictionaryValue = list[index3 - 5].operand;
            list.InsertRange(index3, new[] // TODO: Remove hardcoded indexes 
            {
                new CodeInstruction(OpCodes.Ldloc_S, dataToUseDictionaryValue),
                new CodeInstruction(OpCodes.Brfalse_S, jmpOriginalFlow)
            });
            //

            // && dataToUse != null
            var index4 = 55; // TODO: Remove hardcoded indexes 
            var dataToUseListValue = list[index4 - 3].operand;
            list.InsertRange(index4, new[] // TODO: Remove hardcoded indexes 
            {
                new CodeInstruction(OpCodes.Ldloc_S, dataToUseListValue),
                new CodeInstruction(OpCodes.Brfalse_S, jmpOriginalFlow)
            });
            //

            return list;
        }
        private static void FillObjectFinalizer(Exception __exception, object __instance, int ____containerType, Array ____values)
        {
            var prop = AccessTools.DeclaredPropertyGetter(__instance.GetType(), "Target");
            var method = AccessTools.Method(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "GetDataToUse");

            switch (____containerType)
            {
                case 1:
                {
                    var target = (System.Collections.IList) prop.Invoke(__instance, Array.Empty<object>());
                    if (target == null)
                        ;
                }
                    break;
                case 2:
                {
                    var target = (System.Collections.IDictionary) prop.Invoke(__instance, Array.Empty<object>());
                    if (target == null)
                        ;
                }
                    break;
                case 3:
                {
                    var target = (Array) prop.Invoke(__instance, Array.Empty<object>());
                    if (target == null)
                        ;
                }
                    break;
                case 4:
                {
                    var target = (System.Collections.ICollection) prop.Invoke(__instance, Array.Empty<object>());
                    if (target == null)
                        ;
                }
                    break;
            }

            if (__exception != null)
            {
                var target = (System.Collections.IList) prop.Invoke(__instance, Array.Empty<object>());
                var list = new List<object>();
                foreach (var value in ____values)
                {
                    list.Add(method.Invoke(value, Array.Empty<object>()));
                }

                ;

                foreach (object o in list)
                {
                    target.Add(o);
                }
                ;
            }

            ;
        }
    }
}