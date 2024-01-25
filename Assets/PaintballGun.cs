using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintballGun : MonoBehaviour
{
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private float speed = 10.0f;
    [SerializeField] public GameObject paint;

    public void DoThrow()
    {
        if (paint != null)
        {
            var rotation = transform.rotation;
                
                Quaternion.Euler(new Vector3(0,0,1));

            // Loop through all prefabs and spawn them
            var clone = Instantiate(paint, SpawnPoint.position, rotation);

            clone.SetActive(true);

            // Throw with velocity?
            var cloneRigidbody = clone.GetComponent<Rigidbody>();

            if (cloneRigidbody != null)
            {
                cloneRigidbody.velocity = clone.transform.forward * speed;
            }
        }
    }
}
