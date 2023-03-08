using System;
using System.Collections.Generic;

namespace Eventify.Core.Runtime
{
    public class CallInfoBase
    {
        public static readonly FixedQueue<CallInfoBase> FullLog = new FixedQueue<CallInfoBase>(1000);
        
        public DateTime Time { get; set; }
        public TimeSpan Duration;
        public string Log;
        
        public bool IsPerformed { get; set; }

        protected CallInfoBase()
        {
            FullLog.Enqueue(this);
        }

        public virtual string GetTitle()
        {
            return $"CallInfoBase";
        }

        public virtual bool IsOneOf<T>(List<T> list)
        {
            return false;
        }

        public virtual string SingleLine()
        {
            return string.Empty;
        }
    }
}