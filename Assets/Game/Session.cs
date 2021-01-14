// -- types --
interface Session {
}

// -- options --
sealed class Host: Session {
}

sealed class Client: Session {
    // -- props --
    private string mHostIp;

    // -- lifetime --
    public Client(string hostIp) {
        mHostIp = hostIp;
    }
}
