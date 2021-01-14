sealed class Game {
    // -- module --
    public static Game Shared { get; private set; }

    // -- props --
    private Session mSession;

    // -- lifetime --
    private Game(Session session) {
        mSession = session;
    }

    // -- queries --
    public Session Session => mSession;

    // -- factories --
    public static void Start(Session session) {
        Shared = new Game(session);
    }
}
