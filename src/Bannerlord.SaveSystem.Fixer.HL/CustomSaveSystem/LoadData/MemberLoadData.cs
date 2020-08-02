using TaleWorlds.Library;

namespace Bannerlord.SaveSystem.LoadData
{
    internal abstract class MemberLoadData : VariableLoadData
    {
        public ObjectLoadData ObjectLoadData { get; }

        protected MemberLoadData(ObjectLoadData objectLoadData, IReader reader) : base(objectLoadData.Context, reader)
        {
            ObjectLoadData = objectLoadData;
        }
    }
}