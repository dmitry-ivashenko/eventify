using System.Collections.Generic;

namespace Eventify.Core.Runtime
{
    public class FixedQueue<T>
    {
        private readonly object _lockObject = new object();
        public readonly Queue<T> Queue = new Queue<T>();
        public int Limit { get; }

        public FixedQueue(int limit)
        {
            Limit = limit;
        }

        public void Enqueue(T obj)
        {
            Queue.Enqueue(obj);
            
            lock (_lockObject)
            {
                while (Queue.Count > Limit && Queue.Dequeue() != null) { }
            }
        }

        public void Clear()
        {
            Queue.Clear();
        }
    }
}
