using System;
using System.Collections.Generic;

using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.Definitions
{
    /// <summary>
    /// Custom class which the fixer uses to track the non-existing types
    /// </summary>
    public class NullTypeDefinition : TypeDefinition
    {
        private static Type GetType(SaveId saveId)
        {
            if (saveId is ContainerSaveId containerSaveId)
            {
                switch (containerSaveId.ContainerType)
                {
                    case ContainerType.List:
                        return typeof(List<object>);
                    case ContainerType.Dictionary:
                        return typeof(Dictionary<object, object>);
                    case ContainerType.Array:
                        return typeof(object[]);
                    case ContainerType.Queue:
                        return typeof(Queue<object>);
                }
            }

            return typeof(object);
        }

        public NullTypeDefinition(SaveId saveId) : base(GetType(saveId), 0, new DefaultObjectResolver()) { }
        public NullTypeDefinition(int saveId) : base(typeof(object), saveId, new DefaultObjectResolver()) { }
    }
}