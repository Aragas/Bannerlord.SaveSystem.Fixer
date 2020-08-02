using HarmonyLib;

using System;
using System.Reflection;
using System.Runtime.Serialization;

using Bannerlord.SaveSystem.HarmonyPatch;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;
using TSSL = TaleWorlds.SaveSystem.Load;

namespace Bannerlord.SaveSystem.Patches
{
    public static class FieldLoadDataPatch
    {
        public static HarmonyPatchEntry FillObject_SetNonNullValues { get; } = new HarmonyPatchEntry(
            AccessTools.Method(Type.GetType("TaleWorlds.SaveSystem.Load.FieldLoadData, TaleWorlds.SaveSystem"), "FillObject"),
            new HarmonyMethod(typeof(FieldLoadDataPatch), nameof(FillObjectFinalizer)),
            HarmonyPatchType.Finalizer);


        private static MethodInfo IsContainerMethod { get; } = AccessTools.Method(Type.GetType("TaleWorlds.SaveSystem.TypeExtensions, TaleWorlds.SaveSystem"), "IsContainer", new[] { typeof(Type), typeof(TSS.ContainerType).MakeByRefType() });
        private static MethodInfo GetDataToUseMethod { get; } = AccessTools.Method(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "GetDataToUse");
        private static PropertyInfo MemberSaveIdProperty { get; } = AccessTools.Property(Type.GetType("TaleWorlds.SaveSystem.Load.VariableLoadData, TaleWorlds.SaveSystem"), "MemberSaveId");
        private static PropertyInfo ObjectLoadDataProperty { get; } = AccessTools.Property(Type.GetType("TaleWorlds.SaveSystem.Load.MemberLoadData, TaleWorlds.SaveSystem"), "ObjectLoadData");
        private static void FillObjectFinalizer(Exception __exception, object __instance)
        {
            if (__exception != null)
            {
                ;
            }

            var objectLoadData = (TSSL.ObjectLoadData) ObjectLoadDataProperty.GetValue(__instance);
            var memberSaveId = (TSSD.MemberTypeId) MemberSaveIdProperty.GetValue(__instance);

            TSSD.FieldDefinition definitionWithId;
            if (objectLoadData.TypeDefinition == null || (definitionWithId = objectLoadData.TypeDefinition.GetFieldDefinitionWithId(memberSaveId)) == null)
                return;

            var fieldInfo = definitionWithId.FieldInfo;

            var dataToUse = GetDataToUseMethod.Invoke(__instance, Array.Empty<object>());
            if (dataToUse == null) // Init containers empty
            {
                object[] parameters = new object[] { fieldInfo.FieldType, null };
                var isContainer = (bool) IsContainerMethod.Invoke(null, parameters);

                if (isContainer)
                {
                    switch ((TSS.ContainerType) parameters[1])
                    {
                        case TSS.ContainerType.List:
                            var list = Activator.CreateInstance(fieldInfo.FieldType);
                            fieldInfo.SetValue(objectLoadData.Target, list);
                            break;
                        case TSS.ContainerType.Dictionary:
                            var dict = Activator.CreateInstance(fieldInfo.FieldType);
                            fieldInfo.SetValue(objectLoadData.Target, dict);
                            break;
                        case TSS.ContainerType.Array:
                            var array = Activator.CreateInstance(fieldInfo.FieldType, new object[] { 0 });
                            fieldInfo.SetValue(objectLoadData.Target, array);
                            break;
                        case TSS.ContainerType.Queue:
                            var queue = Activator.CreateInstance(fieldInfo.FieldType);
                            fieldInfo.SetValue(objectLoadData.Target, queue);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    //if (objectLoadData.Target is TextObject)
                    //    return;

                    if (fieldInfo.FieldType == typeof(string))
                    {
                        fieldInfo.SetValue(objectLoadData.Target, "");
                    }
                    else if (fieldInfo.FieldType.IsPrimitive)
                        return;
                    else if (!fieldInfo.FieldType.IsAbstract)
                    {
                        var obj = FormatterServices.GetUninitializedObject(fieldInfo.FieldType);
                        fieldInfo.SetValue(objectLoadData.Target, obj);                
                    }
                }
            }
        }
    }
}