using UnityEngine;

namespace Network {
    public sealed class Connect: MonoBehaviour {
        // -- lifecycle --
        private void Awake() {
            switch (Game.Shared?.Session) {
                case global::Host session:
                    ConnectAs<Host>().Bind(session); break;
                case global::Client session:
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
}
