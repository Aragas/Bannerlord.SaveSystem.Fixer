using Bannerlord.SaveSystem.Definitions;
using Bannerlord.SaveSystem.Extensions;
using Bannerlord.SaveSystem.Load;

using HarmonyLib;

using System;
using System.Reflection;

using TaleWorlds.Library;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.LoadData
{
    internal abstract class VariableLoadData
    {
        private readonly IReader _reader;
        private TSSD.TypeDefinitionBase _typeDefinition;
        private TSSD.SaveId _saveId;
        private object _customStructObject;

        public LoadContext Context { get; }
        public TSSD.MemberTypeId MemberSaveId { get; private set; }
        public SavedMemberType SavedMemberType { get; private set; }
        public object Data { get; private set; }

        protected VariableLoadData(LoadContext context, IReader reader)
        {
            Context = context;
            _reader = reader;
        }

        // PATCH
        private static PropertyInfo SerializerProperty { get; } = AccessTools.Property(Type.GetType("TaleWorlds.SaveSystem.Definition.BasicTypeDefinition, TaleWorlds.SaveSystem"), "Serializer");
        // PATCH
        public void Read()
        {
            SavedMemberType = (SavedMemberType) _reader.ReadByte();
            MemberSaveId = new TSSD.MemberTypeId
            {
                TypeLevel = _reader.ReadByte(),
                LocalSaveId = _reader.ReadShort()
            };
            if (SavedMemberType == SavedMemberType.Object)
            {
                Data = _reader.ReadInt();
                return;
            }

            if (SavedMemberType == SavedMemberType.Container)
            {
                Data = _reader.ReadInt();
                return;
            }

            if (SavedMemberType == SavedMemberType.String)
            {
                Data = _reader.ReadInt();
                return;
            }

            if (SavedMemberType == SavedMemberType.Enum)
            {
                _saveId = TSSD.SaveId.ReadSaveIdFrom(_reader);
                _typeDefinition = Context.DefinitionContext.TryGetTypeDefinition(_saveId);
                Data = _reader.ReadString();
                return;
            }

            if (SavedMemberType == SavedMemberType.BasicType)
            {
                _saveId = TSSD.SaveId.ReadSaveIdFrom(_reader);
                _typeDefinition = Context.DefinitionContext.TryGetTypeDefinition(_saveId);
                // PATCH
                if (_typeDefinition is NullTypeDefinition)
                    return;
                var serializer = (TSSD.IBasicTypeSerializer) SerializerProperty.GetValue(_typeDefinition);
                // PATCH
                Data = serializer.Deserialize(_reader);
                return;
            }

            if (SavedMemberType == SavedMemberType.CustomStruct)
            {
                Data = _reader.ReadInt();
            }
        }

        public void SetCustomStructData(object customStructObject)
        {
            _customStructObject = customStructObject;
        }

        public object? GetDataToUse()
        {
            // PATCH
            if (SavedMemberType == SavedMemberType.Enum && (_typeDefinition == null || _typeDefinition.Type == typeof(object)))
                return null;

            if (_typeDefinition is NullTypeDefinition)
                return null;
            // PATCH

            object? result = null;
            if (SavedMemberType == SavedMemberType.Object)
            {
                var objectWithId = Context.GetObjectWithId((int) Data);
                if (objectWithId != null)
                {
                    result = objectWithId.Target;
                }
            }
            else if (SavedMemberType == SavedMemberType.Container)
            {
                var containerWithId = Context.GetContainerWithId((int) Data);
                if (containerWithId != null)
                {
                    result = containerWithId.Target;
                }
            }
            else if (SavedMemberType == SavedMemberType.String)
            {
                var id = (int) Data;
                result = Context.GetStringWithId(id);
            }
            else if (SavedMemberType == SavedMemberType.Enum)
            {
                result = Enum.Parse(_typeDefinition.Type, (string) Data);
            }
            else if (SavedMemberType == SavedMemberType.BasicType)
            {
                result = Data;
            }
            else if (SavedMemberType == SavedMemberType.CustomStruct)
            {
                result = _customStructObject;
            }

            return result;
        }
    }
}