using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class EventLog {
    // -- module --
    public static EventLog Get
        => sInstance.Value;

    private static Lazy<EventLog> sInstance
        = new Lazy<EventLog>(() => new EventLog());

    // -- props --
    private uint mStep = 0;
    private int mCursor = 0;
    private readonly List<Event> mLog = new List<Event>();
    private readonly Queue<Event> mPending = new Queue<Event>();

    // -- commands --
    public void AdvanceStep() {
        mStep += 1;
    }

    public void Add(Event evt) {
        Log.I($"Events - @{mStep}: add {evt}");
        mLog.Add(evt);
    }

    public void AddPending(Event.Value val) {
        Log.D($"Events - @{mStep}: add pending {val}");
        mPending.Enqueue(new Event(mStep, val));
    }

    public IEnumerable<Event> AdvanceCursor() {
        var i0 = mCursor;
        var i1 = mCursor;

        for (; i1 < mLog.Count; i1++) {
            var evt = mLog[i1];
            if (evt.Step > mStep - 2) {
                break;
            }
        }

        var span = i1 - i0;
        if (span != 0) {
            mCursor = i1;
            Log.D($"Events - @{mStep}: run {span} events (cursor={mCursor})");
        }

        return mLog.Skip(i0).Take(span);
    }

    // -- queries --
    public bool HasPending
        => mPending.Count != 0;

    public Event PopPending() {
        return mPending.Dequeue();
    }
}
