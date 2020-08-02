using HarmonyLib;

using System;
using System.Reflection;
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
    public static class MethodInfoExtensions
    {
        public static bool IsOverridden(this MethodInfo methodInfo) => methodInfo.GetBaseDefinition() != methodInfo;
    }
}