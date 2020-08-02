using TaleWorlds.Library;

namespace Bannerlord.SaveSystem.LoadData
{
    internal class ElementLoadData : VariableLoadData
    {
        public ContainerLoadData ContainerLoadData { get; }

        internal ElementLoadData(ContainerLoadData containerLoadData, IReader reader) : base(containerLoadData.Context, reader)
        {
            ContainerLoadData = containerLoadData;
        }
    }
}