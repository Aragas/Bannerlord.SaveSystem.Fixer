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
    /// <summary>
    /// Injects NullTypeDefinition into the game's save system
    /// </summary>
    public static class DefinitionContextPatch
    {
        public static MethodBase TryGetTypeDefinitionMethod => AccessTools.DeclaredMethod(typeof(DefinitionContext), "TryGetTypeDefinition");
        public static IEnumerable<CodeInstruction> TryGetTypeDefinitionTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            var list = instructions.ToList();

            // return Utils.GetTypeDefinition(saveId);
            list.InsertRange(list.Count - 2, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1) { labels = list[list.Count - 2].labels },
                new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(typeof(Utils), nameof(Utils.GetTypeDefinition)))
            });
            list.RemoveAt(list.Count - 2);
            //

            // if (typeDefinitionBase == null || typeDefinitionBase is NullTypeDefinition)
            var local = (LocalBuilder) list[41].operand;
            var label1 = ilGenerator.DefineLabel();
            var label2 = ilGenerator.DefineLabel();
            list.RemoveAt(42);
            list.RemoveAt(42);
            list.InsertRange(42, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, local),
                new CodeInstruction(OpCodes.Brfalse_S, label1),

                new CodeInstruction(OpCodes.Ldloc_S, local),
                new CodeInstruction(OpCodes.Isinst, typeof(NullTypeDefinition)), // Basic
                new CodeInstruction(OpCodes.Brfalse_S, label2)
            });
            list[47].labels.Add(label1);
            list[49].labels.Add(label2);
            //

            // if (typeDefinition != null && !(typeDefinition is NullTypeDefinition))
            var label3 = ilGenerator.DefineLabel();
            list.RemoveAt(22);
            list.RemoveAt(22);
            list.InsertRange(22, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Brfalse, label3),

                new CodeInstruction(OpCodes.Ldloc_3),
                new CodeInstruction(OpCodes.Isinst, typeof(NullTypeDefinition)), // Basic
                new CodeInstruction(OpCodes.Brtrue, label3)
            });
            list[list.Count - 3].labels.Add(label3);
            //

            return list;
        }

        public static MethodBase GetTypeDefinitionMethod => AccessTools.DeclaredMethod(typeof(DefinitionContext), "GetTypeDefinition");
        public static TypeDefinitionBase GetTypeDefinitionPostfix(TypeDefinitionBase typeDefinition) =>
            typeDefinition ?? new NullTypeDefinition(0);

        public static MethodInfo IsContainerMethod { get; } = AccessTools.Method(
            Type.GetType("TaleWorlds.SaveSystem.TypeExtensions, TaleWorlds.SaveSystem"),
            "IsContainer",
            new [] { typeof(Type) });

        public static MethodBase GetClassDefinitionMethod => AccessTools.DeclaredMethod(typeof(DefinitionContext), "GetClassDefinition");
        public static void GetClassDefinitionPostfix(Type type, ref TypeDefinition __result)
        {
            if (__result == null && (bool) IsContainerMethod.Invoke(null, new object[] { type }))
                __result = new NullTypeDefinition(0);
        }

        public static MethodBase GetStructDefinitionMethod => AccessTools.DeclaredMethod(typeof(TypeDefinition), nameof(TypeDefinition.CollectFields));
        public static bool GetStructDefinitionPrefix(TypeDefinition __instance)
        {
            if (__instance.Type == typeof(ValueTuple))
                return true;
            ;
            return false;
        }
    }
}