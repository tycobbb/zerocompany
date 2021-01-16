using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Assertions;

sealed partial class Host {
    private struct UpdateJob: IJobParallelForDefer {
        // -- props --
        private NetworkDriver.Concurrent mDriver;
        private NativeArray<NetworkConnection> mConnections;

        // -- lifetime --
        public UpdateJob(NetworkDriver.Concurrent driver, NativeArray<NetworkConnection> connections) {
            mDriver = driver;
            mConnections = connections;
        }

        // -- IJob --
        public void Execute(int ci) {
            DataStreamReader stream;
            Assert.IsTrue(mConnections[ci].IsCreated);

            NetworkEvent.Type cmd;
            while ((cmd = mDriver.PopEventForConnection(mConnections[ci], out stream)) != NetworkEvent.Type.Empty) {
                switch (cmd) {
                    case NetworkEvent.Type.Data: {
                        // read number of events
                        var n = stream.ReadByte();
                        Log.D($"Host - received {n} events from client {ci}");

                        // read events out of stream
                        var events = new AnyEvent[n];

                        for (var i = 0; i < n; i++) {
                            events[i] = new AnyEvent(
                                step: stream.ReadUInt(),
                                new AnyEvent.Value(
                                    type: (EventType)stream.ReadByte()
                                )
                            );
                        }

                        // route events back to every client
                        for (var i = 0; i < mConnections.Length; i++) {
                            var writer = mDriver.BeginSend(mConnections[i]);

                            // write the number of events
                            writer.WriteByte(n);

                            // write each event
                            foreach (var evt in events) {
                                writer.WriteUInt(evt.Step);
                                writer.WriteByte((byte)evt.Val.Type);
                            }

                            mDriver.EndSend(writer);
                        }

                        break;
                    }

                    case NetworkEvent.Type.Disconnect: {
                        Log.I($"Host - client {ci} disconnected");
                        mConnections[ci] = default;
                        break;
                    }
                }
            }
        }
    }
}
