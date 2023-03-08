#if PHOTON_UNITY_NETWORKING

using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Eventify.Core.Runtime
{
    public static class NetworkUtils
    {
        public static readonly RaiseEventOptions OptionsMaster = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.MasterClient,
            CachingOption = EventCaching.DoNotCache,
        };

        public static readonly RaiseEventOptions OptionsOthers = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.DoNotCache,
        };

        public static readonly RaiseEventOptions OptionsGlobal = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.DoNotCache,
        };

        public static void RoutePhotonEvent(EventData data)
        {
            if (data.Code >= 200) return;

            if (IsNetMasterEvent(data.Code))
            {
                var evnt = (NetMasterEvent) data.Code;
                Streams.RaiseLocal(evnt, data.CustomData);
            }
            else if (IsNetOpponentEvent(data.Code))
            {
                var evnt = (NetOpponentEvent) data.Code;
                Streams.RaiseLocal(evnt, data.CustomData);
            }
            else if (IsNetPlayerEvent(data.Code))
            {
                var evnt = (NetPlayerEvent) data.Code;
                Streams.RaiseLocal(evnt, data.CustomData);
            }
            else if (IsNetGlobalEvent(data.Code))
            {
                var evnt = (NetGlobalEvent) data.Code;
                Streams.RaiseLocal(evnt, data.CustomData);
            }
        }

        public static RaiseEventOptions GetReceiverGroup(int code)
        {
            if (IsNetMasterEvent(code))
            {
                return OptionsMaster;
            }

            if (IsNetOpponentEvent(code))
            {
                return PhotonNetwork.IsMasterClient ? OptionsOthers : OptionsMaster;
            }

            return OptionsGlobal;
        }

        public static bool IsNetMasterEvent(int code)
        {
            return code.InRange((int) NetMasterEvent.Undefined, (int) NetOpponentEvent.Undefined - 1);
        }

        public static bool IsNetOpponentEvent(int code)
        {
            return code.InRange((int) NetOpponentEvent.Undefined, (int) NetPlayerEvent.Undefined - 1);
        }

        public static bool IsNetPlayerEvent(int code)
        {
            return code.InRange((int) NetPlayerEvent.Undefined, (int) NetGlobalEvent.Undefined - 1);
        }

        public static bool IsNetGlobalEvent(int code)
        {
            return !IsNetMasterEvent(code) && !IsNetOpponentEvent(code) && ! IsNetPlayerEvent(code);
        }
    }
}

#endif