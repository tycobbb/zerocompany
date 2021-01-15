abstract class Session {
    // -- module --
    public static Session Get
        => Instance;

    private static Session Instance;

    // -- queries --
    public abstract string HostIp { get; }

    // -- factories --
    public static void Start(Session session) {
        Instance = session;
    }

    // -- options --
    public sealed class Host: Session {
        public override string HostIp => "";
    }

    public sealed class Client: Session {
        // -- props --
        private readonly string mHostIp;

        // -- lifetime --
        public Client(string hostIp) {
            mHostIp = hostIp;
        }

        // -- queries --
        public override string HostIp => mHostIp;
    }
}
