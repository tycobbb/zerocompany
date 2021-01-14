using System;
using System.Collections.Generic;

// -- types --
using EventListener = System.Action<Event>;

// -- impls --
class EventLog {
    // -- module --
    public static EventLog Get
        => sInstance.Value;

    private static Lazy<EventLog> sInstance
        = new Lazy<EventLog>(() => new EventLog());

    // -- props --
    private List<Event> mLog = new List<Event>();
    private List<EventListener> mListeners = new List<EventListener>();

    // -- commands --
    public void Add(Event e) {
       mLog.Add(e);

       // notify listeners
       foreach (var listener in mListeners) {
           listener(e);
       }
    }

    public void OnEvent(EventListener listener) {
        mListeners.Add(listener);
    }
}
