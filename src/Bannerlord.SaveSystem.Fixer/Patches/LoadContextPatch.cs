using Bannerlord.SaveSystem.Definitions;

using HarmonyLib;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using TaleWorlds.SaveSystem.Load;

namespace Bannerlord.SaveSystem.Patches
{
    public static class LoadContextPatch
    {
        public static MethodBase GetObjectWithIdMethod => AccessTools.Method(typeof(LoadContext), "GetObjectWithId");
        public static ObjectHeaderLoadData? GetObjectWithIdPostfix(ObjectHeaderLoadData loadData) => 
            loadData?.TypeDefinition is NullTypeDefinition ? null : loadData;
        public static IEnumerable<CodeInstruction> GetObjectWithIdTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilGenerator)
        {
            var list = instructions.ToList();

            // if (result?.TypeDefinition is NullTypeDefinition) return null;
            var label0 = ilGenerator.DefineLabel();
            var label1 = ilGenerator.DefineLabel();
            var label2 = ilGenerator.DefineLabel();
            var label3 = ilGenerator.DefineLabel();
            list.InsertRange(10, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0) { labels = { label0 }},
                new CodeInstruction(OpCodes.Brtrue_S, label1),

                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Br_S, label2),

                new CodeInstruction(OpCodes.Ldloc_0) { labels = { label1 }}, 
                new CodeInstruction(OpCodes.Call, AccessTools.DeclaredPropertyGetter(typeof(ObjectHeaderLoadData), "TypeDefinition")),
                new CodeInstruction(OpCodes.Isinst, typeof(NullTypeDefinition)){ labels = { label2 }}, // Basic
                new CodeInstruction(OpCodes.Brfalse_S, label3),

                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Ret),

                /*
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Brfalse_S, label),

                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.DeclaredPropertyGetter(typeof(ObjectHeaderLoadData), "TypeDefinition")),
                new CodeInstruction(OpCodes.Isinst, typeof(NullTypeDefinition)), // Basic
                new CodeInstruction(OpCodes.Brfalse_S, label),

                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Ret),
                */
            });
            list[4].labels.Add(label0);
            list[list.Count - 2].labels.Add(label3);
            //

            return list;
        }
    }
}