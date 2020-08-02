using System;
using System.Collections.Generic;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.Definitions
{
    public class NullContainerDefinition : TSSD.TypeDefinition
    {
        private static Type GetType(TSSD.SaveId saveId)
        {
            if (saveId is TSSD.ContainerSaveId containerSaveId)
            {
                switch (containerSaveId.ContainerType)
                {
                    case TSS.ContainerType.List:
                        return typeof(List<object>);
                    case TSS.ContainerType.Dictionary:
                        return typeof(Dictionary<object, object>);
                    case TSS.ContainerType.Array:
                        return typeof(object[]);
                    case TSS.ContainerType.Queue:
                        return typeof(Queue<object>);
                }
            }

            return typeof(object);
        }

        public NullContainerDefinition(TSSD.SaveId saveId) : base(GetType(saveId), 0, new TSS.DefaultObjectResolver()) { }
        public NullContainerDefinition(int saveId) : base(typeof(object), saveId, new TSS.DefaultObjectResolver()) { }
    }
}