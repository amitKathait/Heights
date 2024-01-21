using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// This class handles the player listing menu and shows the list of players in the room.
/// </summary>
public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform content;
    [SerializeField] private PlayerListing playerListing;
    
    private List<PlayerListing> _listings = new List<PlayerListing>();
    
    public override void OnEnable()
    {
        base.OnEnable();
        GetCurrentRoomPlayers();
    }
    
    /// <summary>
    /// Init the player listing menu with the current players in the room.
    /// </summary>
    private void GetCurrentRoomPlayers()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;
        
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            PlayerJoinedRoom(player);
        }
    }
    
    /// <summary>
    /// On player enter the room add the player to the list.
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        PlayerJoinedRoom(newPlayer);
    }
    
    /// <summary>
    /// On player left the room remove the player from the list.
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        PlayerLeftRoom(otherPlayer);
    }
    
    private void PlayerJoinedRoom(Photon.Realtime.Player newPlayer)
    {
        if (newPlayer == null)
            return;
        
        PlayerLeftRoom(newPlayer);
        
        var listing = Instantiate(playerListing, content);
        if (listing != null)
        {
            listing.SetPlayerInfo(newPlayer);
            _listings.Add(listing);
        }
    }
    
    private void PlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }
    
}
