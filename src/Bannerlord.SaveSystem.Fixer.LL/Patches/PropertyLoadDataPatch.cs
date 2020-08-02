using Bannerlord.SaveSystem.HarmonyPatch;

using HarmonyLib;

using System;
using System.Reflection;
using System.Runtime.Serialization;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;
using TSSL = TaleWorlds.SaveSystem.Load;

namespace Bannerlord.SaveSystem.Patches
{
    public static class PropertyLoadDataPatch
    {
        public static HarmonyPatchEntry FillObject_SetNonNullValues { get; } = new HarmonyPatchEntry(
            AccessTools.Method(Type.GetType("TaleWorlds.SaveSystem.Load.PropertyLoadData, TaleWorlds.SaveSystem"), "FillObject"),
            new HarmonyMethod(typeof(PropertyLoadDataPatch), nameof(FillObjectFinalizer)),
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

            TSSD.PropertyDefinition definitionWithId;
            if (objectLoadData.TypeDefinition == null || (definitionWithId = objectLoadData.TypeDefinition.GetPropertyDefinitionWithId(memberSaveId)) == null)
                return;

            var propertyInfo = definitionWithId.PropertyInfo;
            var setMethod = definitionWithId.SetMethod;

            var dataToUse = GetDataToUseMethod.Invoke(__instance, Array.Empty<object>());
            if (dataToUse == null)
            {
                object[] parameters = new object[] { propertyInfo.PropertyType, null };
                var isContainer = (bool) IsContainerMethod.Invoke(null, parameters);

                if (isContainer)
                {
                    switch ((TSS.ContainerType) parameters[1])
                    {
                        case TSS.ContainerType.List:
                            var list = Activator.CreateInstance(propertyInfo.PropertyType);
                            setMethod.Invoke(objectLoadData.Target, new object[] { list });
                            break;
                        case TSS.ContainerType.Dictionary:
                            var dict = Activator.CreateInstance(propertyInfo.PropertyType);
                            setMethod.Invoke(objectLoadData.Target, new object[] { dict });
                            break;
                        case TSS.ContainerType.Array:
                            var array = Activator.CreateInstance(propertyInfo.PropertyType, new object[] { 0 });
                            setMethod.Invoke(objectLoadData.Target, new object[] { array });
                            break;
                        case TSS.ContainerType.Queue:
                            var queue = Activator.CreateInstance(propertyInfo.PropertyType);
                            setMethod.Invoke(objectLoadData.Target, new object[] { queue });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        setMethod.Invoke(objectLoadData.Target, new object[] { "" });
                    }
                    else if (propertyInfo.PropertyType.IsPrimitive)
                        return;
                    else if (!propertyInfo.PropertyType.IsAbstract)
                    {
                        var obj = FormatterServices.GetUninitializedObject(propertyInfo.PropertyType);
                        setMethod.Invoke(objectLoadData.Target, new object[] { obj });
                    }
                }
            }
        }
    }
}