using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// This class act as a notifier between different class and notifies the changes in one class to another class
/// </summary>
public class GameController : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Singleton object to share between class
    /// </summary>
    private static GameController _instance;
    public static GameController Instance => _instance;
    
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;
    [SerializeField] private Button homeButton;
    
    private GameObject spawnedPlayer;
    public GameObject PlayerPrefab => spawnedPlayer;
    
    private PlayerController selfPlayerController;
    public PlayerController SelfPlayerController => selfPlayerController;
    
    public Action<bool> onLeftButtonClickedAction;
    public Action<bool> onRightButtonClickedAction;
    public Action<bool> onJumpButtonClickedAction;
    public Action<bool> onAttackButtonClickedAction;
    public Action onNewGroundAvailableAction;
    public Action<string> onPlayerWon;
    public Action<string> onPlayerDead;
    public Action disableControlAction;
    
    private bool isHomeButtonClicked;
    
    // Start is called before the first frame update
    void Start()
    {
        if(_instance == null)
            _instance = this;
        
        //Spawning player at a random location
        Vector2 randomPosition = new Vector2(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y));
        spawnedPlayer = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        selfPlayerController = spawnedPlayer.GetComponent<PlayerController>();
        selfPlayerController.IsSelf = true;
        homeButton.onClick.AddListener(OnHomeButtonClicked);
    }

    #region button Action

    private void OnHomeButtonClicked()
    {
        OnGameOver();
    }
    
    /// <summary>
    /// Notifies respective class for button event
    /// </summary>
    /// <param name="pressed">if button is clicked</param>
    private void OnLeftButtonClicked(bool pressed)
    {
        onLeftButtonClickedAction?.Invoke(pressed);
    }
    
    /// <summary>
    /// Notifies respective class for button event
    /// </summary>
    /// <param name="pressed">if button is clicked</param>
    private void OnRightButtonClicked(bool pressed)
    {
        onRightButtonClickedAction?.Invoke(pressed);
    }
    
    /// <summary>
    /// Notifies respective class for button event
    /// </summary>
    /// <param name="pressed">if button is clicked</param>
    private void OnJumpButtonClicked(bool pressed)
    {
        onJumpButtonClickedAction?.Invoke(pressed);
    }
    
    /// <summary>
    /// Notifies respective class for button event
    /// </summary>
    /// <param name="pressed">if button is clicked</param>
    private void OnAttackButtonClicked(bool pressed)
    {
        onAttackButtonClickedAction?.Invoke(pressed);
    }
    
    /// <summary>
    /// Notifies respective class to disable control or movement
    /// </summary>
    /// <param name="pressed">if button is clicked</param>
    public void DisableControl()
    {
        disableControlAction?.Invoke();
    }
    
    /// <summary>
    /// Check which button is pressed and notify respective class
    /// </summary>
    /// <param name="buttonType">button type</param>
    /// <param name="pressed">if pressed or released</param>
    public void OnButtonPressed(ButtonType buttonType, bool pressed)
    {
        switch (buttonType)
        {
            case ButtonType.Left:
                OnLeftButtonClicked(pressed);
                break;
            case ButtonType.Right:
                OnRightButtonClicked(pressed);
                break;
            case ButtonType.Jump:
                OnJumpButtonClicked(pressed);
                break;
            case ButtonType.Attack:
                OnAttackButtonClicked(pressed);
                break;
        }
    }
    
    #endregion

    /// <summary>
    /// Notifies respective class when player is dead
    /// </summary>
    public void OnPlayerDead()
    {
        Destroy(PlayerPrefab);
        onPlayerDead?.Invoke(PhotonNetwork.NickName);
    }
    
    public void OnEnemyDead(string playerName)
    {
        onPlayerDead?.Invoke(playerName);
    }
    
    /// <summary>
    /// Leave room on game over
    /// </summary>
    public void OnGameOver()
    {
        PhotonNetwork.LeaveRoom();
    }
    
    /// <summary>
    /// back to lobby on left room
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(1);
    }
    
    /// <summary>
    /// Notifies respective class when new ground is available
    /// </summary>
    public void OnNewGroundAvailable()
    {
        onNewGroundAvailableAction?.Invoke();
    }
    
    /// <summary>
    /// Notifies respective class when player won
    /// </summary>
    /// <param name="playerName"></param>
    public void OnPlayerWon(string playerName)
    {
        onPlayerWon?.Invoke(playerName);
    }

    private void OnDestroy()
    {
        homeButton.onClick.RemoveListener(OnHomeButtonClicked);
    }
}
