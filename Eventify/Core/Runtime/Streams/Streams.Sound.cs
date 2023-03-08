using System;
using System.Collections.Generic;
using UniRx;

namespace Eventify.Core.Runtime
{
    public class SoundEventParam : BaseEvent<SoundEvent>
    {
        public SoundEventParam(SoundEvent type) : base(type) { }
    
        public T1 Get<T1>() => ((SoundEventParam<T1>) this).Data;
        public T2 Get<T1, T2>() => ((SoundEventParam<T1, T2>) this).Data2;
    }

    public class SoundEventParam<T1> : SoundEventParam
    {
        public T1 Data;

        public SoundEventParam(SoundEvent type, T1 data) : base(type)
        {
            Data = data;
        }
    }
    
    public class SoundEventParam<T1, T2> : SoundEventParam<T1>
    {
        public T2 Data2;

        public SoundEventParam(SoundEvent type, T1 data, T2 data2) : base(type, data)
        {
            Data2 = data2;
        }
    }
    
    public static partial class Streams
    {
        public static List<CallInfo<SoundEventParam>> SoundCallInfo = new List<CallInfo<SoundEventParam>>();
        public static List<CallInfo<SoundEvent>> SoundEnumCallInfo = new List<CallInfo<SoundEvent>>();
        private static ReactiveCommand<SoundEventParam> Sound { get; } = new ReactiveCommand<SoundEventParam>();
        private static ReactiveCommand<SoundEvent> SoundEnum { get; } = new ReactiveCommand<SoundEvent>();
        private static readonly Queue<SoundEventParam> SoundQueue = new Queue<SoundEventParam>();
        private static readonly Queue<SoundEvent> SoundEnumQueue = new Queue<SoundEvent>();
        private static readonly Queue<CallInfo<SoundEventParam>> SoundQueueCallInfo = new Queue<CallInfo<SoundEventParam>>();
        private static readonly Queue<CallInfo<SoundEvent>> SoundEnumQueueCallInfo = new Queue<CallInfo<SoundEvent>>();

#if STREAMS_DEBUG
        public static void Raise(this SoundEvent evnt) => SoundEnumCallInfo.Add(new CallInfo<SoundEvent>(SoundEnum, evnt));
        public static void Raise<T1>(this SoundEvent evnt, T1 data) => SoundCallInfo.Add(new CallInfo<SoundEventParam>(Sound, new SoundEventParam<T1>(evnt, data)));
        public static void Raise<T1, T2>(this SoundEvent evnt, T1 data, T2 data2) => SoundCallInfo.Add(new CallInfo<SoundEventParam>(Sound, new SoundEventParam<T1, T2>(evnt, data, data2)));
#else
        public static void Raise(this SoundEvent evnt) => SoundEnum.Execute(evnt);
        public static void Raise<T1>(this SoundEvent evnt, T1 data) => Sound.Execute(new SoundEventParam<T1>(evnt, data));
#endif
    
        public static IDisposable Subscribe(this SoundEvent evntType, Action action)
        {
            return SoundEnum.Where(evnt => evnt == evntType).Subscribe(evnt => action());
        }

        public static IDisposable Subscribe<T1>(this SoundEvent evntType, Action<T1> action)
        {
            return Sound.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T1>()));
        }
        
        public static IDisposable Subscribe<T1, T2>(this SoundEvent evntType, Action<T1, T2> action)
        {
            return Sound.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T1>(), evnt.Get<T1, T2>()));
        }

        public static IObservable<T1> Where<T1>(this SoundEvent evntType, Func<T1, bool> predicate)
        {
            return Sound.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T1>())).Select(evnt => evnt.Get<T1>());
        }
        
        public static IObservable<T2> Where<T1, T2>(this SoundEvent evntType, Func<T2, bool> predicate)
        {
            return Sound.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T1, T2>())).Select(evnt => evnt.Get<T1, T2>());
        }

        public static IObservable<SoundEvent> AsObservable(this SoundEvent evntType)
        {
            return SoundEnum.Where(evnt => evnt == evntType);
        }
    }
}