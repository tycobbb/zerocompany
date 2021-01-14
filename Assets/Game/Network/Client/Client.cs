using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;
using Config = NetworkConfig;

sealed partial class Client: MonoBehaviour {
    // -- deps --
    private Session.Client mSession;

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

        var endpoint = NetworkEndPoint.Parse(mSession.HostIp, Config.kPort);
        if (endpoint == default) {
            endpoint = NetworkEndPoint.LoopbackIpv4;
            endpoint.Port = Config.kPort;
        }

        mConnection[0] = mDriver.Connect(endpoint);
        Debug.Log($"client: connect to {endpoint.Address}");
    }

    private void Update() {
        mJob.Complete();

        var update = new UpdateJob(mDriver, mConnection, mDone);
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
    public void Bind(Session.Client session) {
        mSession = session;
    }

}
