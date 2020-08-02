using Bannerlord.SaveSystem.Extensions;
using Bannerlord.SaveSystem.Load;

using System.Runtime.Serialization;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.LoadData
{
    public class ObjectHeaderLoadData
    {
        private TSSD.SaveId _saveId;

        public int Id { get; }

        public object LoadedObject { get; private set; }

        public object Target { get; private set; }

        public int PropertyCount { get; private set; }

        public int ChildStructCount { get; private set; }

        public TSSD.TypeDefinition TypeDefinition { get; private set; }

        public LoadContext Context { get; }

        public ObjectHeaderLoadData(LoadContext context, int id)
        {
            Context = context;
            Id = id;
        }

        public void InitialieReaders(SaveEntryFolder saveEntryFolder)
        {
            var binaryReader = saveEntryFolder.GetEntry(new EntryId(-1, TSS.SaveEntryExtension.Basics)).GetBinaryReader();
            _saveId = TSSD.SaveId.ReadSaveIdFrom(binaryReader);
            PropertyCount = binaryReader.ReadShort();
            ChildStructCount = binaryReader.ReadShort();
        }

        public void CreateObject()
        {
            TypeDefinition = Context.DefinitionContext.TryGetTypeDefinition(_saveId) as TSSD.TypeDefinition;
            if (TypeDefinition == null)
                return;
            LoadedObject = FormatterServices.GetUninitializedObject(TypeDefinition.Type);
            Target = LoadedObject;
        }

        public void AdvancedResolveObject(TSS.MetaData metaData, ObjectLoadData objectLoadData)
        {
            // PATCH
            ;
            //Target = TypeDefinition.ObjectResolver.AdvancedResolveObject(LoadedObject, metaData, objectLoadData);
        }

        public void ResolveObject()
        {
            Target = TypeDefinition.ObjectResolver.ResolveObject(LoadedObject);
        }
    }
}