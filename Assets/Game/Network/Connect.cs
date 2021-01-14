using UnityEngine;

public sealed class Connect: MonoBehaviour {
    // -- deps --
    private Session mSession;

    // -- lifecycle --
    private void Awake() {
        mSession = Session.Get;

        switch (mSession) {
            case Session.Host session:
                ConnectAs<Host>().Bind(session); break;
            case Session.Client session:
                ConnectAs<Client>().Bind(session); break;
            default: Remove(); break;
        }
    }

    // -- command --
    private T ConnectAs<T>() where T: Component {
        var component = gameObject.AddComponent<T>();
        Remove();
        return component;
    }

    private void Remove() {
        Destroy(this);
    }
}
