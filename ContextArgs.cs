using System;
using System.Collections.Generic;

namespace ContextSystem
{
    public struct ContextArgs
    {
        Dictionary<string, IContextData> _contextDatas;

        public int Count => _contextDatas.Count;
        
        public ContextArgs(Dictionary<string, IContextData> contextData)
        {
            _contextDatas = new Dictionary<string, IContextData>(contextData);
        }

        public void UseContextData<TContextData>(Action<TContextData> onFound)
            where TContextData : IContextData
        {
            if (_contextDatas.TryGetValue(typeof(TContextData).Name, out IContextData contextData))
                onFound.Invoke((TContextData)contextData);
        }
        
        public void UseContextData<TContextData, TContextData2>(Action<TContextData, TContextData2> onFound)
            where TContextData : IContextData
            where TContextData2 : IContextData
        {
            List<IContextData> foundContextData = new List<IContextData>();
            
            UseContextData<TContextData>(found => foundContextData.Add(found));
            UseContextData<TContextData2>(found => foundContextData.Add(found));

            onFound.Invoke((TContextData)foundContextData[0], (TContextData2)foundContextData[1]);
        }
    }
}