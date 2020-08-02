using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;

namespace Bannerlord.SaveSystem
{
    public class Settings : GlobalSettings<Settings>
    {
        private bool _preventSaveCrash;
        private bool _advancedFixes;

        public override string Id => "Bannerlord.SaveSystem.Fixer.LL_v1";
        public override string DisplayName => $"Aragas's Save Fixer (LL) v{typeof(Settings).Assembly.GetName().Version}";

        [SettingPropertyBool("Advanced Fixes", RequireRestart = false)]
        public bool AdvancedFixes
        {
            get => _advancedFixes;
            set
            {
                _advancedFixes = value;
                OnPropertyChanged(nameof(AdvancedFixes));
            }
        }

        [SettingPropertyBool("Prevent Load Crash", HintText = "Prevents the game from crashing when it attempts to load a broken save file.", RequireRestart = false)]
        public bool PreventSaveCrash
        {
            get => _preventSaveCrash;
            set
            {
                _preventSaveCrash = value;
                OnPropertyChanged(nameof(PreventSaveCrash));
            }
        }
    }
}