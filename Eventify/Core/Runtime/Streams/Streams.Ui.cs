using System;
using System.Collections.Generic;
using UniRx;
// ReSharper disable UnusedMember.Global

namespace Eventify.Core.Runtime
{
    public class UiEventParam : BaseEvent<UiEvent>
    {
        public UiEventParam(UiEvent type) : base(type) { }
    
        public T1 Get<T1>() => ((UiEventParam<T1>) this).Data;
    }

    public class UiEventParam<T1> : UiEventParam
    {
        public T1 Data;

        public UiEventParam(UiEvent type, T1 data) : base(type)
        {
            Data = data;
        }
    }
    
    public static partial class Streams
    {
        public static List<CallInfo<UiEventParam>> UiCallInfo = new List<CallInfo<UiEventParam>>();
        public static List<CallInfo<UiEvent>> UiEnumCallInfo = new List<CallInfo<UiEvent>>();
        private static ReactiveCommand<UiEventParam> Ui { get; } = new ReactiveCommand<UiEventParam>();
        private static ReactiveCommand<UiEvent> UiEnum { get; } = new ReactiveCommand<UiEvent>();
        private static readonly Queue<UiEventParam> UiQueue = new Queue<UiEventParam>();
        private static readonly Queue<UiEvent> UiEnumQueue = new Queue<UiEvent>();
        private static readonly Queue<CallInfo<UiEventParam>> UiQueueCallInfo = new Queue<CallInfo<UiEventParam>>();
        private static readonly Queue<CallInfo<UiEvent>> UiEnumQueueCallInfo = new Queue<CallInfo<UiEvent>>();

#if STREAMS_DEBUG
          public static void Raise(this UiEvent evnt) => UiEnumCallInfo.Add(new CallInfo<UiEvent>(UiEnum, evnt));
          public static void Raise<T>(this UiEvent evnt, T data) => UiCallInfo.Add(new CallInfo<UiEventParam>(Ui, new UiEventParam<T>(evnt, data)));
#else
        public static void Raise(this UiEvent evnt) => UiEnum.Execute(evnt);
        public static void Raise<T>(this UiEvent evnt, T data) => Ui.Execute(new UiEventParam<T>(evnt, data));
#endif
    
        public static IDisposable Subscribe(this UiEvent evntType, Action action)
        {
            return UiEnum.Where(evnt => evnt == evntType).Subscribe(evnt => action());
        }

        public static IDisposable Subscribe<T>(this UiEvent evntType, Action<T> action)
        {
            return Ui.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T>()));
        }

        public static IObservable<T> Where<T>(this UiEvent evntType, Func<T, bool> predicate)
        {
            return Ui.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T>())).Select(evnt => evnt.Get<T>());
        }

        public static IObservable<UiEvent> AsObservable(this UiEvent evntType)
        {
            return UiEnum.Where(evnt => evnt == evntType);
        }
    }
}
