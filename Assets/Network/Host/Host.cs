using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;
using Config = NetworkConfig;

sealed partial class Host: MonoBehaviour {
    // -- constants --
    private const int kMaxConnections = 10;

    // -- props --
    private JobHandle mJob;
    private NetworkDriver mDriver;
    private NativeList<NetworkConnection> mConnections;

    // -- lifecycle --
    private void Start() {
        mDriver = NetworkDriver.Create(new NetworkConfigParameter() {
            disconnectTimeoutMS = int.MaxValue,
        });

        var endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = Config.kPort;

        var res = mDriver.Bind(endpoint);
        if (res != 0) {
            Log.E($"Host - failed to listen on port {Config.kPort} (err: {res})");
        } else {
            mDriver.Listen();
            Log.I($"Host - listen on {Config.kPort}");
        }

        mConnections = new NativeList<NetworkConnection>(kMaxConnections, Allocator.Persistent);
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
}
