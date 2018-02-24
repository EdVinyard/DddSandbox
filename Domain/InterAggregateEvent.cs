using System;

namespace Domain
{
    public class InterAggregateEvent<T> where T : class
    {
        public InterAggregateEvent(T content)
        {
            Content = content;
        }

        public T Content { get; private set; }
    }

    public class InterAggregateEventBus
    {
        public void Subscribe<T>(Action<T> subscriber) where T : class
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(InterAggregateEvent<T> anEvent) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
