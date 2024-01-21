using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        if(GameController.Instance.PlayerPrefab == null)
            return;
        
        var position = GameController.Instance.PlayerPrefab.transform.position;
        transform.position = new Vector3(position.x, position.y, -10.0f);
    }
}
