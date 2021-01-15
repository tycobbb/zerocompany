using System;
using UnityEngine;
using UnityEngine.Serialization;

public sealed class Game: MonoBehaviour {
    // -- deps --
    private EventLog mLog;

    // -- props --
    [SerializeField]
    [Tooltip("The squad prototype.")]
    private GameObject mSquadPrefab;

    // -- lifecycle --
    private void Awake() {
        mLog = EventLog.Get;
    }

    private void Start() {
        mLog.OnEvent(DidReceiveEvent);
    }

    // -- commands --
    private void DidReceiveEvent(Event e) {
        switch (e) {
            case SpawnSquad _: {
                Instantiate(mSquadPrefab); break;
            }
        }
    }
}
