using Bannerlord.SaveSystem.Definitions;

using System.Collections.Generic;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem
{
    public static class Utils
    {
        public static TSSD.TypeDefinitionBase GetTypeDefinition(TSSD.SaveId saveId, TSSD.DefinitionContext definitionContext)
        {
            if (saveId is TSSD.ContainerSaveId containerSaveId)
            {
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