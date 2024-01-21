using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This class controls the level and spawning of ground object on random position and notifies other player when new ground is available.
/// </summary>
public class LevelController : MonoBehaviourPunCallbacks
{
    [Header("ground object")]
    [SerializeField] private GameObject groundObject;
    [SerializeField] private int poolSize = 30;
    [SerializeField] private int topLevel = 100;
    [SerializeField] private float groundObjectMaxWidth = 10f;
    [SerializeField] private float groundObjectMinWidth = 10f;
    [SerializeField] private float groundObjectMaxHeightDistance = 10f;
    [SerializeField] private float groundObjectMinHeightDistance = 10f;
    [SerializeField] private float groundObjectSpawnMaxXDistance = 10f;
    [SerializeField] private float groundObjectSpawnMinXDistance = 10f;

    [Header("timer")] 
    [SerializeField] private TextMeshProUGUI timerTextField;
    [SerializeField] private float timerInSeconds = 60.0f;

    [Header("other")] 
    [SerializeField] private TextMeshProUGUI enemyDeadTextField;
    [SerializeField] private TextMeshProUGUI playerCountTextField;
    [SerializeField] private GameObject multiUsePopupPrefab;
    [SerializeField] private GameObject redLineObject;
    
    private Transform firstGroundObject;
    private Transform lastGroundObject;
    private Queue<Transform> groundObjectPool = new Queue<Transform>();
    private Coroutine timerCoroutine;
    private int currentSpawnCount = 0;

    private int playerCount = 0;
    private Coroutine playerDeadCoroutine;
    
    // Start is called before the first frame update
    void Start()
    {
        groundObjectPool.Clear();
        InitialSetup();
        GameController.Instance.onNewGroundAvailableAction += OnNewGroundAvailable;
        GameController.Instance.onPlayerWon += OnPlayerWon;
        GameController.Instance.onPlayerDead += OnPLayerDead;
        StartTimer();
        playerCount = PhotonNetwork.CurrentRoom.Players.Count;
        SetPlayerLeftText();
        redLineObject.SetActive(true);
    }

    #region Ground Setup
    
    /// <summary>
    /// Randomly spawn ground object at the start of the game of pool size.
    /// </summary>
    private void InitialSetup()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < poolSize; i++)
            {
                SpawnGroundObject();
            }
        }
       
    }
    
    private void OnNewGroundAvailable()
    {
        SpawnGroundObject(true);
    }
    
    /// <summary>
    /// Show the popup when the player won.
    /// </summary>
    /// <param name="playerName">player name who won</param>
    private void OnPlayerWon(string playerName)
    {
        StopTimer();
        ShowMultiUsePopup(false, playerName);
    }
    
    /// <summary>
    /// if isSelfDead is true then show the popup for only player else show the popup for all player.
    /// </summary>
    /// <param name="isSelfDead"></param>
    /// <param name="playerName"></param>
    private void ShowMultiUsePopup(bool isSelfDead, string playerName)
    {
        if (isSelfDead)
        {
            var go =Instantiate(multiUsePopupPrefab, Vector3.zero, Quaternion.identity);
            go.GetComponent<MultiUsePopupView>().Init();
        }
        else
        {
            PhotonNetwork.InstantiateRoomObject("VictoryPopup", Vector3.zero, Quaternion.identity, 0 , new object[] {playerName});
        }
        //Notify to disable the control
        GameController.Instance.DisableControl();
    }

    private void SetPlayerLeftText()
    {
        playerCountTextField.text = $"{playerCount} Player Left";
    }
    
    private void OnPLayerDead(string playerName)
    {
        playerCount--;
        SetPlayerLeftText();
        enemyDeadTextField.SetText($"{playerName} Dead");
        StopPlayerDeadCoroutine();
        playerDeadCoroutine = StartCoroutine(PlayerDeadCoroutine());
        if (playerCount == 1 && PhotonNetwork.IsMasterClient) //if only one player is left then the last player is winner
        {
            var player = PhotonNetwork.CurrentRoom.Players.FirstOrDefault(x => x.Value.NickName != playerName);
            if (player.Value != null)
            {
                OnPlayerWon(player.Value.NickName);
            }
        }
        else if (playerName.Equals(PhotonNetwork.NickName))
        {
            ShowMultiUsePopup(true, playerName);
        }
    }
    
    private IEnumerator PlayerDeadCoroutine()
    {
        yield return new WaitForSeconds(1f);
        enemyDeadTextField.SetText("");
    }
    
    private void StopPlayerDeadCoroutine()
    {
        if (playerDeadCoroutine != null)
        {
            StopCoroutine(playerDeadCoroutine);
        }
    }
    
    /// <summary>
    /// Randomly picks ground scale and position and spawn the ground object. and notifies all other player to spawn the ground object.
    /// </summary>
    /// <param name="canDeque"></param>
    public void SpawnGroundObject(bool canDeque = false)
    {
        if (!PhotonNetwork.IsMasterClient || currentSpawnCount >= topLevel)
        {
            return;
        }
        var groundScale = GetRandomSize();
        var groundObjectPosition = GetRandomPosition();
        if (canDeque && groundObjectPool.Count > 0)
        {
            var go = groundObjectPool.Dequeue().gameObject;
            if (go != null)
            {
                Destroy(go);
            }
        }
        currentSpawnCount++;
        bool isLastGround = currentSpawnCount == topLevel;
        //Send the scale and last ground flag to all other players
        var spawnedGroundObject = PhotonNetwork.InstantiateRoomObject(groundObject.name, groundObjectPosition, Quaternion.identity, 0 , new object[] {groundScale, isLastGround});
        spawnedGroundObject.GetComponent<GroundController>().Init(groundScale);
        groundObjectPool.Enqueue(spawnedGroundObject.transform); //Add to queue //TODO: we can use object pool here
        lastGroundObject = spawnedGroundObject.transform;
        
    }

    /// <summary>
    /// Randomly picks width between groundObjectMinWidth and groundObjectMaxWidth
    /// </summary>
    /// <returns></returns>
    private Vector2 GetRandomSize()
    {
        var groundObjectWidth = groundObjectPool.Count == 0 ? 20 : Random.Range(groundObjectMinWidth, groundObjectMaxWidth); //set first ground object width to 20
        return new Vector2(groundObjectWidth, 1.0f);
    }
    
    /// <summary>
    /// Give new ground position based on the last ground position and last ground scale, as so it wont overlap with last ground object.
    /// </summary>
    /// <returns>new ground position</returns>
    private Vector3 GetRandomPosition()
    {
        var groundObjectPosition = Vector3.zero;
        if (groundObjectPool.Count > 0)
        {
            var groundObjectWidth = Random.Range(groundObjectSpawnMinXDistance, groundObjectSpawnMaxXDistance);
            var groundObjectHeight = Random.Range(groundObjectMinHeightDistance, groundObjectMaxHeightDistance);
            var lastGroundObjectPosition = lastGroundObject.position;
            var distanceByScale = groundObjectPool.Count == 1 ? 0 : 1 + (lastGroundObject.localScale.x / 2.0f);
            var lastGroundObjectXSize = groundObjectWidth < 0 ? groundObjectWidth -(distanceByScale) : groundObjectWidth + (distanceByScale);
            groundObjectPosition = new Vector3(lastGroundObjectPosition.x + lastGroundObjectXSize, lastGroundObjectPosition.y + groundObjectHeight, 0f);
        }
        else
        {
            groundObjectPosition = new Vector3(0f, -4.5f, 0f); //Spawn the first ground object at the bottom fixed position
        }
        return groundObjectPosition;
    }
    #endregion

    #region timerRegion

    private void StartTimer()
    {
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }
    
    /// <summary>
    /// Target timer for player to complete the game
    /// </summary>
    /// <returns></returns>
    private IEnumerator TimerCoroutine()
    {
        var timerInSeconds = this.timerInSeconds;
        while (timerInSeconds > 0)
        {
            timerInSeconds--;
            var mins = Mathf.FloorToInt(timerInSeconds / 60);
            var seconds = Mathf.FloorToInt(timerInSeconds % 60);
            timerTextField.text = $"{mins:00}:{seconds:00}";
            
            yield return new WaitForSeconds(1f);
            
            if (Math.Abs(timerInSeconds - 60.0f) < 0.1f)
            {
                timerTextField.color = Color.yellow;
            }
            else if (Math.Abs(timerInSeconds - 30.0f) < 0.1f)
            {
                timerTextField.color = Color.red;
            }
        }
        
        OnTimerExpires();
    }
    
    private void OnTimerExpires()
    {
        timerTextField.SetText("Couldn't Complete Within Time Limit<br>Better Luck Next Time");
        StopTimer();
    }
    
    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    #endregion
    

    private void OnDestroy()
    {
        StopTimer();
        StopPlayerDeadCoroutine();
        GameController.Instance.onNewGroundAvailableAction -= OnNewGroundAvailable;
        GameController.Instance.onPlayerWon -= OnPlayerWon;
    }
}
