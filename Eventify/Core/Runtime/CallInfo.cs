using System;
using System.Collections.Generic;
using System.Diagnostics;
using UniRx;

namespace Eventify.Core.Runtime
{
    public class CallInfo<T> : CallInfoBase
    {
        public T Enum;
        private readonly ReactiveCommand<T> _stream;
        private readonly Type _dataType;
        private string _singleLine;

        public CallInfo(ReactiveCommand<T> stream, T evnt, bool postponed = false)
        {
            _stream = stream;
            Time = DateTime.Now;
            Log = Environment.StackTrace.RemoveFirstLines(3);
            Enum = evnt;
            _dataType = Enum.GetType();
            
            if (!postponed)
            {
                Perform();
            }
        }

        public void Perform()
        {
            var sw = Stopwatch.StartNew();
            _stream.Execute(Enum);
            sw.Stop();

            Duration = sw.Elapsed;
            IsPerformed = true;
            
            _singleLine = $"[{Time.ToLongTimeString()}] {GetTitle()} ({Duration.TotalMilliseconds} ms)";
        }

        public override string GetTitle()
        {
            var type = Enum.GetType();
            return type.IsEnum ? $"{type.Name}.{Enum.ToString()}" : Enum.ToString();
        }
        
        public override bool IsOneOf<T1>(List<T1> list)
        {
            var listType = typeof(T1);

            if (_dataType.IsEnum)
            {
                if (listType == _dataType)
                {
                    var intValue = Convert.ToInt32(Enum);
                    foreach (var item in list)
                    {
                        if (Convert.ToInt32(item) == intValue)
                        {
                            return true;
                        }
                    }
                }
            }
            else if (Enum is BaseEvent<T1> baseEvent)
            {
                foreach (var item in list)
                {
                    if (item.Equals(baseEvent.Type))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override string SingleLine()
        {
            return _singleLine;
        }
    }
}