using Photon.Pun;
using TMPro;
using UnityEngine;

/// <summary>
/// This class handles the lobby and create/join room functionality.
/// </summary>
public class LobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI createInputField;
    [SerializeField] private TextMeshProUGUI joinInputField;
    [SerializeField] private GameObject createJoinRoomPanel;
    [SerializeField] private GameObject roomPlayerListingPanel;
    [SerializeField] private TextMeshProUGUI startGameButtonText;
    
    public void CreateRoom()
    {
        var roomName = createInputField.text;
        if(!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.CreateRoom(roomName);
        }
    }

    public void JoinRoom()
    {
        var roomName = joinInputField.text;
        if (!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }
    
    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            startGameButtonText.SetText("Waiting for host to start"); //Only Master client can start the game
        }
        ToggleListing(false);
    }
    
    private void ToggleListing(bool isCreateOrJoinEnabled) //Toggle between create/join room and player listing panel when room is created
    {
        createJoinRoomPanel.SetActive(isCreateOrJoinEnabled);
        roomPlayerListingPanel.SetActive(!isCreateOrJoinEnabled);
    }

    public void StartGameButtonClick()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; //Disable the room so no one else can join
            PhotonNetwork.CurrentRoom.IsVisible = false; 
            PhotonNetwork.LoadLevel("Game");
        }
    }
}
