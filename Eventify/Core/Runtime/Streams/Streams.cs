namespace Eventify.Core.Runtime
{
    public static partial class Streams
    {
        public static bool IsQueueEmpty()
        {
            return UiQueue.Count == 0
                   && UiEnumQueue.Count == 0
                   && UiQueueCallInfo.Count == 0
                   && UiEnumQueueCallInfo.Count == 0
                   && DebugQueue.Count == 0
                   && DebugEnumQueue.Count == 0
                   && DebugQueueCallInfo.Count == 0
                   && DebugEnumQueueCallInfo.Count == 0
                   && ServerQueue.Count == 0
                   && ServerEnumQueue.Count == 0
                   && ServerQueueCallInfo.Count == 0
                   && ServerEnumQueueCallInfo.Count == 0
                   && SceneQueue.Count == 0
                   && SceneEnumQueue.Count == 0
                   && SceneQueueCallInfo.Count == 0
                   && SceneEnumQueueCallInfo.Count == 0
                   && SoundQueue.Count == 0
                   && SoundEnumQueue.Count == 0
                   && SoundQueueCallInfo.Count == 0
                   && SoundEnumQueueCallInfo.Count == 0

#if PHOTON_UNITY_NETWORKING
                   && NetMasterQueue.Count == 0
                   && NetMasterEnumQueue.Count == 0
                   && NetMasterQueueCallInfo.Count == 0
                   && NetMasterEnumQueueCallInfo.Count == 0
                   && NetOpponentQueue.Count == 0
                   && NetOpponentEnumQueue.Count == 0
                   && NetOpponentQueueCallInfo.Count == 0
                   && NetOpponentEnumQueueCallInfo.Count == 0
                   && NetGlobalQueue.Count == 0
                   && NetGlobalEnumQueue.Count == 0
                   && NetGlobalQueueCallInfo.Count == 0
                   && NetGlobalEnumQueueCallInfo.Count == 0
                   && NetPlayerQueue.Count == 0
                   && NetPlayerEnumQueue.Count == 0
                   && NetPlayerQueueCallInfo.Count == 0
                   && NetPlayerEnumQueueCallInfo.Count == 0
#endif
                ;
        }

        public static void ProcessQueue()
        {
            if (DebugMode())
            {
                if (UiQueue.Count > 0)
                {
                    UiCallInfo.Add(new CallInfo<UiEventParam>(Ui, UiQueue.Dequeue()));
                }

                if (UiEnumQueue.Count > 0)
                {
                    UiEnumCallInfo.Add(new CallInfo<UiEvent>(UiEnum, UiEnumQueue.Dequeue()));
                }

                if (UiQueueCallInfo.Count > 0)
                {
                    UiQueueCallInfo.Dequeue().Perform();
                }

                if (UiEnumQueueCallInfo.Count > 0)
                {
                    UiEnumQueueCallInfo.Dequeue().Perform();
                }

                if (DebugQueue.Count > 0)
                {
                    DebugCallInfo.Add(new CallInfo<DebugEventParam>(Debug, DebugQueue.Dequeue()));
                }

                if (DebugEnumQueue.Count > 0)
                {
                    DebugEnumCallInfo.Add(new CallInfo<DebugEvent>(DebugEnum, DebugEnumQueue.Dequeue()));
                }

                if (DebugQueueCallInfo.Count > 0)
                {
                    DebugQueueCallInfo.Dequeue().Perform();
                }

                if (DebugEnumQueueCallInfo.Count > 0)
                {
                    DebugEnumQueueCallInfo.Dequeue().Perform();
                }

                if (ServerQueue.Count > 0)
                {
                    ServerCallInfo.Add(new CallInfo<ServerEventParam>(Server, ServerQueue.Dequeue()));
                }

                if (ServerEnumQueue.Count > 0)
                {
                    ServerEnumCallInfo.Add(new CallInfo<ServerEvent>(ServerEnum, ServerEnumQueue.Dequeue()));
                }

                if (ServerQueueCallInfo.Count > 0)
                {
                    ServerQueueCallInfo.Dequeue().Perform();
                }

                if (ServerEnumQueueCallInfo.Count > 0)
                {
                    ServerEnumQueueCallInfo.Dequeue().Perform();
                }


                if (SceneEnumQueue.Count > 0)
                {
                    SceneEnumCallInfo.Add(new CallInfo<SceneEvent>(SceneEnum, SceneEnumQueue.Dequeue()));
                }

                if (SceneQueue.Count > 0)
                {
                    SceneCallInfo.Add(new CallInfo<SceneEventParam>(Scene, SceneQueue.Dequeue()));
                }

                if (SceneQueueCallInfo.Count > 0)
                {
                    SceneQueueCallInfo.Dequeue().Perform();
                }

                if (SceneEnumQueueCallInfo.Count > 0)
                {
                    SceneEnumQueueCallInfo.Dequeue().Perform();
                }

#if PHOTON_UNITY_NETWORKING

                if (NetMasterQueue.Count > 0)
                {
                    NetMasterCallInfo.Add(new CallInfo<NetMasterEventParam>(NetMaster, NetMasterQueue.Dequeue()));
                }

                if (NetMasterEnumQueue.Count > 0)
                {
                    NetMasterEnumCallInfo.Add(new CallInfo<NetMasterEvent>(NetMasterEnum,
                        NetMasterEnumQueue.Dequeue()));
                }

                if (NetMasterQueueCallInfo.Count > 0)
                {
                    NetMasterQueueCallInfo.Dequeue().Perform();
                }

                if (NetMasterEnumQueueCallInfo.Count > 0)
                {
                    NetMasterEnumQueueCallInfo.Dequeue().Perform();
                }


                if (NetOpponentQueue.Count > 0)
                {
                    NetOpponentCallInfo.Add(
                        new CallInfo<NetOpponentEventParam>(NetOpponent, NetOpponentQueue.Dequeue()));
                }

                if (NetOpponentEnumQueue.Count > 0)
                {
                    NetOpponentEnumCallInfo.Add(new CallInfo<NetOpponentEvent>(NetOpponentEnum,
                        NetOpponentEnumQueue.Dequeue()));
                }

                if (NetOpponentQueueCallInfo.Count > 0)
                {
                    NetOpponentQueueCallInfo.Dequeue().Perform();
                }

                if (NetOpponentEnumQueueCallInfo.Count > 0)
                {
                    NetOpponentEnumQueueCallInfo.Dequeue().Perform();
                }


                if (NetGlobalQueue.Count > 0)
                {
                    NetGlobalCallInfo.Add(new CallInfo<NetGlobalEventParam>(NetGlobal, NetGlobalQueue.Dequeue()));
                }

                if (NetGlobalEnumQueue.Count > 0)
                {
                    NetGlobalEnumCallInfo.Add(new CallInfo<NetGlobalEvent>(NetGlobalEnum,
                        NetGlobalEnumQueue.Dequeue()));
                }

                if (NetGlobalQueueCallInfo.Count > 0)
                {
                    NetGlobalQueueCallInfo.Dequeue().Perform();
                }

                if (NetGlobalEnumQueueCallInfo.Count > 0)
                {
                    NetGlobalEnumQueueCallInfo.Dequeue().Perform();
                }


                if (NetPlayerQueue.Count > 0)
                {
                    NetPlayerCallInfo.Add(new CallInfo<NetPlayerEventParam>(NetPlayer, NetPlayerQueue.Dequeue()));
                }

                if (NetPlayerEnumQueue.Count > 0)
                {
                    NetPlayerEnumCallInfo.Add(new CallInfo<NetPlayerEvent>(NetPlayerEnum,
                        NetPlayerEnumQueue.Dequeue()));
                }

                if (NetPlayerQueueCallInfo.Count > 0)
                {
                    NetPlayerQueueCallInfo.Dequeue().Perform();
                }

                if (NetPlayerEnumQueueCallInfo.Count > 0)
                {
                    NetPlayerEnumQueueCallInfo.Dequeue().Perform();
                }
#endif
            }
            else
            {
                if (UiQueue.Count > 0)
                {
                    Ui.Execute(UiQueue.Dequeue());
                }

                if (UiEnumQueue.Count > 0)
                {
                    UiEnum.Execute(UiEnumQueue.Dequeue());
                }

                if (DebugQueue.Count > 0)
                {
                    Debug.Execute(DebugQueue.Dequeue());
                }

                if (DebugEnumQueue.Count > 0)
                {
                    DebugEnum.Execute(DebugEnumQueue.Dequeue());
                }

                if (ServerQueue.Count > 0)
                {
                    Server.Execute(ServerQueue.Dequeue());
                }

                if (ServerEnumQueue.Count > 0)
                {
                    ServerEnum.Execute(ServerEnumQueue.Dequeue());
                }

                if (SceneQueue.Count > 0)
                {
                    Scene.Execute(SceneQueue.Dequeue());
                }

                if (SceneEnumQueue.Count > 0)
                {
                    SceneEnum.Execute(SceneEnumQueue.Dequeue());
                }

#if PHOTON_UNITY_NETWORKING

                if (NetMasterQueue.Count > 0)
                {
                    NetMaster.Execute(NetMasterQueue.Dequeue());
                }

                if (NetMasterEnumQueue.Count > 0)
                {
                    NetMasterEnum.Execute(NetMasterEnumQueue.Dequeue());
                }

                if (NetOpponentQueue.Count > 0)
                {
                    NetOpponent.Execute(NetOpponentQueue.Dequeue());
                }

                if (NetOpponentEnumQueue.Count > 0)
                {
                    NetOpponentEnum.Execute(NetOpponentEnumQueue.Dequeue());
                }

                if (NetGlobalQueue.Count > 0)
                {
                    NetGlobal.Execute(NetGlobalQueue.Dequeue());
                }

                if (NetGlobalEnumQueue.Count > 0)
                {
                    NetGlobalEnum.Execute(NetGlobalEnumQueue.Dequeue());
                }

                if (NetPlayerQueue.Count > 0)
                {
                    NetPlayer.Execute(NetPlayerQueue.Dequeue());
                }

                if (NetPlayerEnumQueue.Count > 0)
                {
                    NetPlayerEnum.Execute(NetPlayerEnumQueue.Dequeue());
                }

#endif

                if (SoundQueue.Count > 0)
                {
                    Sound.Execute(SoundQueue.Dequeue());
                }

                if (SoundEnumQueue.Count > 0)
                {
                    SoundEnum.Execute(SoundEnumQueue.Dequeue());
                }
            }
        }

        public static bool DebugMode()
        {
#if STREAMS_DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}