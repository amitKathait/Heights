using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used to show the popup when the game is over.
/// </summary>
public class MultiUsePopupView : MonoBehaviour,IPunInstantiateMagicCallback
{
    [SerializeField] private TextMeshProUGUI playerWonTextField;
    [SerializeField] private Button homeButton;
    
    // Start is called before the first frame update
    void Start()
    {
        homeButton.onClick.AddListener(OnHomeButtonClicked);
    }

    public void Init()
    {
        playerWonTextField.SetText("Game Over");
    }
    /// <summary>
    /// GO back to lobby when home button is clicked.
    /// </summary>
    private void OnHomeButtonClicked()
    {
       GameController.Instance.OnGameOver();
    }

    private void OnDestroy()
    {
        homeButton.onClick.RemoveListener(OnHomeButtonClicked);
    }
    
    /// <summary>
    /// Process the data sent from the photon network and set the player won text.
    /// </summary>
    /// <param name="info">extra info</param>
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var data = info.photonView.InstantiationData;
        if (data != null)
        {
            playerWonTextField.text = (string)data[0] +" Won!";
        }
    }
}
