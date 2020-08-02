using Bannerlord.SaveSystem.Load;

using TaleWorlds.Library;

namespace Bannerlord.SaveSystem.LoadData
{
    internal class PropertyLoadData : MemberLoadData
    {
        public PropertyLoadData(ObjectLoadData objectLoadData, IReader reader) : base(objectLoadData, reader) { }

        public void FillObject()
        {
            TaleWorlds.SaveSystem.Definition.PropertyDefinition definitionWithId;
            if (ObjectLoadData.TypeDefinition == null || (definitionWithId = ObjectLoadData.TypeDefinition.GetPropertyDefinitionWithId(MemberSaveId)) == null)
                return;
            var setMethod = definitionWithId.SetMethod;
            var target = ObjectLoadData.Target;
            var dataToUse = GetDataToUse();
            if (dataToUse != null && !definitionWithId.PropertyInfo.PropertyType.IsInstanceOfType(dataToUse) && !LoadContext.TryConvertType(dataToUse.GetType(), definitionWithId.PropertyInfo.PropertyType, ref dataToUse))
                return;
            setMethod.Invoke(target, new[] { dataToUse });
        }
    }
}