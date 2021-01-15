struct SpawnSquad: Event {
    public AnyEvent Into() {
        return new AnyEvent(EventType.SpawnSquad);
    }

    struct Factory: Event.Factory {
        public Event From(AnyEvent evt) {
            return new SpawnSquad();
        }
    }
}

