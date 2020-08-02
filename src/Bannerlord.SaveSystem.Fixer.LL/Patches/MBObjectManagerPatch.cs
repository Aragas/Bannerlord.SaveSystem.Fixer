/*
using HarmonyLib;

using System;
using System.Linq;
using System.Reflection;

using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.Patches
{
    public static class MBObjectManagerPatch
    {
        private static PropertyInfo ItemCategoryProperty { get; } = AccessTools.Property(typeof(ItemObject), nameof(ItemObject.ItemCategory));

        public static MethodBase AfterLoadMethod => AccessTools.Method(typeof(MBObjectManager), nameof(MBObjectManager.AfterLoad));
        /// <summary>
        /// Replace custom item categories with DefaultItemCategories.Unassigned
        /// </summary>
        public static void AfterLoadPrefix()
        {
            foreach (var itemObject in ItemObject.All.Where(itemObject => itemObject.ItemCategory == null))
                ItemCategoryProperty.SetValue(itemObject, DefaultItemCategories.Unassigned);
        }


        private static MethodInfo GetDataToUseMethod => AccessTools.Method(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "GetDataToUse");

        public static MethodBase FillObjectMethod => AccessTools.Method(Type.GetType("TaleWorlds.SaveSystem.Load.FieldLoadData, TaleWorlds.SaveSystem"), "FillObject");
        public static void FillObjectPostfix(FieldDefinition __instance, FieldInfo ____fieldInfo)
        {
            if (____fieldInfo == null)
                return;

            if (____fieldInfo.FieldType == null)
                ;

            var dataToUse = GetDataToUseMethod.Invoke(__instance, Array.Empty<object>());
            if (dataToUse != null && !____fieldInfo.FieldType.IsInstanceOfType(dataToUse))
                ;
            else
                ;
            ;
        }
    }
}
*/