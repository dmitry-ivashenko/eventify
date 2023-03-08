#if PHOTON_UNITY_NETWORKING

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UniRx;
// ReSharper disable UnusedMember.Global

namespace Eventify.Core.Runtime
{
    public class NetOpponentEventParam : BaseEvent<NetOpponentEvent>
    {
        public NetOpponentEventParam(NetOpponentEvent type) : base(type) { }
    
        public T Get<T>()
        {
            if (this is NetOpponentEventParam<object> param)
            {
                return (T) param.Data;
            }
            
            return ((NetOpponentEventParam<T>) this).Data;
        }
    }

    public class NetOpponentEventParam<T> : NetOpponentEventParam
    {
        public T Data;

        public NetOpponentEventParam(NetOpponentEvent type, T data) : base(type)
        {
            Data = data;
        }
    }


    public static partial class Streams
    {
        public static List<CallInfo<NetOpponentEventParam>> NetOpponentCallInfo = new List<CallInfo<NetOpponentEventParam>>();
        public static List<CallInfo<NetOpponentEvent>> NetOpponentEnumCallInfo = new List<CallInfo<NetOpponentEvent>>();
        public static ReactiveCommand<NetOpponentEventParam> NetOpponent { get; } = new ReactiveCommand<NetOpponentEventParam>();
        public static ReactiveCommand<NetOpponentEvent> NetOpponentEnum { get; } = new ReactiveCommand<NetOpponentEvent>();
        public static readonly Queue<NetOpponentEventParam> NetOpponentQueue = new Queue<NetOpponentEventParam>();
        public static readonly Queue<NetOpponentEvent> NetOpponentEnumQueue = new Queue<NetOpponentEvent>();
        public static readonly Queue<CallInfo<NetOpponentEventParam>> NetOpponentQueueCallInfo = new Queue<CallInfo<NetOpponentEventParam>>();
        public static readonly Queue<CallInfo<NetOpponentEvent>> NetOpponentEnumQueueCallInfo = new Queue<CallInfo<NetOpponentEvent>>();

        public static void Raise(this NetOpponentEvent evnt)
        {
            PhotonNetwork.RaiseEvent((byte) evnt, null, NetworkUtils.GetReceiverGroup((int) evnt), SendOptions.SendReliable);
        }
        
        public static void Raise<T>(this NetOpponentEvent evnt, T data)
        {
            PhotonNetwork.RaiseEvent((byte) evnt, data, NetworkUtils.GetReceiverGroup((int) evnt), SendOptions.SendReliable);
        }

        private static void RaiseLocal(NetOpponentEvent evnt)
        {
            if (DebugMode())
            {
                NetOpponentEnumQueueCallInfo.Enqueue(new CallInfo<NetOpponentEvent>(NetOpponentEnum, evnt, true));
            }
            else
            {
                NetOpponentEnumQueue.Enqueue(evnt);    
            }
        }

        public static void RaiseLocal<T>(NetOpponentEvent evnt, T data)
        {
            if (data == null)
            {
                RaiseLocal(evnt);
                return;
            }
            
            if (DebugMode())
            {
                NetOpponentQueueCallInfo.Enqueue(new CallInfo<NetOpponentEventParam>(NetOpponent, new NetOpponentEventParam<T>(evnt, data), true) );
            }
            else
            {
                NetOpponentQueue.Enqueue(new NetOpponentEventParam<T>(evnt, data));
            }
        }

        public static IDisposable Subscribe(this NetOpponentEvent evntType, Action action)
        {
            return NetOpponentEnum.Where(evnt => evnt == evntType).Subscribe(evnt => action());
        }

        public static IDisposable Subscribe<T>(this NetOpponentEvent evntType, Action<T> action)
        {
            return NetOpponent.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T>()));
        }
                
        public static IObservable<T> Where<T>(this NetOpponentEvent evntType, Func<T, bool> predicate)
        {
            return NetOpponent.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T>())).Select(evnt => evnt.Get<T>());
        }
    }
}

#endif