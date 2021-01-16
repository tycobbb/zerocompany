using UnityEngine.Assertions;

struct DecodeEvent {
    // -- command --
    public Event Call(AnyEvent evt) {
        return new Event(
            evt.Step,
            GetFactory(evt.Val).From(evt.Val)
        );
    }

    // -- c/helpers
    private Event.Value.Factory GetFactory(AnyEvent.Value val) {
        switch (val.Type) {
            case EventType.SpawnSquad:
                return new SpawnSquad.Factory();
            default: {
                Assert.IsTrue(false, $"tried to decode event w/ unknown type: {val.Type}");
                return default;
            }
        }

    }
}
