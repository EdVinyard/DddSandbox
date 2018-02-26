using System;

namespace Domain
{
    public interface InterAggregateEvent { }

    public class InterAggregateEventBus
    {
        public void Subscribe<T>(Action<T> subscriber) where T : InterAggregateEvent
        {
            throw new NotImplementedException();
        }

        public void Publish(InterAggregateEvent anEvent)
        {
            throw new NotImplementedException();
        }
    }
}
