using UnityEngine;

public sealed class Connect: MonoBehaviour {
    // -- deps --
    private Session mSession;

    // -- lifecycle --
    private void Awake() {
        mSession = Session.Get;
        Call();
    }

    // -- command --
    private void Call() {
        switch (mSession) {
            case Session.Host session: {
                ConnectAs<Host>();
                ConnectAs<Client>().Bind(session);
                break;
            }
            case Session.Client session: {
                ConnectAs<Client>().Bind(session);
                break;
            }
        }

        Destroy(this);
    }

    // -- c/helpers
    private T ConnectAs<T>() where T: Component {
        return gameObject.AddComponent<T>();
    }
}
