using System;
using System.Collections.Generic;
using UniRx;

namespace Eventify.Core.Runtime
{
    public class EditorEventParam : BaseEvent<EditorEvent>
    {
        public EditorEventParam(EditorEvent type) : base(type) { }
    
        public T1 Get<T1>() => ((EditorEventParam<T1>) this).Data;
    }

    public class EditorEventParam<T1> : EditorEventParam
    {
        public T1 Data;

        public EditorEventParam(EditorEvent type, T1 data) : base(type)
        {
            Data = data;
        }
    }
    
    public static partial class Streams
    {
        public static List<CallInfo<EditorEventParam>> EditorCallInfo = new List<CallInfo<EditorEventParam>>();
        public static List<CallInfo<EditorEvent>> EditorEnumCallInfo = new List<CallInfo<EditorEvent>>();
        private static ReactiveCommand<EditorEventParam> Editor { get; } = new ReactiveCommand<EditorEventParam>();
        private static ReactiveCommand<EditorEvent> EditorEnum { get; } = new ReactiveCommand<EditorEvent>();
#if STREAMS_DEBUG
        public static void Raise(this EditorEvent evnt) => EditorEnumCallInfo.Add(new CallInfo<EditorEvent>(EditorEnum, evnt));
        public static void Raise<T>(this EditorEvent evnt, T data) => EditorCallInfo.Add(new CallInfo<EditorEventParam>(Editor, new EditorEventParam<T>(evnt, data)));
#else
        public static void Raise(this EditorEvent evnt) => EditorEnum.Execute(evnt);
        public static void Raise<T>(this EditorEvent evnt, T data) => Editor.Execute(new EditorEventParam<T>(evnt, data));
#endif    
        public static IDisposable Subscribe(this EditorEvent evntType, Action action) => EditorEnum.Where(evnt => evnt == evntType).Subscribe(evnt => action());
        public static IDisposable Subscribe<T>(this EditorEvent evntType, Action<T> action) => Editor.Where(evnt => evnt.Type == evntType).Subscribe(evnt => action(evnt.Get<T>()));
                
        public static IObservable<T> Where<T>(this EditorEvent evntType, Func<T, bool> predicate)
        {
            return Editor.Where(evnt => evnt.Type == evntType && predicate(evnt.Get<T>())).Select(evnt => evnt.Get<T>());
        }
    }
}
