using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Assertions;

struct UpdateHost: IJobParallelForDefer {
    // -- props --
    private NetworkDriver.Concurrent mDriver;
    private NativeArray<NetworkConnection> mConnections;

    // -- lifetime --
    public UpdateHost(NetworkDriver.Concurrent driver, NativeArray<NetworkConnection> connections) {
        mDriver = driver;
        mConnections = connections;
    }

    // -- IJob --
    public void Execute(int index) {
        DataStreamReader stream;
        Assert.IsTrue(mConnections[index].IsCreated);

        NetworkEvent.Type cmd;
        while ((cmd = mDriver.PopEventForConnection(mConnections[index], out stream)) != NetworkEvent.Type.Empty) {
            switch (cmd) {
                case NetworkEvent.Type.Data: {
                    var number = stream.ReadUInt();

                    Debug.Log("Got " + number + " from the Client adding + 2 to it.");
                    number += 2;

                    var writer = mDriver.BeginSend(mConnections[index]);
                    writer.WriteUInt(number);
                    mDriver.EndSend(writer);
                    break;
                }

                case NetworkEvent.Type.Disconnect: {
                    Debug.Log("Client disconnected from server");
                    mConnections[index] = default;
                    break;
                }
            }
        }
    }
}
