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
    /// <summary>
    /// Injects NullTypeDefinition into the game's save system
    /// </summary>
    public static class DefinitionContextPatch
    {
        public static HarmonyPatchEntry TryGetTypeDefinition_AddNullTypeDefinition { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(typeof(TSSD.DefinitionContext), "TryGetTypeDefinition"),
            new HarmonyMethod(typeof(DefinitionContextPatch), nameof(DefinitionContextPatch.TryGetTypeDefinitionTranspiler)),
            HarmonyPatchType.Transpiler);

        public static HarmonyPatchEntry GetTypeDefinition_NullTypeDefinitionWhenNull { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(typeof(TSSD.DefinitionContext), "GetTypeDefinition"),
            new HarmonyMethod(typeof(DefinitionContextPatch), nameof(DefinitionContextPatch.GetTypeDefinitionPostfix)),
            HarmonyPatchType.Postfix);

        public static HarmonyPatchEntry GetClassDefinition_NullTypeDefinitionWhenNull { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(typeof(TSSD.DefinitionContext), "GetClassDefinition"),
            new HarmonyMethod(typeof(DefinitionContextPatch), nameof(DefinitionContextPatch.GetClassDefinitionPostfix)),
            HarmonyPatchType.Postfix);


        private static IEnumerable<CodeInstruction> TryGetTypeDefinitionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            var list = instructions.ToList();

            // return Utils.GetTypeDefinition(saveId);
            var originalInstructionJmps = list[^2].labels;
            list.RemoveAt(list.Count - 2);
            list.InsertRange(list.Count - 1, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1) { labels = originalInstructionJmps },
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(Utils), nameof(Utils.GetTypeDefinition)))
            });
            //

            // if (typeDefinitionBase is NullTypeDefinition) return null;
            var index = 42; // TODO: Remove hardcoded indexes 
            var typeDefinitionBaseLocalVariable = list[42].operand;
            var jmpOriginalFlow = ilGenerator.DefineLabel();
            list[42 + 4].labels.Add(jmpOriginalFlow);
            list.InsertRange(42 + 4, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, typeDefinitionBaseLocalVariable),
                new CodeInstruction(OpCodes.Isinst, typeof(NullTypeDefinition)), // Basic
                new CodeInstruction(OpCodes.Brfalse_S, jmpOriginalFlow),

                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Ret)
            });
            //

            // && !(typeDefinition is NullTypeDefinition)
            var index2 = 23; // TODO: Remove hardcoded indexes 
            var jmpOriginalFlow2 = list[index2].operand;
            list.InsertRange(index2 + 1, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Isinst, typeof(NullTypeDefinition)),
                new CodeInstruction(OpCodes.Brtrue, jmpOriginalFlow2)
            });
            //

            return list;
        }

        private static void GetTypeDefinitionPostfix(Type type, Dictionary<Type, TSSD.TypeDefinitionBase> ____allTypeDefinitions, ref TSSD.TypeDefinitionBase __result)
        {
            if (__result == null)
                __result = new NullTypeDefinition(0);
                //__result = new TypeDefinition(type, 0, new DefaultObjectResolver());
        }

        public static MethodInfo IsContainerMethod { get; } = AccessTools.DeclaredMethod(
            Type.GetType("TaleWorlds.SaveSystem.TypeExtensions, TaleWorlds.SaveSystem"),
            "IsContainer",
            new [] { typeof(Type) });
        private static void GetClassDefinitionPostfix(Type type, ref TSSD.TypeDefinition __result)
        {
            if (__result != null)
                return;

            var isContainer = (bool) IsContainerMethod.Invoke(null, new object[] { type });
         
            if (!isContainer)
                __result = new NullTypeDefinition(0);

            /*
            if (isContainer)
            {
                var containerType = ContainerType.None;
                if (type.IsGenericType && !type.IsGenericTypeDefinition)
                {
                    var genericTypeDefinition = type.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(Dictionary<, >))
                    {
                        containerType = ContainerType.Dictionary;
                    }
                    if (genericTypeDefinition == typeof(List<>))
                    {
                        containerType = ContainerType.List;
                    }
                    if (genericTypeDefinition == typeof(Queue<>))
                    {
                        containerType = ContainerType.Queue;
                    }
                }
                else if (type.IsArray)
                {
                    containerType = ContainerType.Array;
                }
                __result = new NullContainerDefinition(new ContainerSaveId(containerType, null, null));
                //__result = new ContainerDefinition(type, new ContainerSaveId());
            }
            */
        }

        /*
        public static MethodBase GetStructDefinitionMethod => AccessTools.DeclaredMethod(typeof(TSSD.TypeDefinition), nameof(TSSD.TypeDefinition.CollectFields));
        public static bool GetStructDefinitionPrefix(TSSD.TypeDefinition __instance)
        {
            if (__instance.Type == typeof(ValueTuple))
                return true;
            ;
            return false;
        }
        */
    }
}