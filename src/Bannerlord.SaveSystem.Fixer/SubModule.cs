using Bannerlord.SaveSystem.Patches;

using HarmonyLib;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.SaveSystem
{
    public class SubModule : MBSubModuleBase
    {
        public SubModule()
        {
            var harmony = new Harmony("org.aragas.bannerlord.savesystem.fix");

            harmony.Patch(LoadContextPatch.GetObjectWithIdMethod,
                postfix: new HarmonyMethod(typeof(LoadContextPatch), nameof(LoadContextPatch.GetObjectWithIdPostfix)));

            harmony.Patch(VariableLoadDataPatch.GetDataToUseMethod,
                transpiler: new HarmonyMethod(typeof(VariableLoadDataPatch), nameof(VariableLoadDataPatch.GetDataToUseTranspiler)));
            harmony.Patch(VariableLoadDataPatch.ReadMethod,
                transpiler: new HarmonyMethod(typeof(VariableLoadDataPatch), nameof(VariableLoadDataPatch.ReadTranspiler)));

            harmony.Patch(DefinitionContextPatch.TryGetTypeDefinitionMethod,
                transpiler: new HarmonyMethod(typeof(DefinitionContextPatch), nameof(DefinitionContextPatch.TryGetTypeDefinitionTranspiler)));
            harmony.Patch(DefinitionContextPatch.GetTypeDefinitionMethod,
                postfix: new HarmonyMethod(typeof(DefinitionContextPatch), nameof(DefinitionContextPatch.GetTypeDefinitionPostfix)));
            harmony.Patch(DefinitionContextPatch.GetClassDefinitionMethod,
                postfix: new HarmonyMethod(typeof(DefinitionContextPatch), nameof(DefinitionContextPatch.GetClassDefinitionPostfix)));

            harmony.Patch(AccessTools.Method(typeof(Crafting).GetNestedType("CraftedItemGenerationHelper", AccessTools.all), "GenerateCraftedItem"),
                prefix: new HarmonyMethod(typeof(CraftingPatch), nameof(CraftingPatch.GenerateCraftedItemPrefix)));

            harmony.Patch(AccessTools.Method(typeof(Campaign), "OnSessionStart"),
                prefix: new HarmonyMethod(typeof(ItemCategoryPatch), nameof(ItemCategoryPatch.OnSessionStartPrefix)));
        }
    }
}