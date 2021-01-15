using System;
using System.Collections.Generic;

// -- impls --
class EventLog {
    // -- module --
    public static EventLog Get
        => sInstance.Value;

    private static Lazy<EventLog> sInstance
        = new Lazy<EventLog>(() => new EventLog());

    // -- props --
    private List<Event> mLog = new List<Event>();
    private List<Action<Event>> mListeners = new List<Action<Event>>();

    // -- commands --
    public void Add(Event e) {
       mLog.Add(e);

       // notify listeners
       foreach (var listener in mListeners) {
           listener(e);
       }
    }

    public void OnEvent(Action<Event> listener) {
        mListeners.Add(listener);
    }
}
