using System;

namespace LATwo
{
    public class Message<T> //! i hate this. its ugly. its weird. its dumb.
    {
        // public static Instance of the Message Object itself, so we can access it from everywhere
        // has a generic parameter, so we can basically use every custom type as parameter
        public static readonly Message<T> Instance = new Message<T>();

        // private generic Action as delegate so we can use generic Callbacks
        private Action<T> delegateAction = delegate { };


        /// <summary>
        /// Our internal set-Method to set the callback Handler
        /// </summary>
        /// <param name="handler"></param>
        private void set(Action<T> handler)
        {
            delegateAction = handler;
        }

        // The static Set Method, which will be called by the Message Instance
        public static void Set(Action<T> handler)
        {
            Instance.set(handler);
        }

        // Our internal add Method to add a Callback Handler
        private void add(Action<T> handler)
        {
            delegateAction += handler;
        }

        // The static Add Method, which will be called by the Message Instance
        // to subscribe to the generic Messages/Events
        public static void Add(Action<T> handler)
        {
            Instance.add(handler);
        }

        // Our internal remove Method to remove a callback handler
        private void remove(Action<T> handler)
        {
            delegateAction -= handler;
        }

        // The static Remove Method, which is used by the Message Instance
        // to remove/unsubscribe from generic Messages/Events
        public static void Remove(Action<T> handler)
        {
            Instance.remove(handler);
        }

        // The internal raise Method to post/raise/fire a generic Event
        private void raise(T genericEvent)
        {
            delegateAction(genericEvent);
        }

        // The static Raise Method which is used by the Message Instance
        // to post/raise/fire the generic Event
        public static void Raise(T genericEvent)
        {
            Instance.raise(genericEvent);
        }
    }

    public static class Message
    {
        public static void Raise<T>(T genericEvent)
        {
            Message<T>.Raise(genericEvent);
        }
    }
}