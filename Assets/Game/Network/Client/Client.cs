using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;

namespace Network {
    sealed class Client: MonoBehaviour {
        // -- deps --
        private global::Client mSession;

        // -- props --
        private JobHandle mJob;
        private NetworkDriver mDriver;
        private NativeArray<byte> mDone;
        private NativeArray<NetworkConnection> mConnection;

        // -- lifecycle --
        private void Start() {
            mDriver = NetworkDriver.Create();

            mDone = new NativeArray<byte>(1, Allocator.Persistent);
            mConnection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);

            var endpoint = NetworkEndPoint.LoopbackIpv4;
            endpoint.Port = Config.kPort;
            mConnection[0] = mDriver.Connect(endpoint);
        }

        private void Update() {
            mJob.Complete();

            var update = new UpdateClient(mDriver, mConnection, mDone);
            mJob = mDriver.ScheduleUpdate();
            mJob = update.Schedule(mJob);
        }

        private void OnDestroy() {
            mJob.Complete();
            mDriver.Dispose();
            mDone.Dispose();
            mConnection.Dispose();
        }

        // -- deps --
        public void Bind(global::Client session) {
            mSession = session;
        }

    }
}
