using System;

using TSS = TaleWorlds.SaveSystem;
using TSSD = TaleWorlds.SaveSystem.Definition;

namespace Bannerlord.SaveSystem
{
    public struct FolderId : IEquatable<FolderId>
    {
        public int LocalId { get; }
        public TSS.SaveFolderExtension Extension { get; }

        public FolderId(int localId, TSS.SaveFolderExtension extension)
        {
            LocalId = localId;
            Extension = extension;
        }

        public override bool Equals(object obj) => obj is FolderId folderId && folderId.LocalId == LocalId && folderId.Extension == Extension;
        public bool Equals(FolderId other) => other.LocalId == LocalId && other.Extension == Extension;

        public override int GetHashCode()
        {
            var num1 = LocalId;
            var num2 = num1.GetHashCode() * 397;
            num1 = (int) Extension;
            var hashCode = num1.GetHashCode();
            return num2 ^ hashCode;
        }

        public static bool operator ==(FolderId a, FolderId b) => a.LocalId == b.LocalId && a.Extension == b.Extension;
        public static bool operator !=(FolderId a, FolderId b) => !(a == b);
    }
}