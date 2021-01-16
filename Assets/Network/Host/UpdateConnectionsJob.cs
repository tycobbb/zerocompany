using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;

sealed partial class Host {
    private struct UpdateConnectionsJob: IJob {
        // -- props --
        private NetworkDriver mDriver;
        private NativeList<NetworkConnection> mConnections;

        // -- lifetime --
        public UpdateConnectionsJob(NetworkDriver driver, NativeList<NetworkConnection> connections) {
            mDriver = driver;
            mConnections = connections;
        }

        // -- IJob --
        public void Execute() {
            // remove old connections
            for (var i = 0; i < mConnections.Length; i++) {
                if (!mConnections[i].IsCreated) {
                    mConnections.RemoveAtSwapBack(i);
                    --i;
                }
            }

            // accept new connections
            NetworkConnection c;
            while ((c = mDriver.Accept()) != default) {
                mConnections.Add(c);
                Log.D("Host - accepted connection");
            }
        }
    }
}
