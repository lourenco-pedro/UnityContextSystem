namespace ContextSystem
{
    public interface IContext
    {
        void RegisterContexData(params IContextData[] data);
        IContextData[] GetContextDatas();
        void Start(ContextArgs args);
        void Update(ContextArgs args);
        void Dispose();
    }
}