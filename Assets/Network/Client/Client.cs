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
    private NativeArray<byte> mDone;
    private NativeArray<NetworkConnection> mConnection;
    private NativeList<AnyEvent> mEvents;
    private NetworkConnection.State mState = NetworkConnection.State.Connecting;
    private readonly Queue<AnyEvent> mPendingEvents = new Queue<AnyEvent>();

    // -- lifecycle --
    private void Start() {
        mDriver = NetworkDriver.Create(new NetworkConfigParameter {
            connectTimeoutMS = 1000,
            maxConnectAttempts = 3,
            disconnectTimeoutMS = 30000,
            maxFrameTimeMS = 0,
        });

        mDone = new NativeArray<byte>(1, Allocator.Persistent);
        mConnection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
        mEvents = new NativeList<AnyEvent>(10, Allocator.Persistent);

        var endpoint = NetworkEndPoint.Parse(mSession.HostIp, Config.kPort);
        if (endpoint == default) {
            endpoint = NetworkEndPoint.LoopbackIpv4;
            endpoint.Port = Config.kPort;
        }

        mConnection[0] = mDriver.Connect(endpoint);
        Debug.Log($"client: connecting to {endpoint.Address}");

        mLog.OnEvent(DidAddEvent);
    }

    private void Update() {
        mJob.Complete();

        // copy state from native containers
        mState = mConnection[0].GetState(mDriver);

        // copy events to native containers
        while (mPendingEvents.Count != 0) {
            mEvents.Add(mPendingEvents.Dequeue());
        }

        // schedule next update
        var update = new UpdateJob(mDriver, mConnection, mDone, mEvents);
        mJob = mDriver.ScheduleUpdate();
        mJob = update.Schedule(mJob);
    }

    private void OnDestroy() {
        mJob.Complete();
        mDriver.Dispose();
        mDone.Dispose();
        mConnection.Dispose();
    }

    // -- queries --
    public bool IsConnected() {
        return mState == NetworkConnection.State.Connected;
    }

    public bool IsDisconnected() {
        return mState == NetworkConnection.State.Disconnected;
    }

    // -- events --
    private void DidAddEvent(Event evt) {
        mPendingEvents.Enqueue(evt.Into());
    }

    // -- deps --
    public void Bind(Session session, EventLog log = null) {
        mLog = log ?? EventLog.Get;
        mSession = session;
    }
}
