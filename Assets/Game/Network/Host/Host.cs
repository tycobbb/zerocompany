using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;
using Config = NetworkConfig;

sealed partial class Host: MonoBehaviour {
    // -- constants --
    private const int kMaxConnections = 10;

    // -- deps --
    private Session.Host mSession;

    // -- props --
    private JobHandle mJob;
    private NetworkDriver mDriver;
    private NativeList<NetworkConnection> mConnections;

    // -- lifecycle --
    private void Start() {
        mDriver = NetworkDriver.Create();

        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = Config.kPort;

        var res = mDriver.Bind(endpoint);
        if (res != 0) {
            Debug.Log($"Failed to bind to {Config.kPort}; {res}");
        } else {
            mDriver.Listen();
        }

        mConnections = new NativeList<NetworkConnection>(kMaxConnections, Allocator.Persistent);
        Debug.Log($"host: listen on {Config.kPort}");
    }

    private void Update() {
        mJob.Complete();

        var updatec = new UpdateConnectionsJob(mDriver, mConnections);
        var updateh = new UpdateJob(mDriver.ToConcurrent(), mConnections.AsDeferredJobArray());

        mJob = mDriver.ScheduleUpdate();
        mJob = updatec.Schedule(mJob);
        mJob = updateh.Schedule(mConnections, 1, mJob);
    }

    private void OnDestroy() {
        mJob.Complete();
        mDriver.Dispose();
        mConnections.Dispose();
    }

    // -- deps --
    public void Bind(Session.Host session) {
        mSession = session;
    }
}
