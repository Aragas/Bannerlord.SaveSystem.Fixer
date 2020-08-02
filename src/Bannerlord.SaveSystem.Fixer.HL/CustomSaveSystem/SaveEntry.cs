using TaleWorlds.Library;

namespace Bannerlord.SaveSystem
{
    public class SaveEntry
    {
        public static SaveEntry CreateFrom(int entryFolderId, EntryId entryId, byte[] data) => new SaveEntry { FolderId = entryFolderId, Id = entryId, Data = data };
        public static SaveEntry CreateNew(SaveEntryFolder parentFolder, EntryId entryId) => new SaveEntry { Id = entryId, FolderId = parentFolder.GlobalId };

        public EntryId Id { get; private set; }
        public int FolderId { get; private set; }
        public byte[]? Data { get; private set; }
        
        public BinaryReader GetBinaryReader() => new BinaryReader(Data);

        public void FillFrom(BinaryWriter writer) => Data = writer.Data;
    }
}