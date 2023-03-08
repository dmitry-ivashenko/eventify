#if PHOTON_UNITY_NETWORKING

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UniRx;
// ReSharper disable UnusedMember.Global

namespace Eventify.Core.Runtime
{
    public class NetMasterEventParam : BaseEvent<NetMasterEvent>
    {
        public NetMasterEventParam(NetMasterEvent type) : base(type) { }
    
        public T Get<T>()
        {
            if (this is NetMasterEventParam<object> param)
            {
                return (T) param.Data;
            }
            
            return ((NetMasterEventParam<T>) this).Data;
        }
    }

    public class NetMasterEventParam<T> : NetMasterEventParam
    {
        public T Data;

        public NetMasterEventParam(NetMasterEvent type, T data) : base(type)
        {
            Data = data;
        }
    }
    
    public static partial class Streams
    {
        public static List<CallInfo<NetMasterEventParam>> NetMasterCallInfo = new List<CallInfo<NetMasterEventParam>>();
        public static List<CallInfo<NetMasterEvent>> NetMasterEnumCallInfo = new List<CallInfo<NetMasterEvent>>();
        public static ReactiveCommand<NetMasterEventParam> NetMaster { get; } = new ReactiveCommand<NetMasterEventParam>();
        public static ReactiveCommand<NetMasterEvent> NetMasterEnum { get; } = new ReactiveCommand<NetMasterEvent>();
        public static readonly Queue<NetMasterEventParam> NetMasterQueue = new Queue<NetMasterEventParam>();
        public static readonly Queue<NetMasterEvent> NetMasterEnumQueue = new Queue<NetMasterEvent>();
        public static readonly Queue<CallInfo<NetMasterEventParam>> NetMasterQueueCallInfo = new Queue<CallInfo<NetMasterEventParam>>();
        public static readonly Queue<CallInfo<NetMasterEvent>> NetMasterEnumQueueCallInfo = new Queue<CallInfo<NetMasterEvent>>();

        public static void Raise(this NetMasterEvent evnt)
        {
            PhotonNetwork.RaiseEvent((byte) evnt, null, NetworkUtils.GetReceiverGroup((int) evnt), SendOptions.SendReliable);
        }
        
        public static void Raise<T>(this NetMasterEvent evnt, T data)
        {
            PhotonNetwork.RaiseEvent((byte) evnt, data, NetworkUtils.GetReceiverGroup((int) evnt), SendOptions.SendReliable);
        }

        private static void RaiseLocal(NetMasterEvent evnt)
        {
            if (DebugMode())
            {
                NetMasterEnumQueueCallInfo.Enqueue(new CallInfo<NetMasterEvent>(NetMasterEnum, evnt, true));
            }
            else
            {
                NetMasterEnumQueue.Enqueue(evnt);    
            }
        }

        public static void RaiseLocal<T>(NetMasterEvent evnt, T data)
        {
            if (data == null)
            {
                RaiseLocal(evnt);
                return;
            }
            
            if (DebugMode())
            {
                NetMasterQueueCallInfo.Enqueue(new CallInfo<NetMasterEventParam>(NetMaster, new NetMasterEventParam<T>(evnt, data), true) );
            }
            else
            {
                NetMasterQueue.Enqueue(new NetMasterEventParam<T>(evnt, data));
            }
        }

        public static IDisposable Subscribe(this NetMasterEvent evntType, Action action)
        {
            return NetMasterEnum.Where(evnt => evnt == evntType).Subscribe(evnt => action());
        }

        public static IDisposable Subscribe<T>(this NetMasterEvent evntType, Action<T> action)
        {
            return NetMaster.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T>()));
        }
                
        public static IObservable<T> Where<T>(this NetMasterEvent evntType, Func<T, bool> predicate)
        {
            return NetMaster.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T>())).Select(evnt => evnt.Get<T>());
        }
    }
}

#endif