using Bannerlord.SaveSystem.Patches;

using HarmonyLib;

using TaleWorlds.MountAndBlade;

namespace Bannerlord.SaveSystem
{
    public class SubModule : MBSubModuleBase
    {
        private readonly Harmony _harmony = new Harmony("org.aragas.bannerlord.savesystem.fixer.highlevel");

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            LoadResultPatch.InitializeObjects_CallExternalCallback_Transpiler.Enable(_harmony);

            DefinitionContextPatch.TryGetTypeDefinition_AddNullTypeDefinition.Enable(_harmony);
            DefinitionContextPatch.GetTypeDefinition_NullTypeDefinitionWhenNull.Enable(_harmony);
            DefinitionContextPatch.GetClassDefinition_NullTypeDefinitionWhenNull.Enable(_harmony);

            SaveManagerPatch.InitializeObjects_CallExternalCallback.Enable(_harmony);

            CraftingPatch.GenerateCraftedItem_ReplaceInvalidPieces.Enable(_harmony);

            ItemCategoryPatch.OnSessionStart_FixItemCategories.Enable(_harmony);

            DefinitionContextPatch.TryGetTypeDefinition_AddNullTypeDefinition.Enable(_harmony);
            DefinitionContextPatch.GetTypeDefinition_NullTypeDefinitionWhenNull.Enable(_harmony);
            DefinitionContextPatch.GetClassDefinition_NullTypeDefinitionWhenNull.Enable(_harmony);

            SaveManagerPatch.InitializeObjects_CallExternalCallback.Enable(_harmony);

            CraftingPatch.GenerateCraftedItem_ReplaceInvalidPieces.Enable(_harmony);

            ItemCategoryPatch.OnSessionStart_FixItemCategories.Enable(_harmony);
        }
    }
}