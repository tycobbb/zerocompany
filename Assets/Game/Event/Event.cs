readonly struct Event {
    // -- props --
    readonly uint mStep;
    readonly Value mValue;

    // -- lifetime --
    public Event(uint step, Value value) {
        mStep = step;
        mValue = value;
    }

    // -- queries --
    public uint Step => mStep;
    public Value Val => mValue;

    // -- q/debug
    public override string ToString() {
        return $"<E @{Step} Val={Val}>";
    }

    // -- q/conversions
    public AnyEvent Into() {
        return new AnyEvent(mStep, mValue.Into());
    }

    // -- Value --
    internal interface Value {
        AnyEvent.Value Into();

        internal interface Factory {
            Value From(AnyEvent.Value evt);
        }
    }
}

readonly struct AnyEvent {
    // -- props --
    readonly uint mStep;
    readonly Value mValue;

    // -- lifetime --
    public AnyEvent(uint step, Value value) {
        mStep = step;
        mValue = value;
    }

    // -- queries --
    public uint Step => mStep;
    public Value Val => mValue;

    // -- q/debug
    public override string ToString() {
        return $"<AE @{Step} Val={Val}>";
    }

    // -- Value --
    internal readonly struct Value {
        // -- props --
        public readonly EventType Type;

        // -- lifetime --
        public Value(EventType type) {
            Type = type;
        }

        // -- q/debug
        public override string ToString() {
            return $"<AE.Val Type={Type}>";
        }
    }
}
