using System;
using System.Collections.Generic;
using UniRx;
// ReSharper disable UnusedMember.Global

namespace Eventify.Core.Runtime
{
    public class ServerEventParam : BaseEvent<ServerEvent>
    {
        public ServerEventParam(ServerEvent type) : base(type) { }
    
        public T1 Get<T1>() => ((ServerEventParam<T1>) this).Data;
    }

    public class ServerEventParam<T1> : ServerEventParam
    {
        public T1 Data;

        public ServerEventParam(ServerEvent type, T1 data) : base(type)
        {
            Data = data;
        }
    }
    
    public static partial class Streams
    {
        public static List<CallInfo<ServerEventParam>> ServerCallInfo = new List<CallInfo<ServerEventParam>>();
        public static List<CallInfo<ServerEvent>> ServerEnumCallInfo = new List<CallInfo<ServerEvent>>();
        private static ReactiveCommand<ServerEventParam> Server { get; } = new ReactiveCommand<ServerEventParam>();
        private static ReactiveCommand<ServerEvent> ServerEnum { get; } = new ReactiveCommand<ServerEvent>();
        private static readonly Queue<ServerEventParam> ServerQueue = new Queue<ServerEventParam>();
        private static readonly Queue<ServerEvent> ServerEnumQueue = new Queue<ServerEvent>();
        private static readonly Queue<CallInfo<ServerEventParam>> ServerQueueCallInfo = new Queue<CallInfo<ServerEventParam>>();
        private static readonly Queue<CallInfo<ServerEvent>> ServerEnumQueueCallInfo = new Queue<CallInfo<ServerEvent>>();
#if STREAMS_DEBUG
        public static void Raise(this ServerEvent evnt) => ServerEnumQueueCallInfo.Enqueue(new CallInfo<ServerEvent>(ServerEnum, evnt, true));
        public static void Raise<T>(this ServerEvent evnt, T data) => ServerQueueCallInfo.Enqueue(new CallInfo<ServerEventParam>(Server, new ServerEventParam<T>(evnt, data), true) );
#else
        public static void Raise(this ServerEvent evnt) => ServerEnumQueue.Enqueue(evnt);
        public static void Raise<T>(this ServerEvent evnt, T data) => ServerQueue.Enqueue(new ServerEventParam<T>(evnt, data));
#endif
        public static IDisposable Subscribe(this ServerEvent evntType, Action action) => ServerEnum.Where(evnt => evnt == evntType).Subscribe(evnt => action());
        public static IDisposable Subscribe<T>(this ServerEvent evntType, Action<T> action) => Server.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T>()));
        
        public static IObservable<T> Where<T>(this ServerEvent evntType, Func<T, bool> predicate)
        {
            return Server.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T>())).Select(evnt => evnt.Get<T>());
        }
    }
}