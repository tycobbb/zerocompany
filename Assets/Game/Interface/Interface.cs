using UnityEngine;

public sealed class Interface: MonoBehaviour {
    // -- deps --
    private EventLog mLog = EventLog.Get;

    // -- events --
    public void DidClickSpawn() {
        mLog.Add(new SpawnSquad());
    }
}
