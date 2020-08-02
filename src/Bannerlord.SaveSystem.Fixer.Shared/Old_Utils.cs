using Bannerlord.SaveSystem.Definitions;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Reflection;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem
{
    public static class Utils
    {
        private static Type TypeId = Type.GetType("TaleWorlds.SaveSystem.Definition.TypeSaveId, TaleWorlds.SaveSystem");
        private static PropertyInfo Id = AccessTools.Property(TypeId, "Id");

        private static AccessTools.FieldRef<TSSD.DefinitionContext, Dictionary<TSSD.SaveId, TSSD.TypeDefinitionBase>> _allTypeDefinitionsWithId =
            AccessTools.FieldRefAccess<TSSD.DefinitionContext, Dictionary<TSSD.SaveId, TSSD.TypeDefinitionBase>>("_allTypeDefinitionsWithId");

        public static TSSD.TypeDefinitionBase GetTypeDefinition(TSSD.SaveId saveId, TSSD.DefinitionContext definitionContext)
        {
            if (saveId is TSSD.ContainerSaveId containerSaveId)
            {
                /*
                return null;
                var dict = _allTypeDefinitionsWithId(definitionContext);
                if (dict.TryGetValue(containerSaveId.KeyId, out var result))
                {
                    ;
                }
                else
                {
                    var id = (int) Id.GetValue(containerSaveId.KeyId);

                    var type1 = Type.GetType("TaleWorlds.SaveSystem.Definition.BasicTypeDefinition, TaleWorlds.SaveSystem");
                    var type2 = Type.GetType("TaleWorlds.SaveSystem.Definition.ByteBasicTypeSerializer, TaleWorlds.SaveSystem");

                    var serializer = Activator.CreateInstance(type2, Array.Empty<object>());

                    var definition = (TypeDefinitionBase) Activator.CreateInstance(type1, new[] { typeof(object), id, serializer });
                    dict.Add(containerSaveId.KeyId, definition);
                }

                return new NullContainerDefinition(containerSaveId);
                */
                switch (containerSaveId.ContainerType)
                {
                    case TSS.ContainerType.List:
                        return new TSSD.ContainerDefinition(typeof(List<object>), containerSaveId);
                    case TSS.ContainerType.Dictionary:
                        return new TSSD.ContainerDefinition(typeof(Dictionary<object, object>), containerSaveId);
                    case TSS.ContainerType.Array:
                        return new TSSD.ContainerDefinition(typeof(object[]), containerSaveId);
                    case TSS.ContainerType.Queue:
                        return new TSSD.ContainerDefinition(typeof(Queue<object>), containerSaveId);
                }
            }

            return new NullTypeDefinition(saveId);
        }
    }
}