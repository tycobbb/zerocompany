using UnityEngine;

public sealed class Game: MonoBehaviour {
    // -- constants --
    private const uint mStepSpan = 200;

    // -- deps --
    private EventLog mLog;

    // -- props --
    private uint mTime = 0;
    private uint mNextStepTime = mStepSpan;

    // -- p/fields
    [SerializeField]
    [Tooltip("The squad prototype.")]
    private GameObject mSquadPrefab;

    // -- lifecycle --
    private void Awake() {
        mLog = EventLog.Get;
    }

    private void FixedUpdate() {
        // i think we want to move the simulation step forward deterministically,
        // hence fixed update
        var delta = (uint)(Time.fixedDeltaTime * 1000.0f);
        mTime += delta;

        // advance step once we pass the boundary
        if (mTime >= mNextStepTime) {
            mNextStepTime += mStepSpan;
            mLog.AdvanceStep();
            RunEvents();
        }
    }

    // -- commands --
    private void RunEvents() {
        var events = mLog.AdvanceCursor();

        foreach (var evt in events) {
            RunEvent(evt.Val);
        }
    }

    private void RunEvent(Event.Value e) {
        switch (e) {
            case SpawnSquad _: {
                Instantiate(mSquadPrefab, transform); break;
            }
        }
    }
}
