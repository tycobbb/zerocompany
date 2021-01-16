using UnityEngine;

public sealed class Interface: MonoBehaviour {
    // -- deps --
    private readonly EventLog mLog = EventLog.Get;

    // -- events --
    public void DidClickSpawn() {
        mLog.AddPending(new SpawnSquad());
    }
}
