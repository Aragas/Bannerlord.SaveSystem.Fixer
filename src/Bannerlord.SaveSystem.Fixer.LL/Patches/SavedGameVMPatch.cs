using Bannerlord.SaveSystem.HarmonyPatch;

using HarmonyLib;

using System;

using TaleWorlds.MountAndBlade;

namespace Bannerlord.SaveSystem.Patches
{
    public static class SavedGameVMPatch
    {
        public static HarmonyPatchEntry StartGame_ReturnToMenuOnCrash { get; } = new HarmonyPatchEntry(
            AccessTools.DeclaredMethod(Type.GetType("SandBox.ViewModelCollection.SaveLoad.SavedGameVM, SandBox.ViewModelCollection"), "StartGame"),
            new HarmonyMethod(typeof(ContainerLoadDataPatch), nameof(StartGameFinalizer)),
            HarmonyPatchType.Finalizer);

        private static void StartGameFinalizer(Exception __exception)
        {
            if (__exception != null)
            {
                MBGameManager.EndGame();
            }
        }
    }
}