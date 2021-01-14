using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

internal sealed class Menu: MonoBehaviour {
    // -- props --
    private string mIp;

    // -- commands --
    private void StartHost() {
        StartGame(new Host());
    }

    private void StartClient() {
        StartGame(new Client(mIp?.Trim() ?? ""));
    }

    // -- c/helpers
    private void StartGame(Session session) {
        Game.Start(session);

        // advance to the loading scene
        var i = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(i + 1);
    }

    // -- events --
    // -- e/host
    public void didClickStart() {
        StartHost();
    }

    // -- e/join
    public void didChangeIp(InputField field) {
        mIp = field.text;
        Debug.Log(mIp);
    }

    public void didClickJoin() {
        StartClient();
    }
}
