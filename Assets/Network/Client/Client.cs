using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;
using Config = NetworkConfig;

sealed partial class Client: MonoBehaviour {
    // -- deps --
    private EventLog mLog;
    private Session mSession;

    // -- props --
    private JobHandle mJob;
    private NetworkDriver mDriver;
    private NativeArray<NetworkConnection> mConnection;
    private NativeList<AnyEvent> mEvents;
    private NetworkConnection.State mState = NetworkConnection.State.Connecting;

    // -- lifecycle --
    private void Start() {
        mDriver = NetworkDriver.Create(new NetworkConfigParameter {
            connectTimeoutMS = 1000,
            maxConnectAttempts = 3,
            disconnectTimeoutMS = int.MaxValue,
            maxFrameTimeMS = 0,
        });

        mConnection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
        mEvents = new NativeList<AnyEvent>(10, Allocator.Persistent);

        var endpoint = NetworkEndPoint.Parse(mSession.HostIp, Config.kPort);
        if (endpoint == default) {
            endpoint = NetworkEndPoint.LoopbackIpv4;
            endpoint.Port = Config.kPort;
        }

        mConnection[0] = mDriver.Connect(endpoint);
        Log.I($"Client - connecting to {endpoint.Address}");
    }

    private void Update() {
        mJob.Complete();

        // copy connection state from native container
        mState = mConnection[0].GetState(mDriver);

        // copy received events from native container
        var decode = new DecodeEvent();
        for (var i = 0; i < mEvents.Length; i++) {
            mLog.Add(decode.Call(mEvents[i]));
            mEvents.RemoveAtSwapBack(i);
            --i;
        }

        // copy pending events to native container to send to host
        while (mLog.HasPending) {
            mEvents.Add(mLog.PopPending().Into());
        }

        // schedule next update
        var update = new UpdateJob(mDriver, mConnection, mEvents);
        mJob = mDriver.ScheduleUpdate();
        mJob = update.Schedule(mJob);
    }

    private void OnDestroy() {
        mJob.Complete();
        mDriver.Dispose();
        mConnection.Dispose();
    }

    // -- queries --
    public bool IsConnected() {
        return mState == NetworkConnection.State.Connected;
    }

    public bool IsDisconnected() {
        return mState == NetworkConnection.State.Disconnected;
    }

    // -- deps --
    public void Bind(Session session, EventLog log = null) {
        mLog = log ?? EventLog.Get;
        mSession = session;
    }
}
