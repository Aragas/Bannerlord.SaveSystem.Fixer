using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Bannerlord.SaveSystem.Extensions;
using Bannerlord.SaveSystem.Load;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.LoadData
{
    public class ObjectLoadData
	{
        private TSSD.SaveId _saveId;
        private short _propertyCount;
        private readonly List<PropertyLoadData> _propertyValues = new List<PropertyLoadData>();
        private readonly List<FieldLoadData> _fieldValues = new List<FieldLoadData>();
        private readonly List<MemberLoadData> _memberValues = new List<MemberLoadData>();
        private readonly List<ObjectLoadData> _childStructs = new List<ObjectLoadData>();
        private short _childStructCount;

        public int Id { get; }
        public LoadContext Context { get; }

		public object Target { get; private set; }
		
		public TSSD.TypeDefinition TypeDefinition { get; private set; }

		public object GetDataBySaveId(int localSaveId)
		{
			var memberLoadData = _memberValues.SingleOrDefault(value => value.MemberSaveId.LocalSaveId == localSaveId);
			if (memberLoadData != null)
			{
				return memberLoadData.GetDataToUse();
			}
			return null;
		}

		public object GetDataValueBySaveId(int localSaveId)
		{
			var memberLoadData = _memberValues.SingleOrDefault(value => value.MemberSaveId.LocalSaveId == localSaveId);
			if (memberLoadData == null)
			{
				return null;
			}
			return memberLoadData.GetDataToUse();
		}

		public ObjectLoadData(LoadContext context, int id)
		{
			Context = context;
			Id = id;
        }

		public ObjectLoadData(ObjectHeaderLoadData headerLoadData)
		{
			Id = headerLoadData.Id;
			Target = headerLoadData.Target;
			Context = headerLoadData.Context;
			TypeDefinition = headerLoadData.TypeDefinition;
        }

		public void InitializeReaders(SaveEntryFolder saveEntryFolder)
		{
			var binaryReader = saveEntryFolder.GetEntry(new EntryId(-1, TSS.SaveEntryExtension.Basics)).GetBinaryReader();
			_saveId = TSSD.SaveId.ReadSaveIdFrom(binaryReader);
			_propertyCount = binaryReader.ReadShort();
			_childStructCount = binaryReader.ReadShort();
			for (var i = 0; i < _childStructCount; i++)
			{
				var item = new ObjectLoadData(Context, i);
				_childStructs.Add(item);
			}
			foreach (var saveEntry in saveEntryFolder.ChildEntries)
			{
				if (saveEntry.Id.Extension == TSS.SaveEntryExtension.Property)
				{
					var binaryReader2 = saveEntry.GetBinaryReader();
					var item2 = new PropertyLoadData(this, binaryReader2);
					_propertyValues.Add(item2);
					_memberValues.Add(item2);
				}
				else if (saveEntry.Id.Extension == TSS.SaveEntryExtension.Field)
				{
					var binaryReader3 = saveEntry.GetBinaryReader();
					var item3 = new FieldLoadData(this, binaryReader3);
					_fieldValues.Add(item3);
					_memberValues.Add(item3);
				}
			}
			for (var j = 0; j < _childStructCount; j++)
			{
				var objectLoadData = _childStructs[j];
				var childFolder = saveEntryFolder.GetChildFolder(new FolderId(j, TSS.SaveFolderExtension.Struct));
				objectLoadData.InitializeReaders(childFolder);
			}
		}

		public void CreateStruct()
		{
			TypeDefinition = (Context.DefinitionContext.TryGetTypeDefinition(_saveId) as TSSD.TypeDefinition);
			if (TypeDefinition != null)
			{
				var type = TypeDefinition.Type;
				Target = FormatterServices.GetUninitializedObject(type);
			}
            foreach (var objectLoadData in _childStructs)
			{
				objectLoadData.CreateStruct();
			}
		}

		public void FillCreatedObject()
		{
			foreach (var objectLoadData in _childStructs)
			{
				objectLoadData.CreateStruct();
			}
		}

		public void Read()
		{
			foreach (var objectLoadData in _childStructs)
			{
				objectLoadData.Read();
			}
			foreach (var memberLoadData in _memberValues)
			{
				memberLoadData.Read();
				if (memberLoadData.SavedMemberType == SavedMemberType.CustomStruct)
				{
					var index = (int)memberLoadData.Data;
					var target = _childStructs[index].Target;
					memberLoadData.SetCustomStructData(target);
				}
			}
		}

		public void FillObject()
		{
			foreach (var objectLoadData in _childStructs)
			{
				objectLoadData.FillObject();
			}
			foreach (var fieldLoadData in _fieldValues)
			{
				fieldLoadData.FillObject();
			}
			foreach (var propertyLoadData in _propertyValues)
			{
				propertyLoadData.FillObject();
			}
		}
    }
}