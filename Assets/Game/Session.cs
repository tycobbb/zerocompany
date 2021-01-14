class Session {
    // -- module --
    public static Session Get
        => Instance;

    private static Session Instance;

    // -- factories --
    public static void Start(Session session) {
        Instance = session;
    }

    // -- options --
    public sealed class Host: Session {
    }

    public sealed class Client: Session {
        // -- props --
        private string mHostIp;

        // -- lifetime --
        public Client(string hostIp) {
            mHostIp = hostIp;
        }

        // -- queries --
        public string HostIp => mHostIp;
    }
}
