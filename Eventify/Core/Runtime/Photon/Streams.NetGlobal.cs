#if PHOTON_UNITY_NETWORKING


using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UniRx;
// ReSharper disable UnusedMember.Global

namespace Eventify.Core.Runtime
{
    public class NetGlobalEventParam : BaseEvent<NetGlobalEvent>
    {
        public NetGlobalEventParam(NetGlobalEvent type) : base(type) { }
    
        public T Get<T>()
        {
            if (this is NetGlobalEventParam<object> param)
            {
                return (T) param.Data;
            }
            
            return ((NetGlobalEventParam<T>) this).Data;
        }
    }

    public class NetGlobalEventParam<T> : NetGlobalEventParam
    {
        public T Data;

        public NetGlobalEventParam(NetGlobalEvent type, T data) : base(type)
        {
            Data = data;
        }
    }
    
    public static partial class Streams
    {
        public static List<CallInfo<NetGlobalEventParam>> NetGlobalCallInfo = new List<CallInfo<NetGlobalEventParam>>();
        public static List<CallInfo<NetGlobalEvent>> NetGlobalEnumCallInfo = new List<CallInfo<NetGlobalEvent>>();
        public static ReactiveCommand<NetGlobalEventParam> NetGlobal { get; } = new ReactiveCommand<NetGlobalEventParam>();
        public static ReactiveCommand<NetGlobalEvent> NetGlobalEnum { get; } = new ReactiveCommand<NetGlobalEvent>();
        public static readonly Queue<NetGlobalEventParam> NetGlobalQueue = new Queue<NetGlobalEventParam>();
        public static readonly Queue<NetGlobalEvent> NetGlobalEnumQueue = new Queue<NetGlobalEvent>();
        public static readonly Queue<CallInfo<NetGlobalEventParam>> NetGlobalQueueCallInfo = new Queue<CallInfo<NetGlobalEventParam>>();
        public static readonly Queue<CallInfo<NetGlobalEvent>> NetGlobalEnumQueueCallInfo = new Queue<CallInfo<NetGlobalEvent>>();

        public static void Raise(this NetGlobalEvent evnt)
        {
            PhotonNetwork.RaiseEvent((byte) evnt, null, NetworkUtils.GetReceiverGroup((int) evnt), SendOptions.SendReliable);
        }
        
        public static void Raise<T>(this NetGlobalEvent evnt, T data)
        {
            PhotonNetwork.RaiseEvent((byte) evnt, data, NetworkUtils.GetReceiverGroup((int) evnt), SendOptions.SendReliable);
        }

        private static void RaiseLocal(NetGlobalEvent evnt)
        {
            if (DebugMode())
            {
                NetGlobalEnumQueueCallInfo.Enqueue(new CallInfo<NetGlobalEvent>(NetGlobalEnum, evnt, true));
            }
            else
            {
                NetGlobalEnumQueue.Enqueue(evnt);    
            }
        }

        public static void RaiseLocal<T>(NetGlobalEvent evnt, T data)
        {
            if (data == null)
            {
                RaiseLocal(evnt);
                return;
            }
            
            if (DebugMode())
            {
                NetGlobalQueueCallInfo.Enqueue(new CallInfo<NetGlobalEventParam>(NetGlobal, new NetGlobalEventParam<T>(evnt, data), true) );
            }
            else
            {
                NetGlobalQueue.Enqueue(new NetGlobalEventParam<T>(evnt, data));
            }
        }

        public static IDisposable Subscribe(this NetGlobalEvent evntType, Action action)
        {
            return NetGlobalEnum.Where(evnt => evnt == evntType).Subscribe(evnt => action());
        }

        public static IDisposable Subscribe<T>(this NetGlobalEvent evntType, Action<T> action)
        {
            return NetGlobal.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T>()));
        }
        
        public static IObservable<T> Where<T>(this NetGlobalEvent evntType, Func<T, bool> predicate)
        {
            return NetGlobal.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T>())).Select(evnt => evnt.Get<T>());
        }

    }
}

#endif