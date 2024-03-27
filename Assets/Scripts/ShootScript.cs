using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{

    private int speed = -6;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Gets called when the object goes out of the screen
    void OnBecameInvisible()
    {
        // Destroy the bullet
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
