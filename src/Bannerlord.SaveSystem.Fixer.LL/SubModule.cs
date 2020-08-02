using Bannerlord.SaveSystem.Patches;

using HarmonyLib;

using TaleWorlds.MountAndBlade;

namespace Bannerlord.SaveSystem
{
    public class SubModule : MBSubModuleBase
    {
        private readonly Harmony _harmony = new Harmony("org.aragas.bannerlord.savesystem.fixer.lowlevel");

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            LoadContextPatch.GetObjectWithId_NullTypeDefinitionToNull.Enable(_harmony);

            VariableLoadDataPatch.GetDataToUse_NullTypeDefinitionAndEnumNullFix.Enable(_harmony);
            VariableLoadDataPatch.Read_NullTypeDefinitionSkip.Enable(_harmony);
            ContainerLoadDataPatch.FillObject_CheckForNull.Enable(_harmony);
            ContainerLoadDataPatch.FillObject_Debug.Enable(_harmony);
            FieldLoadDataPatch.FillObject_SetNonNullValues.Enable(_harmony);
            PropertyLoadDataPatch.FillObject_SetNonNullValues.Enable(_harmony);

            DefinitionContextPatch.TryGetTypeDefinition_AddNullTypeDefinition.Enable(_harmony);
            DefinitionContextPatch.GetTypeDefinition_NullTypeDefinitionWhenNull.Enable(_harmony);
            DefinitionContextPatch.GetClassDefinition_NullTypeDefinitionWhenNull.Enable(_harmony);

            LoadCallbackInitializatorPatch.InitializeObjects_IgnoreInvalidCallbacks.Enable(_harmony);

            CraftingPatch.GenerateCraftedItem_ReplaceInvalidPieces.Enable(_harmony);

            ItemCategoryPatch.OnSessionStart_FixItemCategories.Enable(_harmony);
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            if (Settings.Instance is { } settings)
                settings.PropertyChanged += Settings_PropertyChanged;
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();

            if (Settings.Instance is { } settings)
                settings.PropertyChanged -= Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.PreventSaveCrash))
            {
                if (Settings.Instance?.PreventSaveCrash == true)
                    SavedGameVMPatch.StartGame_ReturnToMenuOnCrash.Enable(_harmony);
                else
                    SavedGameVMPatch.StartGame_ReturnToMenuOnCrash.Disable(_harmony);
            }
        }
    }
}