using System;

namespace DDD
{
    /// <summary>
    /// A Domain Event.  I use the more specific name here, "inter-aggregate 
    /// event" because these events are only propogated from one Aggregate to
    /// another.
    /// </summary>
    public interface InterAggregateEvent : HasState
    {
    }

    public interface IInterAggregateEventBus
    {
        void Subscribe<T>(Action<T> subscriber) where T : InterAggregateEvent;
        void Publish(InterAggregateEvent anEvent);
    }
}
