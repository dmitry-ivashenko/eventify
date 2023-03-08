using System;
using System.Collections.Generic;
using UniRx;
// ReSharper disable UnusedMember.Global

namespace Eventify.Core.Runtime
{
    public class NetPlayerEventParam : BaseEvent<NetPlayerEvent>
    {
        public NetPlayerEventParam(NetPlayerEvent type) : base(type) { }
    
        public T Get<T>()
        {
            if (this is NetPlayerEventParam<object> param)
            {
                return (T) param.Data;
            }
            
            return ((NetPlayerEventParam<T>) this).Data;
        }
    }

    public class NetPlayerEventParam<T> : NetPlayerEventParam
    {
        public T Data;

        public NetPlayerEventParam(NetPlayerEvent type, T data) : base(type)
        {
            Data = data;
        }
    }
    
    public static partial class Streams
    {
        public static List<CallInfo<NetPlayerEventParam>> NetPlayerCallInfo = new List<CallInfo<NetPlayerEventParam>>();
        public static List<CallInfo<NetPlayerEvent>> NetPlayerEnumCallInfo = new List<CallInfo<NetPlayerEvent>>();
        public static ReactiveCommand<NetPlayerEventParam> NetPlayer { get; } = new ReactiveCommand<NetPlayerEventParam>();
        public static ReactiveCommand<NetPlayerEvent> NetPlayerEnum { get; } = new ReactiveCommand<NetPlayerEvent>();
        public static readonly Queue<NetPlayerEventParam> NetPlayerQueue = new Queue<NetPlayerEventParam>();
        public static readonly Queue<NetPlayerEvent> NetPlayerEnumQueue = new Queue<NetPlayerEvent>();
        public static readonly Queue<CallInfo<NetPlayerEventParam>> NetPlayerQueueCallInfo = new Queue<CallInfo<NetPlayerEventParam>>();
        public static readonly Queue<CallInfo<NetPlayerEvent>> NetPlayerEnumQueueCallInfo = new Queue<CallInfo<NetPlayerEvent>>();
        
        private static void RaiseLocal(NetPlayerEvent evnt)
        {
            if (DebugMode())
            {
                NetPlayerEnumQueueCallInfo.Enqueue(new CallInfo<NetPlayerEvent>(NetPlayerEnum, evnt, true));
            }
            else
            {
                NetPlayerEnumQueue.Enqueue(evnt);    
            }
        }

        public static void RaiseLocal<T>(NetPlayerEvent evnt, T data)
        {
            if (data == null)
            {
                RaiseLocal(evnt);
                return;
            }
            
            if (DebugMode())
            {
                NetPlayerQueueCallInfo.Enqueue(new CallInfo<NetPlayerEventParam>(NetPlayer, new NetPlayerEventParam<T>(evnt, data), true) );
            }
            else
            {
                NetPlayerQueue.Enqueue(new NetPlayerEventParam<T>(evnt, data));
            }
        }

        public static IDisposable Subscribe(this NetPlayerEvent evntType, Action action)
        {
            return NetPlayerEnum.Where(evnt => evnt == evntType).Subscribe(evnt => action());
        }

        public static IDisposable Subscribe<T>(this NetPlayerEvent evntType, Action<T> action)
        {
            return NetPlayer.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T>()));
        }
                
        public static IObservable<T> Where<T>(this NetPlayerEvent evntType, Func<T, bool> predicate)
        {
            return NetPlayer.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T>())).Select(evnt => evnt.Get<T>());
        }
    }
}
