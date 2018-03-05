using System;

namespace Domain
{
    /// <summary>
    /// A Domain Event.  I use the more specific name here, "inter-aggregate 
    /// event" because these events are only propogated from one Aggregate to
    /// another.
    /// </summary>
    public interface InterAggregateEvent { }

    public interface IInterAggregateEventBus
    {
        void Subscribe<T>(Action<T> subscriber) where T : InterAggregateEvent;
        void Publish(InterAggregateEvent anEvent);
    }

    public class InterAggregateEventBus : IInterAggregateEventBus
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
