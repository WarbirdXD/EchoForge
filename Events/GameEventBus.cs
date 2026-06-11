using System;
using System.Collections.Generic;

namespace EchoForge.Events
{
    public class GameEventBus
    {
        private readonly Dictionary<Type, List<Action<IGameEvent>>> handlers =
            new Dictionary<Type, List<Action<IGameEvent>>>();

        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            Type type = typeof(T);
            if (!handlers.ContainsKey(type))
                handlers[type] = new List<Action<IGameEvent>>();

            handlers[type].Add(e => handler((T)e));
        }

        public void Publish<T>(T gameEvent) where T : IGameEvent
        {
            Type type = typeof(T);
            if (!handlers.ContainsKey(type)) return;

            foreach (Action<IGameEvent> handler in handlers[type])
                handler(gameEvent);
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            Type type = typeof(T);
            if (handlers.ContainsKey(type))
                handlers[type].RemoveAll(h => h.Target == handler.Target);
        }

        public void Clear()
        {
            handlers.Clear();
        }
    }
}
