using Photon.Pun;
using UnityEngine;

public class PlayerController : MovementController
{
    [SerializeField] private GameObject bulletObject;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private HealthBarView healthBarView;
    [SerializeField] private float dragForce;
    
    private bool isSelf = false;
    private bool isAttackButtonPressed = false;
    
    //For mobile device
    private bool isWaitingForNextButtonClicked = false;
 
    /// <summary>
    /// True if this is the local player.
    /// </summary>
    public bool IsSelf
    {
        get => isSelf;
        set => isSelf = value;
    }
    
    protected override void Start()
    {
        base.Start();
        if (photonView.IsMine) //Listen to input only if this is the local player for mobile device
        {
            GameController.Instance.onAttackButtonClickedAction += OnAttackButtonClicked;
            GameController.Instance.onLeftButtonClickedAction += OnLeftButtonClicked;
            GameController.Instance.onRightButtonClickedAction += OnRightButtonClicked;
            GameController.Instance.onJumpButtonClickedAction += OnJumpButtonClicked;
            GameController.Instance.disableControlAction += OnControlDisabled;
        }
    }

    protected override void Update()
    {
        if (photonView.IsMine && isControlEnabled)
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.K) || (isAttackButtonPressed && !isWaitingForNextButtonClicked))
            {
                isWaitingForNextButtonClicked = true;
                SpawnBullet();
            }
        }
    }

    /// <summary>
    /// Spawns the bullet and all player does movement for bullet spawned locally
    /// </summary>
    private void SpawnBullet()
    {
        var movementVector = PlayerDirection == PlayerDirection.Right ? Vector3.right : Vector3.left; 
        PhotonNetwork.Instantiate(bulletObject.name, bulletSpawnPoint.position, Quaternion.identity,0, new object[] {movementVector});
    }
    
    /// <summary>
    /// Notified through GameController when control is disabled
    /// </summary>
    /// <param name="pressed">if button is pressed or released</param>
    private void OnAttackButtonClicked(bool pressed)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (isWaitingForNextButtonClicked && !pressed) //One we have received the click, we will wait for the pointer to be released before accepting the next click
        {
            isWaitingForNextButtonClicked = false;
        }
        isAttackButtonPressed = pressed;
    }
    
    private void OnControlDisabled()
    {
        isControlEnabled = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            healthBarView.TakeDamage(1, OnHealthOver); //When health is over, we will call OnHealthOver
        }
        else if (photonView.IsMine && other.transform.CompareTag("Ground"))
        {
            if (other.transform.GetComponent<GroundController>().IsLastGround)
            {
                isControlEnabled = false;
                //Notify other players that this player has won
                photonView.RPC("OnPlayerWon", RpcTarget.All, false);
                GameController.Instance.OnPlayerWon(PhotonNetwork.NickName);
            }
        }
    }
    
    [PunRPC]
    private void OnPlayerWon(bool isControlEnabled, PhotonMessageInfo info)
    {
        this.isControlEnabled = isControlEnabled;
    }

    private void OnHealthOver()
    {
        if (photonView.IsMine)
        {
            //Notify other players that this player has died
            photonView.RPC("OnPlayerDead", RpcTarget.All, PhotonNetwork.NickName);
            GameController.Instance.OnPlayerDead();
        }
    }

    [PunRPC]
    private void OnPlayerDead(string playerName, PhotonMessageInfo info)
    {
        var name = playerName;
        if (!name.Equals(PhotonNetwork.NickName))
        {
            GameController.Instance.OnEnemyDead(name);
        }
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            GameController.Instance.onAttackButtonClickedAction -= OnAttackButtonClicked;
            GameController.Instance.onLeftButtonClickedAction -= OnLeftButtonClicked;
            GameController.Instance.onRightButtonClickedAction -= OnRightButtonClicked;
            GameController.Instance.onJumpButtonClickedAction -= OnJumpButtonClicked;
            GameController.Instance.disableControlAction -= OnControlDisabled;
        }
    }
}
