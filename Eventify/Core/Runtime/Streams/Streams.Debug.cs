using System;
using System.Collections.Generic;
using UniRx;

namespace Eventify.Core.Runtime
{
    public class DebugEventParam : BaseEvent<DebugEvent>
    {
        public DebugEventParam(DebugEvent type) : base(type) { }
    
        public T1 Get<T1>() => ((DebugEventParam<T1>) this).Data;
    }

    public class DebugEventParam<T1> : DebugEventParam
    {
        public T1 Data;

        public DebugEventParam(DebugEvent type, T1 data) : base(type)
        {
            Data = data;
        }
    }
    
    public static partial class Streams
    {
        public static List<CallInfo<DebugEventParam>> DebugCallInfo = new List<CallInfo<DebugEventParam>>();
        public static List<CallInfo<DebugEvent>> DebugEnumCallInfo = new List<CallInfo<DebugEvent>>();
        private static ReactiveCommand<DebugEventParam> Debug { get; } = new ReactiveCommand<DebugEventParam>();
        private static ReactiveCommand<DebugEvent> DebugEnum { get; } = new ReactiveCommand<DebugEvent>();
        private static readonly Queue<DebugEventParam> DebugQueue = new Queue<DebugEventParam>();
        private static readonly Queue<DebugEvent> DebugEnumQueue = new Queue<DebugEvent>();
        private static readonly Queue<CallInfo<DebugEventParam>> DebugQueueCallInfo = new Queue<CallInfo<DebugEventParam>>();
        private static readonly Queue<CallInfo<DebugEvent>> DebugEnumQueueCallInfo = new Queue<CallInfo<DebugEvent>>();

#if STREAMS_DEBUG
        public static void Raise(this DebugEvent evnt) => DebugEnumCallInfo.Add(new CallInfo<DebugEvent>(DebugEnum, evnt));
        public static void Raise<T>(this DebugEvent evnt, T data) => DebugCallInfo.Add(new CallInfo<DebugEventParam>(Debug, new DebugEventParam<T>(evnt, data)));
#else
        public static void Raise(this DebugEvent evnt) => DebugEnum.Execute(evnt);
        public static void Raise<T>(this DebugEvent evnt, T data) => Debug.Execute(new DebugEventParam<T>(evnt, data));
#endif
    
        public static IDisposable Subscribe(this DebugEvent evntType, Action action)
        {
            return DebugEnum.Where(evnt => evnt == evntType).Subscribe(evnt => action());
        }

        public static IDisposable Subscribe<T>(this DebugEvent evntType, Action<T> action)
        {
            return Debug.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T>()));
        }

        public static IObservable<T> Where<T>(this DebugEvent evntType, Func<T, bool> predicate)
        {
            return Debug.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T>())).Select(evnt => evnt.Get<T>());
        }

        public static IObservable<DebugEvent> AsObservable(this DebugEvent evntType)
        {
            return DebugEnum.Where(evnt => evnt == evntType);
        }
    }
}
