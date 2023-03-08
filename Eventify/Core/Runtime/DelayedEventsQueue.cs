namespace Eventify.Core.Runtime
{
    public class DelayedEventsQueue
    {
        public static readonly float ExecutionLimit = 0f;

        public static bool IsEmpty()
        {
            return Streams.IsQueueEmpty();
        }

        public static void ProcessEvents()
        {
            Streams.ProcessQueue();
        }
    }
}