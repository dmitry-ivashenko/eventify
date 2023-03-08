using System;
using System.Collections.Generic;
using UniRx;
// ReSharper disable UnusedMember.Global

namespace Eventify.Core.Runtime
{
    public class SceneEventParam : BaseEvent<SceneEvent>
    {
        public SceneEventParam(SceneEvent type) : base(type) { }
    
        public T1 Get<T1>() => ((SceneEventParam<T1>) this).Data;
    }

    public class SceneEventParam<T1> : SceneEventParam
    {
        public T1 Data;

        public SceneEventParam(SceneEvent type, T1 data) : base(type)
        {
            Data = data;
        }
    }
    
    public static partial class Streams
    {
        public static List<CallInfo<SceneEventParam>> SceneCallInfo = new List<CallInfo<SceneEventParam>>();
        public static List<CallInfo<SceneEvent>> SceneEnumCallInfo = new List<CallInfo<SceneEvent>>();
        private static ReactiveCommand<SceneEventParam> Scene { get; } = new ReactiveCommand<SceneEventParam>();
        private static ReactiveCommand<SceneEvent> SceneEnum { get; } = new ReactiveCommand<SceneEvent>();
        private static readonly Queue<SceneEventParam> SceneQueue = new Queue<SceneEventParam>();
        private static readonly Queue<SceneEvent> SceneEnumQueue = new Queue<SceneEvent>();
        private static readonly Queue<CallInfo<SceneEventParam>> SceneQueueCallInfo = new Queue<CallInfo<SceneEventParam>>();
        private static readonly Queue<CallInfo<SceneEvent>> SceneEnumQueueCallInfo = new Queue<CallInfo<SceneEvent>>();
#if STREAMS_DEBUG
        public static void Raise(this SceneEvent evnt) => SceneEnumQueueCallInfo.Enqueue(new CallInfo<SceneEvent>(SceneEnum, evnt, true));
        public static void Raise<T>(this SceneEvent evnt, T data) => SceneQueueCallInfo.Enqueue(new CallInfo<SceneEventParam>(Scene, new SceneEventParam<T>(evnt, data), true) );
#else
        public static void Raise(this SceneEvent evnt) => SceneEnumQueue.Enqueue(evnt);
        public static void Raise<T>(this SceneEvent evnt, T data) => SceneQueue.Enqueue(new SceneEventParam<T>(evnt, data));
#endif
        public static IDisposable Subscribe(this SceneEvent evntType, Action action) => SceneEnum.Where(evnt => evnt == evntType).Subscribe(evnt => action());
        public static IDisposable Subscribe<T>(this SceneEvent evntType, Action<T> action) => Scene.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T>()));

        public static IObservable<SceneEventParam> Where(this SceneEvent evntType, Predicate<SceneEvent> predicate)
        {
            return Scene.Where(evnt => evnt.Type == evntType && predicate(evntType));
        }
        
        public static IObservable<SceneEventParam> Where<T>(this SceneEvent evntType, Predicate<T> predicate)
        {
            return Scene.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T>()));
        }
    
        public static IDisposable Subscribe<T>(this IObservable<SceneEventParam> evntType, Action<T> action)
        {
            return evntType.Subscribe(evnt => action(evnt.Get<T>()));
        }        
        
        public static IObservable<T> Where<T>(this SceneEvent evntType, Func<T, bool> predicate)
        {
            return Scene.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T>())).Select(evnt => evnt.Get<T>());
        }
    }
}