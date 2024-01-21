using Photon.Pun;
using UnityEngine;

/// <summary>
/// This class set the ground object scale and also set if this is the last ground.
/// </summary>
public class GroundController : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    public bool IsLastGround;
    
   public void Init(Vector3 localScale)
   {
      transform.localScale = localScale;
   }
   
   /// <summary>
   /// Set the ground scale and last ground flag when the ground is instantiated.
   /// </summary>
   /// <param name="info">extra info recieved</param>
   public void OnPhotonInstantiate(PhotonMessageInfo info)
   {
       var data = info.photonView.InstantiationData;
       if (data != null)
       {
           Init((Vector2)data[0]);
           IsLastGround = (bool)data[1];
       }
   }
}
