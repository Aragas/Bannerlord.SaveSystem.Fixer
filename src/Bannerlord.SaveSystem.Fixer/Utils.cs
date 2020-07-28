using Bannerlord.SaveSystem.Definitions;

using System.Collections.Generic;

using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem
{
    public static class Utils
    {
        public static TypeDefinitionBase GetTypeDefinition(SaveId saveId)
        {
            if (saveId is ContainerSaveId containerSaveId)
            {
                switch (containerSaveId.ContainerType)
                {
                    case ContainerType.List:
                        return new ContainerDefinition(typeof(List<object>), containerSaveId);
                    case ContainerType.Dictionary:
                        return new ContainerDefinition(typeof(Dictionary<object, object>), containerSaveId);
                    case ContainerType.Array:
                        return new ContainerDefinition(typeof(object[]), containerSaveId);
                    case ContainerType.Queue:
                        return new ContainerDefinition(typeof(Queue<object>), containerSaveId);
                }
            }

            return new NullTypeDefinition(saveId);
        }
    }
}