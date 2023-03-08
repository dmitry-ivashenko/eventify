using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Eventify.Core.Runtime
{
    public class DelayedEventsProcessor
    {
        // This method is called when the game starts
        [RuntimeInitializeOnLoadMethod]
        public static async void Init()
        {
            // Create a cancellation token source
            var source = new CancellationTokenSource();
            // Store the current delta time
            var time = Time.deltaTime;

            // While the cancellation is not requested,
            // the queue is not empty
            // and the execution limit is not exceeded
            while (!source.IsCancellationRequested
                   && !DelayedEventsQueue.IsEmpty()
                   && (Time.deltaTime - time) > DelayedEventsQueue.ExecutionLimit)
            {
                // Process the events in the queue
                DelayedEventsQueue.ProcessEvents();
                // Waiting for next frame
                await Task.Yield();
                // Update the delta time
                time = Time.deltaTime;
            }

            // When the application is quitting, cancel the token
            Application.quitting += () => source.Cancel();
        }
    }
}