using Bannerlord.SaveSystem.HarmonyPatch;

using HarmonyLib;

using System.Linq;
using System.Reflection;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Bannerlord.SaveSystem.Patches
{
    public static class ItemCategoryPatch
    {
        public static HarmonyPatchEntry OnSessionStart_FixItemCategories { get; } = new HarmonyPatchEntry(
            AccessTools.Method(typeof(Campaign), "OnSessionStart"),
            new HarmonyMethod(typeof(ItemCategoryPatch), nameof(OnSessionStartPrefix)),
            HarmonyPatchType.Prefix);


        private static PropertyInfo ItemCategoryProperty { get; } = AccessTools.Property(typeof(ItemObject), nameof(ItemObject.ItemCategory));
        /// <summary>
        /// Replace custom item categories with DefaultItemCategories.Unassigned
        /// </summary>
        private static void OnSessionStartPrefix()
        {
            foreach (var itemObject in ItemObject.All.Where(itemObject => itemObject.ItemCategory == null))
                ItemCategoryProperty.SetValue(itemObject, DefaultItemCategories.Unassigned);
        }
    }
}
