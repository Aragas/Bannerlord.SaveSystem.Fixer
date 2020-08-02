using Bannerlord.SaveSystem.Definitions;
using Bannerlord.SaveSystem.LoadData;

using System;
using System.Globalization;

using TaleWorlds.Library;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.Load
{
    // Token: 0x02000037 RID: 55
    public class LoadContext
    {
        // PATCH
        internal static LoadCallbackInitializator CallbackInitializator { get; private set; }
        // PATCH

        private int _objectCount;
        private int _stringCount;
        private int _containerCount;
        private ObjectHeaderLoadData[]? _objectHeaderLoadDatas;
        private ContainerHeaderLoadData[]? _containerHeaderLoadDatas;
        private string[]? _strings;

        public object? RootObject { get; private set; }

        public TSSD.DefinitionContext DefinitionContext { get; }
        public TSS.ISaveDriver Driver { get; }


        public LoadContext(TSSD.DefinitionContext definitionContext, TSS.ISaveDriver driver)
        {
            DefinitionContext = definitionContext;
            Driver = driver;
        }

        internal static ObjectLoadData CreateLoadData(TSS.LoadData loadData, int i, ObjectHeaderLoadData header)
        {
            var archiveDeserializer = new ArchiveDeserializer();
            archiveDeserializer.LoadFrom(loadData.GameData.ObjectData[i]);
            var rootFolder = archiveDeserializer.RootFolder;
            var objectLoadData = new ObjectLoadData(header);
            var childFolder = rootFolder.GetChildFolder(new FolderId(i, TSS.SaveFolderExtension.Object));
            objectLoadData.InitializeReaders(childFolder);
            objectLoadData.FillCreatedObject();
            objectLoadData.Read();
            objectLoadData.FillObject();
            return objectLoadData;
        }

        public bool Load(TSS.LoadData loadData, bool loadAsLateInitialize)
        {
            var result = false;
            //try
            {
                using (new PerformanceTestBlock("LoadContext::Load Headers"))
                {
                    using (new PerformanceTestBlock("LoadContext::Load And Create Header"))
                    {
                        var archiveDeserializer = new ArchiveDeserializer();
                        archiveDeserializer.LoadFrom(loadData.GameData.Header);
                        var headerRootFolder = archiveDeserializer.RootFolder;
                        var binaryReader = headerRootFolder.GetEntry(new EntryId(-1, TSS.SaveEntryExtension.Config)).GetBinaryReader();
                        _objectCount = binaryReader.ReadInt();
                        _stringCount = binaryReader.ReadInt();
                        _containerCount = binaryReader.ReadInt();
                        _objectHeaderLoadDatas = new ObjectHeaderLoadData[_objectCount];
                        _containerHeaderLoadDatas = new ContainerHeaderLoadData[_containerCount];
                        _strings = new string[_stringCount];
                        //Parallel.For(0, _objectCount, delegate(int i)
                        for (var i = 0; i < _objectCount; i++)
                        {
                            var objectHeaderLoadData = new ObjectHeaderLoadData(this, i);
                            var childFolder = headerRootFolder.GetChildFolder(new FolderId(i, TSS.SaveFolderExtension.Object));
                            objectHeaderLoadData.InitialieReaders(childFolder);
                            _objectHeaderLoadDatas[i] = objectHeaderLoadData;
                        }

                        //);
                        //Parallel.For(0, _containerCount, delegate(int i)
                        for (var i = 0; i < _containerCount; i++)
                        {
                            var containerHeaderLoadData = new ContainerHeaderLoadData(this, i);
                            var childFolder = headerRootFolder.GetChildFolder(new FolderId(i, TSS.SaveFolderExtension.Container));
                            containerHeaderLoadData.InitializeReaders(childFolder);
                            _containerHeaderLoadDatas[i] = containerHeaderLoadData;
                        }

                        //);
                    }

                    using (new PerformanceTestBlock("LoadContext::Create Objects"))
                    {
                        foreach (var objectHeaderLoadData in _objectHeaderLoadDatas)
                        {
                            objectHeaderLoadData.CreateObject();
                            if (objectHeaderLoadData.Id == 0)
                            {
                                RootObject = objectHeaderLoadData.Target;
                            }
                        }

                        foreach (var containerHeaderLoadData in _containerHeaderLoadDatas)
                        {
                            if (containerHeaderLoadData.GetObjectTypeDefinition())
                            {
                                containerHeaderLoadData.CreateObject();
                            }
                        }
                    }
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                using (new PerformanceTestBlock("LoadContext::Load Strings"))
                {
                    var archiveDeserializer = new ArchiveDeserializer();
                    archiveDeserializer.LoadFrom(loadData.GameData.Strings);
                    for (var j = 0; j < _stringCount; j++)
                    {
                        _strings[j] = LoadString(archiveDeserializer, j);
                    }
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                using (new PerformanceTestBlock("LoadContext::Resolve Objects"))
                {
                    for (var k = 0; k < _objectHeaderLoadDatas.Length; k++)
                    {
                        var objectHeaderLoadData = _objectHeaderLoadDatas[k];
                        if (objectHeaderLoadData.TypeDefinition is { } typeDefinition)
                        {
                            var loadedObject = objectHeaderLoadData.LoadedObject;
                            if (typeDefinition.ObjectResolver.CheckIfRequiresAdvancedResolving(loadedObject))
                            {
                                var objectLoadData = CreateLoadData(loadData, k, objectHeaderLoadData);
                                objectHeaderLoadData.AdvancedResolveObject(loadData.MetaData, objectLoadData);
                            }
                            else
                            {
                                objectHeaderLoadData.ResolveObject();
                            }
                        }
                    }
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                using (new PerformanceTestBlock("LoadContext::Load Object Datas"))
                {
                    //Parallel.For(0, _objectCount, delegate(int i)
                    for (var i = 0; i < _objectCount; i++)
                    {
                        var objectHeaderLoadData = _objectHeaderLoadDatas[i];
                        if (objectHeaderLoadData.Target == objectHeaderLoadData.LoadedObject)
                        {
                            CreateLoadData(loadData, i, objectHeaderLoadData);
                        }
                    }

                    //);
                }

                using (new PerformanceTestBlock("LoadContext::Load Container Datas"))
                {
                    //Parallel.For(0, _containerCount, delegate(int i)
                    for (var i = 0; i < _containerCount; i++)
                    {
                        var binaryArchive = loadData.GameData.ContainerData[i];
                        var archiveDeserializer = new ArchiveDeserializer();
                        archiveDeserializer.LoadFrom(binaryArchive);
                        var rootFolder = archiveDeserializer.RootFolder;
                        var containerLoadData = new ContainerLoadData(_containerHeaderLoadDatas[i]);
                        var childFolder = rootFolder.GetChildFolder(new FolderId(i, TSS.SaveFolderExtension.Container));
                        containerLoadData.InitializeReaders(childFolder);
                        containerLoadData.FillCreatedObject();
                        containerLoadData.Read();
                        containerLoadData.FillObject();
                    }

                    //);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (!loadAsLateInitialize)
                {
                    // PATCH
                    //InitializeObjectsMethod.Invoke(CreateLoadCallbackInitializator(loadData), Array.Empty<object>());
                    CreateLoadCallbackInitializator1(loadData).InitializeObjects();
                    // PATCH
                }
                else
                {
                    CallbackInitializator = CreateLoadCallbackInitializator1(loadData);
                }

                result = true;
            }
            //catch (Exception e)
            //{
            //	result = false;
            //}
            return result;
        }

        // PATCH
        /*
        private MethodInfo InitializeObjectsMethod { get; } = AccessTools.Method(
            Type.GetType("TaleWorlds.SaveSystem.Load.LoadCallbackInitializator, TaleWorlds.SaveSystem"),
            "InitializeObjects");
        internal object CreateLoadCallbackInitializator(TaleWorlds.SaveSystem.LoadData loadData) => Activator.CreateInstance(
            Type.GetType("TaleWorlds.SaveSystem.Load.LoadCallbackInitializator, TaleWorlds.SaveSystem"),
            new object[] { loadData, _objectHeaderLoadDatas, _objectCount });
        */
        private LoadCallbackInitializator CreateLoadCallbackInitializator1(TSS.LoadData loadData) => new LoadCallbackInitializator(loadData, _objectHeaderLoadDatas, _objectCount);
        internal object CreateLoadCallbackInitializator(TSS.LoadData loadData) => null!;
        // PATCH

        private static string LoadString(ArchiveDeserializer saveArchive, int id) => saveArchive.RootFolder
            .GetChildFolder(new FolderId(-1, TSS.SaveFolderExtension.Strings))
            .GetEntry(new EntryId(id, TSS.SaveEntryExtension.Txt))
            .GetBinaryReader()
            .ReadString();

        public static bool TryConvertType(Type sourceType, Type targetType, ref object data)
        {
            static bool IsInt(Type type) => type == typeof(long) || type == typeof(int) || type == typeof(short) || type == typeof(ulong) || type == typeof(uint) || type == typeof(ushort);
            static bool IsFloat(Type type) => type == typeof(double) || type == typeof(float);
            static bool Func(Type type) => IsInt(type) || IsFloat(type);

            if (Func(sourceType) && Func(targetType))
            {
                try
                {
                    data = Convert.ChangeType(data, targetType);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            if (Func(sourceType) && targetType == typeof(string))
            {
                if (IsInt(sourceType))
                {
                    data = Convert.ToInt64(data).ToString();
                }
                else if (IsFloat(sourceType))
                {
                    data = Convert.ToDouble(data).ToString(CultureInfo.InvariantCulture);
                }

                return true;
            }

            return false;
        }

        public ObjectHeaderLoadData? GetObjectWithId(int id)
        {
            ObjectHeaderLoadData? result = null;

            // PATCH
            if (_objectHeaderLoadDatas != null && id != -1)
                result = _objectHeaderLoadDatas[id];
            // PATCH

            // PATCH
            if (result?.TypeDefinition is NullTypeDefinition)
                result = null;
            // PATCH

            return result;
        }

        public ContainerHeaderLoadData? GetContainerWithId(int id)
        {
            ContainerHeaderLoadData? result = null;

            // PATCH
            if (_containerHeaderLoadDatas != null && id != -1)
                result = _containerHeaderLoadDatas[id];
            // PATCH

            return result;
        }

        public string? GetStringWithId(int id)
        {
            string? result = null;

            // PATCH
            if (_strings != null && id != -1)
                result = _strings[id];
            // PATCH

            return result;
        }
    }
}