using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody ballRb;
    private float destroyDelay = 13f;
    
    // Start is called before the first frame update
    void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        ballRb.AddExplosionForce(5, transform.position, 0, 1);
        if (Data.Instance.gameManager != null)
            Data.Instance.gameManager.dirtyFloorItems.Add(gameObject);
        
    }
}
