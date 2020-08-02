using HarmonyLib;

using System;
using System.Reflection;

namespace Bannerlord.SaveSystem.HarmonyPatch
{
    public class HarmonyPatchEntry
    {
        public MethodBase MethodRef { get; }
        public HarmonyMethod MethodPatch { get; }
        public HarmonyPatchType PatchType { get; }

        public HarmonyPatchEntry(MethodBase methodRef, HarmonyMethod methodPatch, HarmonyPatchType patchType)
        {
            MethodRef = methodRef;
            MethodPatch = methodPatch;
            PatchType = patchType;
        }

        public void Enable(HarmonyLib.Harmony harmony)
        {
            switch (PatchType)
            {
                case HarmonyPatchType.Prefix:
                    harmony.Patch(MethodRef, prefix: MethodPatch);
                    break;
                case HarmonyPatchType.Postfix:
                    harmony.Patch(MethodRef, postfix: MethodPatch);
                    break;
                case HarmonyPatchType.Transpiler:
                    harmony.Patch(MethodRef, transpiler: MethodPatch);
                    break;
                case HarmonyPatchType.Finalizer:
                    harmony.Patch(MethodRef, finalizer: MethodPatch);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Disable(HarmonyLib.Harmony harmony)
        {
            switch (PatchType)
            {
                case HarmonyPatchType.Prefix:
                    harmony.Unpatch(MethodRef, MethodPatch.method);
                    break;
                case HarmonyPatchType.Postfix:
                    harmony.Unpatch(MethodRef, MethodPatch.method);
                    break;
                case HarmonyPatchType.Transpiler:
                    harmony.Unpatch(MethodRef, MethodPatch.method);
                    break;
                case HarmonyPatchType.Finalizer:
                    harmony.Unpatch(MethodRef, MethodPatch.method);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}