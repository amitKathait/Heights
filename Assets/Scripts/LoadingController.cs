using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LoadingController : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    //Connect to photon pun server
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 10;
        PhotonNetwork.LocalPlayer.NickName = "Player_" + Random.Range(0, 1000); //Random name for player
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// On Connection to master join lobby
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// Load next scene when joined lobby
    /// </summary>
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
