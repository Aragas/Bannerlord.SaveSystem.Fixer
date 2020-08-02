using Bannerlord.SaveSystem.HarmonyPatch;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.Library;

using TSSL = TaleWorlds.SaveSystem.Load;

namespace Bannerlord.SaveSystem.Patches
{
    public static class LoadCallbackInitializatorPatch
    {
        public static HarmonyPatchEntry InitializeObjects_IgnoreInvalidCallbacks { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(Type.GetType("TaleWorlds.SaveSystem.Load.LoadCallbackInitializator, TaleWorlds.SaveSystem"), "InitializeObjects"),
            new HarmonyMethod(typeof(LoadCallbackInitializatorPatch), nameof(InitializeObjectsFinalizer)),
            HarmonyPatchType.Finalizer);


        private static MethodInfo CreateLoadDataMethod { get; } = AccessTools.DeclaredMethod(typeof(TSSL.LoadContext), "CreateLoadData");
        private static void InitializeObjectsFinalizer(Exception __exception, TSSL.ObjectHeaderLoadData[] ____objectHeaderLoadDatas, int ____objectCount, TaleWorlds.SaveSystem.LoadData ____loadData)
        {
            if (__exception != null)
            {
                using (new PerformanceTestBlock("LoadContext::Callbacks"))
                {
                    for (var i = 0; i < ____objectCount; i++)
                    {
                        var objectHeaderLoadData = ____objectHeaderLoadDatas[i];
                        if (objectHeaderLoadData?.Target != null && objectHeaderLoadData.TypeDefinition?.InitializationCallbacks is { } enumerable)
                        {
                            foreach (var methodInfo in enumerable)
                            {
                                var parameters = methodInfo.GetParameters();
                                if (parameters.Length > 1 && parameters[1].ParameterType == typeof(TSSL.ObjectLoadData))
                                {
                                    try
                                    {
                                        var objectLoadData = (TSSL.LoadContext) CreateLoadDataMethod.Invoke(null, new object[] { ____loadData, i, objectHeaderLoadData });
                                        methodInfo.Invoke(objectHeaderLoadData.Target, new object[] { ____loadData.MetaData, objectLoadData });
                                    }
                                    catch { }
                                }
                                else
                                {
                                    try
                                    {
                                        methodInfo.Invoke(objectHeaderLoadData.Target, new object[] { ____loadData.MetaData });
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}