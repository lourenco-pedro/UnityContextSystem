using System.Collections.Generic;
using System.Linq;

namespace ContextSystem
{
    public abstract class BaseContext : IContext
    {
        private readonly Dictionary<string, IContextData> _contextDatas = new Dictionary<string, IContextData>();

        public abstract void Start(ContextArgs args);
        public abstract void Update(ContextArgs args);
        public abstract void Dispose();

        public void RegisterContexData(params IContextData[] contextData)
        {
            foreach (IContextData data in contextData)
            {
                string name = data.GetType().Name;
                _contextDatas[name] = data;
            }
        }
        
        public IContextData[] GetContextDatas()
        {
            return _contextDatas.Values.ToArray();
        }
    }
}