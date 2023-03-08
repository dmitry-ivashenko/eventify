
namespace Eventify.Core.Runtime
{
    public class BaseEvent
    {
    }

    public class BaseEvent<T> : BaseEvent
    {
        public T Type;

        protected BaseEvent(T type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"{Type.GetType().Name}.{Type.ToString()}";
        }
    }
}
