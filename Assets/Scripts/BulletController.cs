using Photon.Pun;
using UnityEngine;

public class BulletController : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float speed;
    [SerializeField] private float lifeTime;
    
    private float missileWidth;
    private float missileHeight;
    private Vector3 movementVector;
    private float currentLifeTime;
    
    public void InitBullet(Vector3 direction)
    {
        movementVector = direction;
    }

    private void Update()
    {
        MoveBullet();
    }

    private void MoveBullet()
    {
        transform.Translate(movementVector * (speed * Time.deltaTime));
        currentLifeTime += Time.deltaTime;
    }
    
    private void LateUpdate()
    {
       CheckBounds();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Bullet"))
        {
           Destroy(other.gameObject);
           Destroy(this.gameObject);
        }
    }
    
    /// <summary>
    /// Check if the bullet has been moved for more than its lifetime
    /// </summary>
    private void CheckBounds()
    {
        if(currentLifeTime > lifeTime)
            Destroy(gameObject);
    }

    /// <summary>
    /// Once the bullet is instantiated by one player,
    /// we will get the direction from the player and move the bullet in that direction to do local calculations
    /// to save the network bandwidth
    /// </summary>
    /// <param name="info">info for the spawned object</param>
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var data = info.photonView.InstantiationData;
        if (data != null)
        {
            InitBullet((Vector3)data[0]);
        }
    }
}
