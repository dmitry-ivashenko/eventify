using System;
using System.Collections.Generic;
using UniRx;

namespace Eventify.Core.Runtime
{
    public class ProfilerEventParam : BaseEvent<ProfilerEvent>
    {
        public ProfilerEventParam(ProfilerEvent type) : base(type) { }
    
        public T1 Get<T1>() => ((ProfilerEventParam<T1>) this).Data;
    }

    public class ProfilerEventParam<T1> : ProfilerEventParam
    {
        public T1 Data;

        public ProfilerEventParam(ProfilerEvent type, T1 data) : base(type)
        {
            Data = data;
        }
    }
    
    public static partial class Streams
    {
        public static List<CallInfo<ProfilerEventParam>> ProfilerCallInfo = new List<CallInfo<ProfilerEventParam>>();
        public static List<CallInfo<ProfilerEvent>> ProfilerEnumCallInfo = new List<CallInfo<ProfilerEvent>>();
        private static ReactiveCommand<ProfilerEventParam> Profiler { get; } = new ReactiveCommand<ProfilerEventParam>();
        private static ReactiveCommand<ProfilerEvent> ProfilerEnum { get; } = new ReactiveCommand<ProfilerEvent>();
#if STREAMS_DEBUG
        public static void Raise(this ProfilerEvent evnt) => ProfilerEnumCallInfo.Add(new CallInfo<ProfilerEvent>(ProfilerEnum, evnt));
        public static void Raise<T>(this ProfilerEvent evnt, T data) => ProfilerCallInfo.Add(new CallInfo<ProfilerEventParam>(Profiler, new ProfilerEventParam<T>(evnt, data)));
#else
        public static void Raise(this ProfilerEvent evnt) => ProfilerEnum.Execute(evnt);
        public static void Raise<T>(this ProfilerEvent evnt, T data) => Profiler.Execute(new ProfilerEventParam<T>(evnt, data));
#endif
        public static IDisposable Subscribe(this ProfilerEvent evntType, Action action) => ProfilerEnum.Where(evnt => evnt == evntType).Subscribe(evnt => action());
        public static IDisposable Subscribe<T>(this ProfilerEvent evntType, Action<T> action) => Profiler.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T>()));
        
        public static IObservable<T> Where<T>(this ProfilerEvent evntType, Func<T, bool> predicate)
        {
            return Profiler.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T>())).Select(evnt => evnt.Get<T>());
        }
    }
}
