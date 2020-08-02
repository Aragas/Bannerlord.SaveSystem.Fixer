using System.Collections.Generic;

using TaleWorlds.Library;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem
{
    internal class ArchiveDeserializer
    {
        public SaveEntryFolder RootFolder { get; } = new SaveEntryFolder(-1, -1, new FolderId(-1, TSS.SaveFolderExtension.Root), 3);

        public void LoadFrom(byte[] binaryArchive)
        {
            var dictionary = new Dictionary<int, SaveEntryFolder>();
            var list = new List<SaveEntry>();
            var binaryReader = new BinaryReader(binaryArchive);
            var num = binaryReader.ReadInt();
            for (var i = 0; i < num; i++)
            {
                var parentGlobalId = binaryReader.Read3ByteInt();
                var globalId = binaryReader.Read3ByteInt();
                var localId = binaryReader.Read3ByteInt();
                var extension = (TSS.SaveFolderExtension)binaryReader.ReadByte();
                var folderId = new FolderId(localId, extension);
                var saveEntryFolder = new SaveEntryFolder(parentGlobalId, globalId, folderId, 3);
                dictionary.Add(saveEntryFolder.GlobalId, saveEntryFolder);
            }
            var num2 = binaryReader.ReadInt();
            for (var j = 0; j < num2; j++)
            {
                var entryFolderId = binaryReader.Read3ByteInt();
                var id = binaryReader.Read3ByteInt();
                var extension2 = (TSS.SaveEntryExtension)binaryReader.ReadByte();
                var length = binaryReader.ReadShort();
                var data = binaryReader.ReadBytes(length);
                var item = SaveEntry.CreateFrom(entryFolderId, new EntryId(id, extension2), data);
                list.Add(item);
            }
            foreach (var saveEntryFolder2 in dictionary.Values)
            {
                if (saveEntryFolder2.ParentGlobalId != -1)
                {
                    dictionary[saveEntryFolder2.ParentGlobalId].AddChildFolderEntry(saveEntryFolder2);
                }
                else
                {
                    RootFolder.AddChildFolderEntry(saveEntryFolder2);
                }
            }
            foreach (var saveEntry in list)
            {
                if (saveEntry.FolderId != -1)
                {
                    dictionary[saveEntry.FolderId].AddEntry(saveEntry);
                }
                else
                {
                    RootFolder.AddEntry(saveEntry);
                }
            }
        }
    }
}