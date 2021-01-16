struct SpawnSquad: Event.Value {
    public AnyEvent.Value Into() {
        return new AnyEvent.Value(
            type: EventType.SpawnSquad
        );
    }

    internal struct Factory: Event.Value.Factory {
        public Event.Value From(AnyEvent.Value evt) {
            return new SpawnSquad();
        }
    }
}
