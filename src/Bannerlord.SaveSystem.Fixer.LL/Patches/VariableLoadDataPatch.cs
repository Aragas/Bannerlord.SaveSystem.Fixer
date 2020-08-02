using Bannerlord.SaveSystem.Definitions;
using Bannerlord.SaveSystem.HarmonyPatch;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.Patches
{
    public static class VariableLoadDataPatch
    {
        public static HarmonyPatchEntry GetDataToUse_NullTypeDefinitionAndEnumNullFix { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "GetDataToUse"),
            new HarmonyMethod(typeof(VariableLoadDataPatch), nameof(GetDataToUsePrefix)),
            HarmonyPatchType.Prefix);
        public static HarmonyPatchEntry GetDataToUse_NullTypeDefinitionAndEnumNullFix_Transpiler { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "GetDataToUse"),
            new HarmonyMethod(typeof(VariableLoadDataPatch), nameof(GetDataToUseTranspiler)),
            HarmonyPatchType.Transpiler);

        public static HarmonyPatchEntry Read_NullTypeDefinitionSkip { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "Read"),
            new HarmonyMethod(typeof(VariableLoadDataPatch), nameof(ReadTranspiler)),
            HarmonyPatchType.Transpiler);


        private static MethodInfo SavedMemberTypeProperty => AccessTools.DeclaredPropertyGetter(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "SavedMemberType");
        private static bool GetDataToUsePrefix(object __instance, TSSD.TypeDefinitionBase ____typeDefinition, ref object? __result)
        {
            var savedMemberType = (int) SavedMemberTypeProperty.Invoke(__instance, Array.Empty<object>());
            if (savedMemberType == 5 && (____typeDefinition == null || ____typeDefinition.Type == typeof(object)))
            {
                __result = null;
                return true;
            }

            /*
            if (____typeDefinition is NullContainerDefinition)
            {
                ;
            }
            */

            if (____typeDefinition is NullTypeDefinition)
            {
                /*
                if (____typeDefinition.Type != typeof(object))
                    ;
                */

                __result = null;
                return true;
            }

            return false;
        }
        public static IEnumerable<CodeInstruction> GetDataToUseTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            var list = instructions.ToList();

            var jmpOriginalFlow = ilGenerator.DefineLabel();
            list[0].labels.Add(jmpOriginalFlow);

            // if (SavedMemberType == SavedMemberType.Enum && (_typeDefinition == null || _typeDefinition.Type == typeof(object))) return null;
            var index = 0;
            var jmpReturnNull = ilGenerator.DefineLabel();
            var jmpNextExpression = ilGenerator.DefineLabel();
            list.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.DeclaredPropertyGetter(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "SavedMemberType")),
                new CodeInstruction(OpCodes.Ldc_I4_5),
                new CodeInstruction(OpCodes.Bne_Un_S, jmpNextExpression),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.DeclaredField(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "_typeDefinition")),
                new CodeInstruction(OpCodes.Brfalse_S, jmpReturnNull),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.DeclaredField(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "_typeDefinition")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.DeclaredPropertyGetter(typeof(TSSD.TypeDefinitionBase), "Type")),
                new CodeInstruction(OpCodes.Ldtoken, typeof(object)),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Type), nameof(Type.GetTypeFromHandle))),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Type), "op_Equality")),
                new CodeInstruction(OpCodes.Brfalse_S, jmpNextExpression),

                new CodeInstruction(OpCodes.Ldnull) { labels = { jmpReturnNull } },
                new CodeInstruction(OpCodes.Ret)
            });
            //

            // if (_typeDefinition is NullTypeDefinition) return null;
            var index2 = 16; // Right after previous expression
            list.InsertRange(index2, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0) { labels = { jmpNextExpression } }, // Catch the original flow return
                new CodeInstruction(OpCodes.Ldfld, AccessTools.DeclaredField(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "_typeDefinition")),
                new CodeInstruction(OpCodes.Isinst, typeof(NullTypeDefinition)),
                new CodeInstruction(OpCodes.Brfalse_S, jmpOriginalFlow),

                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Ret)
            });
            //

            return list;
        }

        private static IEnumerable<CodeInstruction> ReadTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            var list = instructions.ToList();

            // if (_typeDefinition is NullTypeDefinition) return;
            var index = 92; // TODO: Remove hardcoded indexes 
            var jmpOriginalFlow = ilGenerator.DefineLabel();
            list[index].labels.Add(jmpOriginalFlow);
            list.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.DeclaredField(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "_typeDefinition")),
                new CodeInstruction(OpCodes.Isinst, typeof(NullTypeDefinition)),
                new CodeInstruction(OpCodes.Brfalse_S, jmpOriginalFlow),

                new CodeInstruction(OpCodes.Ret)
            });
            //

            return list;
        }
    }
}