// -- types --
interface Event {
    AnyEvent Into();

    interface Factory {
        Event From(AnyEvent evt);
    }
}

readonly struct AnyEvent {
    // -- props --
    public readonly EventType Type;

    // -- lifetime --
    public AnyEvent(EventType type) {
        Type = type;
    }
}
