using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;

sealed partial class Client {
    struct UpdateJob: IJob {
        // -- props --
        private NetworkDriver mDriver;
        private NativeArray<NetworkConnection> mConnection;
        private NativeArray<byte> mDone;

        // -- lifetime --
        public UpdateJob(NetworkDriver driver, NativeArray<NetworkConnection> connection,
            NativeArray<byte> done) {
            mDriver = driver;
            mConnection = connection;
            mDone = done;
        }

        // -- IJob --
        public void Execute() {
            if (!mConnection[0].IsCreated) {
                if (mDone[0] != 1) {
                    Debug.Log("Something went wrong during connect");
                }

                return;
            }

            DataStreamReader stream;
            NetworkEvent.Type cmd;

            while ((cmd = mConnection[0].PopEvent(mDriver, out stream)) != NetworkEvent.Type.Empty) {
                switch (cmd) {
                    case NetworkEvent.Type.Connect: {
                        Debug.Log("We are now connected to the server");
                        var value = (uint) 1;
                        var writer = mDriver.BeginSend(mConnection[0]);
                        writer.WriteUInt(value);
                        mDriver.EndSend(writer);
                        break;
                    }
                    case NetworkEvent.Type.Data: {
                        var value = stream.ReadUInt();
                        Debug.Log("Got the value = " + value + " back from the server");
                        // And finally change the `done[0]` to `1`
                        mDone[0] = 1;
                        mConnection[0].Disconnect(mDriver);
                        mConnection[0] = default;
                        break;
                    }
                    case NetworkEvent.Type.Disconnect: {
                        Debug.Log("Client got disconnected from server");
                        mConnection[0] = default;
                        break;
                    }
                    default: {
                        Debug.Log("something else happened.");
                        break;
                    }
                }
            }
        }
    }
}
