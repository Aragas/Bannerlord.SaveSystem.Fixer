using System;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem
{
    public struct EntryId : IEquatable<EntryId>
    {
        public int Id { get; }
        public TSS.SaveEntryExtension Extension { get; }

        public EntryId(int id, TSS.SaveEntryExtension extension)
        {
            Id = id;
            Extension = extension;
        }

        public override bool Equals(object obj) => obj is EntryId entryId && entryId.Id == Id && entryId.Extension == Extension;
        public bool Equals(EntryId other) => other.Id == Id && other.Extension == Extension;

        public override int GetHashCode()
        {
            var num1 = Id;
            var num2 = num1.GetHashCode() * 397;
            num1 = (int) Extension;
            var hashCode = num1.GetHashCode();
            return num2 ^ hashCode;
        }

        public static bool operator ==(EntryId a, EntryId b) => a.Id == b.Id && a.Extension == b.Extension;
        public static bool operator !=(EntryId a, EntryId b) => !(a == b);
    }
}