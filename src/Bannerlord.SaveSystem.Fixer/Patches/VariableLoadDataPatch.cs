using Bannerlord.SaveSystem.Definitions;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.Patches
{
    public static class VariableLoadDataPatch
    {
        public static MethodBase GetDataToUseMethod => AccessTools.DeclaredMethod(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "GetDataToUse");
        public static PropertyInfo SavedMemberTypeProperty => AccessTools.DeclaredProperty(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "SavedMemberType");
        public static bool GetDataToUsePrefix(object __instance, TypeDefinitionBase ____typeDefinition, ref object __result)
        {
            var savedMemberType = (int) SavedMemberTypeProperty.GetValue(__instance);
            if (savedMemberType == 5 && (____typeDefinition == null || ____typeDefinition.Type == typeof(object)))
            {
                __result = null;
                return true;
            }

            if (____typeDefinition is NullTypeDefinition)
            {
                __result = null;
                return true;
            }
            return false;
        }
        public static IEnumerable<CodeInstruction> GetDataToUseTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            var list = instructions.ToList();

            var firstReturnNull = ilGenerator.DefineLabel();
            var secondExpressionStart = ilGenerator.DefineLabel();
            var originalEntry = ilGenerator.DefineLabel();
            list[0].labels.Add(originalEntry);

            // if (SavedMemberType == SavedMemberType.Enum && (_typeDefinition == null || _typeDefinition.Type == typeof(object))) return null;
            list.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.DeclaredPropertyGetter(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "SavedMemberType")),
                new CodeInstruction(OpCodes.Ldc_I4_5),
                new CodeInstruction(OpCodes.Bne_Un_S, secondExpressionStart),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.DeclaredField(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "_typeDefinition")),
                new CodeInstruction(OpCodes.Brfalse_S, firstReturnNull),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.DeclaredField(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "_typeDefinition")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.DeclaredPropertyGetter(typeof(TypeDefinitionBase), "Type")),
                new CodeInstruction(OpCodes.Ldtoken, typeof(object)),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Type), nameof(Type.GetTypeFromHandle))),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Type), "op_Equality")),
                new CodeInstruction(OpCodes.Brfalse_S, secondExpressionStart),

                new CodeInstruction(OpCodes.Ldnull) { labels = { firstReturnNull } },
                new CodeInstruction(OpCodes.Ret),
            });
            //

            // if (_typeDefinition is NullTypeDefinition) return null;
            list.InsertRange(16, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0) { labels = { secondExpressionStart } },
                new CodeInstruction(OpCodes.Ldfld, AccessTools.DeclaredField(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "_typeDefinition")),
                new CodeInstruction(OpCodes.Isinst, typeof(NullTypeDefinition)), // Basic
                new CodeInstruction(OpCodes.Brfalse_S, originalEntry),

                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Ret)
            });
            //

            return list;
        }
        
        public static MethodBase ReadMethod => AccessTools.DeclaredMethod(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "Read");
        public static IEnumerable<CodeInstruction> ReadTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            var list = instructions.ToList();

            // if (_typeDefinition is NullTypeDefinition) return;
            var label = ilGenerator.DefineLabel();
            list.InsertRange(92, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.DeclaredField(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "_typeDefinition")),
                new CodeInstruction(OpCodes.Isinst, typeof(NullTypeDefinition)), // Basic
                new CodeInstruction(OpCodes.Brfalse_S, label),

                new CodeInstruction(OpCodes.Ret)
            });
            list[97].labels.Add(label);
            //

            return list;
        }
    }
}