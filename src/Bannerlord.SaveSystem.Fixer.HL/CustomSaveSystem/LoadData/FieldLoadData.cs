using System.Reflection;

using TaleWorlds.Library;

namespace Bannerlord.SaveSystem.LoadData
{
    internal class FieldLoadData : MemberLoadData
    {
        private FieldInfo? _fieldInfo;

        public FieldLoadData(ObjectLoadData objectLoadData, IReader reader) : base(objectLoadData, reader) { }

        public void FillObject()
        {
            TaleWorlds.SaveSystem.Definition.FieldDefinition definitionWithId;
            if (ObjectLoadData.TypeDefinition == null || (definitionWithId = ObjectLoadData.TypeDefinition.GetFieldDefinitionWithId(MemberSaveId)) == null)
                return;
            _fieldInfo = definitionWithId.FieldInfo;
            var target = ObjectLoadData.Target;
            var dataToUse = GetDataToUse();
            if (dataToUse != null && !_fieldInfo.FieldType.IsInstanceOfType(dataToUse) && !Load.LoadContext.TryConvertType(dataToUse.GetType(), _fieldInfo.FieldType, ref dataToUse))
                return;
            _fieldInfo.SetValue(target, dataToUse);
        }
    }
}