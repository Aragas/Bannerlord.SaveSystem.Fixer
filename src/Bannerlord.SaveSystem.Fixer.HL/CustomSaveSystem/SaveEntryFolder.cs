using System.Collections.Generic;
using System.Linq;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem
{
    public class SaveEntryFolder
    {
        public static SaveEntryFolder CreateRootFolder() => new SaveEntryFolder(-1, -1, new FolderId(-1, TSS.SaveFolderExtension.Root), 3);
        

        private readonly Dictionary<EntryId, SaveEntry> _entries;
        private readonly Dictionary<FolderId, SaveEntryFolder> _saveEntryFolders;

        public Dictionary<EntryId, SaveEntry>.ValueCollection ChildEntries => _entries.Values;
        public Dictionary<FolderId, SaveEntryFolder>.ValueCollection ChildFolders => _saveEntryFolders.Values;

        public IEnumerable<SaveEntry> AllEntries
        {
            get
            {
                foreach (var saveEntry in _entries.Values)
                    yield return saveEntry;

                foreach (var saveEntry in _saveEntryFolders.Values.SelectMany(saveEntryFolder => saveEntryFolder.AllEntries))
                    yield return saveEntry;
            }
        }

        public int GlobalId { get; }
        public int ParentGlobalId { get; }
        public FolderId FolderId { get; }

        
        public SaveEntryFolder(SaveEntryFolder parent, int globalId, FolderId folderId, int entryCount) : this(parent.GlobalId, globalId, folderId, entryCount) { }
        public SaveEntryFolder(int parentGlobalId, int globalId, FolderId folderId, int entryCount)
        {
            ParentGlobalId = parentGlobalId;
            GlobalId = globalId;
            FolderId = folderId;
            _entries = new Dictionary<EntryId, SaveEntry>(entryCount);
            _saveEntryFolders = new Dictionary<FolderId, SaveEntryFolder>(3);
        }

        public void AddEntry(SaveEntry saveEntry) => _entries.Add(saveEntry.Id, saveEntry);
        public void AddChildFolderEntry(SaveEntryFolder saveEntryFolder) => _saveEntryFolders.Add(saveEntryFolder.FolderId, saveEntryFolder);

        public SaveEntry GetEntry(EntryId entryId) => _entries[entryId];
        internal SaveEntryFolder GetChildFolder(FolderId folderId) => _saveEntryFolders[folderId];

        public SaveEntry CreateEntry(EntryId entryId)
        {
            var saveEntry = SaveEntry.CreateNew(this, entryId);
            AddEntry(saveEntry);
            return saveEntry;
        }
    }
}