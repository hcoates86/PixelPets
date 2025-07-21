using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        if (Data.Instance.currentChar.hasDied && other.gameObject.CompareTag("Ground"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Destroy(rb);

        }
    }
}
