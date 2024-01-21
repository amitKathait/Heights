using Photon.Pun;
using UnityEngine;

/// <summary>
/// This class is used to control the red line. which move constantly upwards. and kill player if player touches it.
/// and spawn new ground at top when it touches the ground.
/// </summary>
public class RedLineController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.0f;
    
    private void OnEnable()
    {
        GameController.Instance.disableControlAction += OnControlDisabled;
    }
    
    /// <summary>
    /// Disable the movement of the red line when the game is over.
    /// </summary>
    private void OnControlDisabled()
    {
        moveSpeed = 0;
    }

    void Update()
    {
        if (moveSpeed > 0)
        {
            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("OnCollisionEnter2D ismaster " + PhotonNetwork.IsMasterClient + " " + other.transform.tag);
        if (other.transform.CompareTag("Player"))
        {
            var playerController = other.transform.GetComponent<PlayerController>();
            if (playerController != null && playerController.IsSelf)
            {
               GameController.Instance.OnPlayerDead(); //Instant kill the player if it touches the red line
            }
        }
        else if (PhotonNetwork.IsMasterClient && other.transform.CompareTag("Ground")) //Spawn new ground only if this is the master client
        {
            GameController.Instance.OnNewGroundAvailable();
        }
    }

    private void OnDestroy()
    {
        GameController.Instance.disableControlAction -= OnControlDisabled;
    }
}
