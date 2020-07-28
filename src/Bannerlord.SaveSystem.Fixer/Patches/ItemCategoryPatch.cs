using HarmonyLib;

using System.Linq;
using System.Reflection;

using TaleWorlds.Core;

namespace Bannerlord.SaveSystem.Patches
{
    public static class ItemCategoryPatch
    {
        private static PropertyInfo ItemCategoryProperty { get; } =
            AccessTools.Property(typeof(ItemObject), nameof(ItemObject.ItemCategory));

        /// <summary>
        /// Replace custom item categories with DefaultItemCategories.Unassigned
        /// </summary>
        public static void OnSessionStartPrefix()
        {
            foreach (var itemObject in ItemObject.All.Where(itemObject => itemObject.ItemCategory == null))
                ItemCategoryProperty.SetValue(itemObject, DefaultItemCategories.Unassigned);
        }
    }
}
