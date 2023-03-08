using System;
using System.Collections.Generic;
using UniRx;

namespace Eventify.Core.Runtime
{
    public class SyncEventParam : BaseEvent<SyncEvent>
    {
        public SyncEventParam(SyncEvent type) : base(type) { }
    
        public T1 Get<T1>() => ((SyncEventParam<T1>) this).Data;
    }

    public class SyncEventParam<T1> : SyncEventParam
    {
        public T1 Data;

        public SyncEventParam(SyncEvent type, T1 data) : base(type)
        {
            Data = data;
        }
    }
    
    public static partial class Streams
    {
        public static List<CallInfo<SyncEventParam>> SyncCallInfo = new List<CallInfo<SyncEventParam>>();
        public static List<CallInfo<SyncEvent>> SyncEnumCallInfo = new List<CallInfo<SyncEvent>>();
        private static ReactiveCommand<SyncEventParam> Sync { get; } = new ReactiveCommand<SyncEventParam>();
        private static ReactiveCommand<SyncEvent> SyncEnum { get; } = new ReactiveCommand<SyncEvent>();
#if STREAMS_DEBUG
        public static void Raise(this SyncEvent evnt) => SyncEnumCallInfo.Add(new CallInfo<SyncEvent>(SyncEnum, evnt));
        public static void Raise<T>(this SyncEvent evnt, T data) => SyncCallInfo.Add(new CallInfo<SyncEventParam>(Sync, new SyncEventParam<T>(evnt, data)));
#else
        public static void Raise(this SyncEvent evnt) => SyncEnum.Execute(evnt);
        public static void Raise<T>(this SyncEvent evnt, T data) => Sync.Execute(new SyncEventParam<T>(evnt, data));
#endif
        public static IDisposable Subscribe(this SyncEvent evntType, Action action) => SyncEnum.Where(evnt => evnt == evntType).Subscribe(evnt => action());
        public static IDisposable Subscribe<T>(this SyncEvent evntType, Action<T> action) => Sync.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T>()));
        
        public static IObservable<T> Where<T>(this SyncEvent evntType, Func<T, bool> predicate)
        {
            return Sync.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T>())).Select(evnt => evnt.Get<T>());
        }
    }
}
