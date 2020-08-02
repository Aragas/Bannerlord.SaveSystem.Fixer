using Bannerlord.SaveSystem.HarmonyPatch;

using HarmonyLib;

using System.Collections.Generic;
using System.Linq;

using TaleWorlds.Core;

namespace Bannerlord.SaveSystem.Patches
{
    public static class CraftingPatch
    {
        public static HarmonyPatchEntry GenerateCraftedItem_ReplaceInvalidPieces { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(typeof(Crafting).GetNestedType("CraftedItemGenerationHelper", AccessTools.all), "GenerateCraftedItem"),
            new HarmonyMethod(typeof(CraftingPatch), nameof(GenerateCraftedItemPrefix)),
            HarmonyPatchType.Prefix);


        private static AccessTools.FieldRef<WeaponDesign, WeaponDesignElement[]> UsedPiecesFieldRef { get; } = AccessTools.FieldRefAccess<WeaponDesign, WeaponDesignElement[]>("_usedPieces");
        /// <summary>
        /// Replace custom crafting pieces with the default ones
        /// </summary>
        private static void GenerateCraftedItemPrefix(WeaponDesign craftedData)
        {
            var validPieces = new List<WeaponDesignElement>();
            foreach (var weaponDesignElement in craftedData.UsedPieces)
            {
                if (weaponDesignElement.IsValid && !craftedData.Template.Pieces.Contains(weaponDesignElement.CraftingPiece))
                {
                    var replacementCraftingPiece = craftedData.Template.Pieces.First(p => p.PieceType == weaponDesignElement.CraftingPiece.PieceType);
                    validPieces.Add(WeaponDesignElement.CreateUsablePiece(replacementCraftingPiece));
                }
                else
                    validPieces.Add(weaponDesignElement);
            }
            UsedPiecesFieldRef(craftedData) = validPieces.ToArray();
        }
    }
}