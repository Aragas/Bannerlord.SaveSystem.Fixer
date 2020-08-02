using System;

using Bannerlord.SaveSystem.Extensions;
using Bannerlord.SaveSystem.Load;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem.LoadData
{
    public class ContainerHeaderLoadData
    {
        public int Id { get; }
        public LoadContext Context { get; }

        public object Target { get; private set; }

        public TSSD.ContainerDefinition TypeDefinition { get; private set; }

        public TSSD.SaveId SaveId { get; private set; }

        public int ElementCount { get; private set; }

        public TSS.ContainerType ContainerType { get; private set; }

        public ContainerHeaderLoadData(LoadContext context, int id)
        {
            Id = id;
            Context = context;
        }

        public bool GetObjectTypeDefinition()
        {
            TypeDefinition = Context.DefinitionContext.TryGetTypeDefinition(SaveId) as TSSD.ContainerDefinition;
            return TypeDefinition != null;
        }

        public void CreateObject()
        {
            var type = TypeDefinition.Type;
            Target = ContainerType == TSS.ContainerType.Array
                ? Activator.CreateInstance(type, new object[] { ElementCount })
                : Activator.CreateInstance(type);
        }

        // PATCH
        public void InitializeReaders(SaveEntryFolder saveEntryFolder) 
        // PATCH
        {
            var binaryReader = saveEntryFolder.GetEntry(new EntryId(-1, TSS.SaveEntryExtension.Object)).GetBinaryReader();
            SaveId = TSSD.SaveId.ReadSaveIdFrom(binaryReader);
            ContainerType = (TSS.ContainerType) binaryReader.ReadByte();
            ElementCount = binaryReader.ReadInt();
        }
    }
}