using Bannerlord.SaveSystem.LoadData;

using System;

using TaleWorlds.Library;

using TSS = TaleWorlds.SaveSystem;

namespace Bannerlord.SaveSystem.Load
{
    internal class LoadCallbackInitializator
    {
        private readonly ObjectHeaderLoadData[] _objectHeaderLoadDatas;
        private readonly int _objectCount;
        private readonly TSS.LoadData _loadData;

        public LoadCallbackInitializator(TSS.LoadData loadData, ObjectHeaderLoadData[] objectHeaderLoadDatas, int objectCount)
        {
            _loadData = loadData;
            _objectHeaderLoadDatas = objectHeaderLoadDatas;
            _objectCount = objectCount;
        }

        public void InitializeObjects()
        {
            using (new PerformanceTestBlock("LoadContext::Callbacks"))
            {
                for (var i = 0; i < _objectCount; i++)
                {
                    var objectHeaderLoadData = _objectHeaderLoadDatas[i];
                    if (objectHeaderLoadData.Target == null)
                        continue;

                    var typeDefinition = objectHeaderLoadData.TypeDefinition;
                    var enumerable = typeDefinition?.InitializationCallbacks;
                    if (enumerable == null)
                        continue;

                    foreach (var methodInfo in enumerable)
                    {
                        var parameters = methodInfo.GetParameters();
                        if (parameters.Length > 1 && parameters[1].ParameterType == typeof(TSS.Load.ObjectLoadData))
                        {
                            //var objectLoadData = LoadContext.CreateLoadData(_loadData, i, objectHeaderLoadData);
                            //methodInfo.Invoke(objectHeaderLoadData.Target, new object[] { _loadData.MetaData, objectLoadData });
                        }
                        else
                        {
                            methodInfo.Invoke(objectHeaderLoadData.Target, new object[] { _loadData.MetaData });
                        }
                    }
                }
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}