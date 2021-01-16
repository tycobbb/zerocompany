using UnityEngine;

// -- types --
enum LogLevel {
    None,
    Error,
    Info,
    Debug
}

// -- impls --
static class Log {
    // -- props --
    private static readonly LogLevel Level = LogLevel.Debug;

    // -- commands --
    public static void E(string message) {
        #if UNITY_EDITOR
        if (Level >= LogLevel.Error) {
            Debug.Log("[E] " + message);
        }
        #endif
    }

    public static void I(string message) {
        #if UNITY_EDITOR
        if (Level >= LogLevel.Info) {
            Debug.Log("[I] " + message);
        }
        #endif
    }

    public static void D(string message) {
        #if UNITY_EDITOR
        if (Level >= LogLevel.Debug) {
            Debug.Log("[D] " + message);
        }
        #endif
    }
}
