using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

sealed class Menu: MonoBehaviour {
    // -- props --
    private string mIp;

    // -- commands --
    private void StartHost() {
        StartSession(new Session.Host());
    }

    private void StartClient() {
        StartSession(new Session.Client(mIp?.Trim() ?? ""));
    }

    // -- c/helpers
    private void StartSession(Session session) {
        Session.Start(session);

        // advance to the loading scene
        var i = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(i + 1);
    }

    // -- events --
    // -- e/host
    public void DidClickStart() {
        StartHost();
    }

    // -- e/join
    public void DidChangeIp(InputField field) {
        mIp = field.text;
    }

    public void DidClickJoin() {
        StartClient();
    }
}
