using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace ContextSystem
{
    public static class ContextManager
    {
        private static IDisposable _disposableEveryUpdate;
        
        private static List<IContext> _stackedContexts = new List<IContext>();
        
        public static string CurrentContext => _stackedContexts.Count == 0 ? string.Empty : _stackedContexts.Last().GetType().Name;
        public static bool Initialized => _disposableEveryUpdate != null;

        private static void ForeachUpdateOfStackedContexts()
        {
            Dictionary<string, IContextData> accumulatedContextDatas = new Dictionary<string, IContextData>();
            
            foreach (var ctx in _stackedContexts)
            {
                try
                {
                    accumulatedContextDatas.AddRange(
                            ctx.GetContextDatas()
                            .ToDictionary(data => data.GetType().Name, data => data));
                }
                catch(Exception e)
                {
                    Debug.LogError("Could not spread context data. Error bellow");
                    Debug.LogException(e);
                }
                
                ctx.Update(new ContextArgs(accumulatedContextDatas));
            }
        }
        
        public static void Tick()
        {
            _disposableEveryUpdate = Observable
                .EveryUpdate(UnityFrameProvider.Update)
                .Subscribe(_ => ForeachUpdateOfStackedContexts());
        }

        public static void Terminate()
        {
            _disposableEveryUpdate?.Dispose();
        }
        
        public static void SwitchContext<TContextModel>(IContextData contextData = null)
        where TContextModel : IContext, new()
        {
            PopContext();
            PushContext<TContextModel>(contextData);
        }

        public static void PushContext<TContextModel>(IContextData contextData = null)
        where TContextModel : IContext, new()
        {
            try
            {
                Dictionary<string, IContextData> accumulatedContextDatas = new Dictionary<string, IContextData>();
                foreach (var ctx in _stackedContexts)
                {
                    accumulatedContextDatas.AddRange(ctx.GetContextDatas().ToDictionary(data => data.GetType().Name, data => data));
                }
                
                TContextModel context = new TContextModel();

                if (null != contextData)
                {
                    context.RegisterContexData(contextData);
                    accumulatedContextDatas.Add(contextData.GetType().Name, contextData);
                }

                context.Start(new ContextArgs(accumulatedContextDatas));
                
                _stackedContexts.Add(context);
            }
            catch(Exception e)
            {
                Debug.LogError("Could not spread context data. Error bellow");
                Debug.LogException(e);
            }
            
        }

        public static void PopContext()
        {
            if(_stackedContexts.Count == 0)
                return;
            
            _stackedContexts.Last().Dispose();
            _stackedContexts.RemoveAt(_stackedContexts.Count - 1);
        }
    }
}