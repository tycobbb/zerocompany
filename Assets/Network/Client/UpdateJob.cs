using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;

sealed partial class Client {
    private struct UpdateJob: IJob {
        // -- props --
        private NetworkDriver mDriver;
        private NativeArray<NetworkConnection> mConnection;
        private NativeList<AnyEvent> mEvents;

        // -- lifetime --
        public UpdateJob(
            NetworkDriver driver,
            NativeArray<NetworkConnection> connection,
            NativeList<AnyEvent> events
        ) {
            mDriver = driver;
            mConnection = connection;
            mEvents = events;
        }

        // -- IJob --
        public void Execute() {
            if (!mConnection[0].IsCreated) {
                Log.E("Client - not connected");
                return;
            }

            // send events to host
            var n = (byte)mEvents.Length;
            if (n != 0) {
                Log.D($"Client - sending {n} events");
                var writer = mDriver.BeginSend(mConnection[0]);

                // write count
                writer.WriteByte(n);

                // write each event
                for (var i = 0; i < mEvents.Length; i++) {
                    var evt = mEvents[i];
                    writer.WriteUInt(evt.Step);
                    writer.WriteByte((byte)evt.Val.Type);
                    mEvents.RemoveAtSwapBack(i);
                    --i;
                }

                mDriver.EndSend(writer);
            }

            // read events
            DataStreamReader stream;
            NetworkEvent.Type cmd;

            while ((cmd = mConnection[0].PopEvent(mDriver, out stream)) != NetworkEvent.Type.Empty) {
                switch (cmd) {
                    case NetworkEvent.Type.Connect: {
                        Log.I("Client - connected");
                        break;
                    }
                    case NetworkEvent.Type.Data: {
                        // read number of events
                        n = stream.ReadByte();
                        Log.D($"Client - received {n} events");

                        // read events out of stream
                        for (var i = 0; i < n; i++) {
                            mEvents.Add(new AnyEvent(
                                step: stream.ReadUInt(),
                                new AnyEvent.Value(
                                    type: (EventType)stream.ReadByte()
                                )
                            ));
                        }

                        break;
                    }
                    case NetworkEvent.Type.Disconnect: {
                        Log.I("Client - disconnected");
                        mConnection[0] = default;
                        break;
                    }
                    case NetworkEvent.Type.Empty: {
                        break;
                    }
                }
            }
        }
    }
}
