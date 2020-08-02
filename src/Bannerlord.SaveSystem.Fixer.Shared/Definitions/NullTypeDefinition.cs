using System;
using System.Collections.Generic;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.Definitions
{
    /// <summary>
    /// Custom class which the fixer uses to track the non-existing types
    /// </summary>
    public class NullTypeDefinition : TSSD.TypeDefinition
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

        public NullTypeDefinition(TSSD.SaveId saveId) : base(GetType(saveId), 0, new TSS.DefaultObjectResolver()) { }
        public NullTypeDefinition(int saveId) : base(typeof(object), saveId, new TSS.DefaultObjectResolver()) { }
    }
}