using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerListing : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    
    public Player Player;
    
    public void SetPlayerInfo(Player player)
    {
        Player = player;
        playerNameText.text = player.NickName;
    }
}
