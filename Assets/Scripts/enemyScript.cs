using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class enemyScript : MonoBehaviour
{
    // Public variable that contains the speed of the enemy
    public int speed = -5;
    public AudioClip myClip;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 v = rb.velocity;
        v.x = speed;
        v.y = 0;
        rb.velocity = v;
        Destroy(gameObject, 3);

        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    //function called when the enemy collides with another object
    void OnTriggerEnter2D(Collider2D obj)
    {
        string name = obj.gameObject.name;

        // if collided with bullet
        if (name == "shoot(Clone)")
        {
            AudioSource.PlayClipAtPoint(myClip, transform.position);

            // Destroy itself (the enemy)
            Destroy(gameObject);

            // And destroy the bullet
            Destroy(obj.gameObject);
        }

        if (name == "player")
        {
            AudioSource.PlayClipAtPoint(myClip, transform.position);

            PlayerScript playerController = player.GetComponent<PlayerScript>();
            playerController.BeAttacked();

            // Destroy itself (the enemy)
            Destroy(gameObject);
        }
    }
}
