using DDD;
using System;

namespace Application
{
    // TODO: Implement a real InterAggregateEventBus.
    // TODO: Implement a real way to subscribe to and handle InterAggregateEvents.
    public class FakeEventBus : IInterAggregateEventBus
    {
        public void Publish(InterAggregateEvent anEvent)
        {
            Console.WriteLine($"Published: {anEvent}");
        }

        public void Subscribe<T>(Action<T> subscriber) where T : InterAggregateEvent
        {
            Console.WriteLine($"Registered subscription for {typeof(T).Name}.");
        }
    }
}
