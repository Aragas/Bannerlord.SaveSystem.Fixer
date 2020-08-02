using Bannerlord.SaveSystem.Load;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.LoadData
{
    internal class ContainerLoadData
    {
        private readonly TSSD.SaveId _saveId;
        private readonly int _elementCount;
        private readonly TSS.ContainerType _containerType;
        private readonly ElementLoadData[] _keys;
        private readonly ElementLoadData[] _values;
        private readonly Dictionary<int, ObjectLoadData> _childStructs = new Dictionary<int, ObjectLoadData>();

        public int Id => ContainerHeaderLoadData.Id;

        public object Target => ContainerHeaderLoadData.Target;

        public LoadContext Context => ContainerHeaderLoadData.Context;

        public TSSD.ContainerDefinition TypeDefinition => ContainerHeaderLoadData.TypeDefinition;

        public ContainerHeaderLoadData ContainerHeaderLoadData { get; }

        public ContainerLoadData(ContainerHeaderLoadData headerLoadData)
        {
            ContainerHeaderLoadData = headerLoadData;
            _saveId = headerLoadData.SaveId;
            _containerType = headerLoadData.ContainerType;
            _elementCount = headerLoadData.ElementCount;
            _keys = new ElementLoadData[_elementCount];
            _values = new ElementLoadData[_elementCount];
        }

        // PATCH
        private IEnumerable<FolderId> GetChildStructNames(SaveEntryFolder saveEntryFolder)
        // PATCH
        {
            var list = new List<FolderId>();
            foreach (SaveEntryFolder saveEntryFolder2 in saveEntryFolder.ChildFolders)
            {
                if (saveEntryFolder2.FolderId.Extension == TSS.SaveFolderExtension.Struct && !list.Contains(saveEntryFolder2.FolderId))
                {
                    list.Add(saveEntryFolder2.FolderId);
                }
            }
            return list.ToArray();
        }

        // Token: 0x06000144 RID: 324 RVA: 0x00005E8C File Offset: 0x0000408C
        public void InitializeReaders(SaveEntryFolder saveEntryFolder)
        {
            foreach (var folderId in GetChildStructNames(saveEntryFolder))
            {
                var localId = folderId.LocalId;
                var value = new ObjectLoadData(Context, localId);
                _childStructs.Add(localId, value);
            }

            for (var j = 0; j < _elementCount; j++)
            {
                var binaryReader = saveEntryFolder.GetEntry(new EntryId(j, TSS.SaveEntryExtension.Value)).GetBinaryReader();
                var elementLoadData = new ElementLoadData(this, binaryReader);
                _values[j] = elementLoadData;
                if (_containerType == TSS.ContainerType.Dictionary)
                {
                    var binaryReader2 = saveEntryFolder.GetEntry(new EntryId(j, TSS.SaveEntryExtension.Key))
                        .GetBinaryReader();
                    var elementLoadData2 = new ElementLoadData(this, binaryReader2);
                    _keys[j] = elementLoadData2;
                }
            }

            foreach (var keyValuePair in _childStructs)
            {
                var key = keyValuePair.Key;
                var value2 = keyValuePair.Value;
                var childFolder = saveEntryFolder.GetChildFolder(new FolderId(key, TSS.SaveFolderExtension.Struct));
                value2.InitializeReaders(childFolder);
            }
        }

        // Token: 0x06000145 RID: 325 RVA: 0x00005FB8 File Offset: 0x000041B8
        public void FillCreatedObject()
        {
            foreach (var objectLoadData in _childStructs.Values)
            {
                objectLoadData.CreateStruct();
            }
        }

        // Token: 0x06000146 RID: 326 RVA: 0x00006010 File Offset: 0x00004210
        public void Read()
        {
            for (var i = 0; i < _elementCount; i++)
            {
                _values[i].Read();
                if (_containerType == TSS.ContainerType.Dictionary)
                {
                    _keys[i].Read();
                }
            }

            foreach (var objectLoadData in _childStructs.Values)
            {
                objectLoadData.Read();
            }
        }

        private static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().FullName == name);
        }

        public void FillObject()
        {
            foreach (var objectLoadData in _childStructs.Values)
            {
                objectLoadData.FillObject();
            }

            for (var i = 0; i < _elementCount; i++)
            {
                if (_containerType == TSS.ContainerType.List)
                {
                    var list = (IList) Target;
                    var elementLoadData = _values[i];
                    if (elementLoadData.SavedMemberType == SavedMemberType.CustomStruct)
                    {
                        var key = (int) elementLoadData.Data;
                        var objectLoadData2 = _childStructs[key];
                        elementLoadData.SetCustomStructData(objectLoadData2.Target);
                    }

                    var dataToUse = elementLoadData.GetDataToUse();
                    if (list != null)
                    {
                        list.Add(dataToUse);
                    }
                }
                else if (_containerType == TSS.ContainerType.Dictionary)
                {
                    var dictionary = (IDictionary) Target;
                    var elementLoadData2 = _keys[i];
                    var elementLoadData3 = _values[i];
                    if (elementLoadData2.SavedMemberType == SavedMemberType.CustomStruct)
                    {
                        var key2 = (int) elementLoadData2.Data;
                        var objectLoadData3 = _childStructs[key2];
                        elementLoadData2.SetCustomStructData(objectLoadData3.Target);
                    }

                    if (elementLoadData3.SavedMemberType == SavedMemberType.CustomStruct)
                    {
                        var key3 = (int) elementLoadData3.Data;
                        var objectLoadData4 = _childStructs[key3];
                        elementLoadData3.SetCustomStructData(objectLoadData4.Target);
                    }

                    var dataToUse2 = elementLoadData2.GetDataToUse();
                    var dataToUse3 = elementLoadData3.GetDataToUse();
                    if (dictionary != null && dataToUse2 != null)
                    {
                        dictionary.Add(dataToUse2, dataToUse3);
                    }
                }
                else if (_containerType == TSS.ContainerType.Array)
                {
                    var array = (Array) Target;
                    var elementLoadData4 = _values[i];
                    if (elementLoadData4.SavedMemberType == SavedMemberType.CustomStruct)
                    {
                        var key4 = (int) elementLoadData4.Data;
                        var objectLoadData5 = _childStructs[key4];
                        elementLoadData4.SetCustomStructData(objectLoadData5.Target);
                    }

                    var dataToUse4 = elementLoadData4.GetDataToUse();
                    array.SetValue(dataToUse4, i);
                }
                else if (_containerType == TSS.ContainerType.Queue)
                {
                    var collection = (ICollection) Target;
                    var elementLoadData5 = _values[i];
                    if (elementLoadData5.SavedMemberType == SavedMemberType.CustomStruct)
                    {
                        var key5 = (int) elementLoadData5.Data;
                        var objectLoadData6 = _childStructs[key5];
                        elementLoadData5.SetCustomStructData(objectLoadData6.Target);
                    }

                    var dataToUse5 = elementLoadData5.GetDataToUse();
                    collection.GetType().GetMethod("Enqueue").Invoke(collection, new[] { dataToUse5 });
                }
            }
        }
    }
}