using HarmonyLib;

using System;
using TaleWorlds.MountAndBlade;

namespace Bannerlord.SaveSystem.Tests.Extensions
{
    public static class MBSubModuleBaseExtensions
    {
        public static void Load(this MBSubModuleBase subModule)
        {
            var method = AccessTools.DeclaredMethod(subModule.GetType(), "OnSubModuleLoad");
            if (method.IsOverridden())
                method.Invoke(subModule, Array.Empty<object>());
        }
    }
}