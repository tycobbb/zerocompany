using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

sealed class Menu: MonoBehaviour {
    // -- props --
    private string mIp;

    // -- private
    private Client mClient;

    // -- p/fields
    [SerializeField]
    [Tooltip("The network prefab.")]
    private GameObject mNetworkPrefab;

    // -- lifecycle --
    private void Update() {
        StartGameOnConnect();
    }

    // -- commands --
    private void StartHost() {
        StartSession(new Session.Host());
    }

    private void StartClient() {
        StartSession(new Session.Client(mIp?.Trim() ?? ""));
    }

    private void StartGameOnConnect() {
        if (!mClient) {
            return;
        }

        if (mClient.IsConnected()) {
            StartGame();
        } else if (mClient.IsDisconnected()) {
            Destroy(mClient.gameObject);
            mClient = null;
        }
    }

    // -- c/helpers
    private void StartSession(Session session) {
        Session.Start(session);

        // instantiate the network
        var network = Instantiate(mNetworkPrefab);
        mClient = network.GetComponent<Client>();
    }

    private void StartGame() {
        DontDestroyOnLoad(mClient.gameObject);

        // advance to the game scene
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
