using HarmonyLib;

using System.Collections.Generic;
using System.Linq;

using TaleWorlds.Core;

namespace Bannerlord.SaveSystem.Patches
{
    public static class CraftingPatch
    {
        private static AccessTools.FieldRef<WeaponDesign, WeaponDesignElement[]> UsedPiecesFieldRef =
            AccessTools.FieldRefAccess<WeaponDesign, WeaponDesignElement[]>("_usedPieces");

        /// <summary>
        /// Replace custom crafting pieces with the default ones
        /// </summary>
        public static void GenerateCraftedItemPrefix(WeaponDesign craftedData)
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