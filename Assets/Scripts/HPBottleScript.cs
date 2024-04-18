using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBottleScript : MonoBehaviour
{
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D obj)
    {
        string name = obj.gameObject.name;

        if (name == "player")
        {
            //AudioSource.PlayClipAtPoint(myClip, transform.position);

            PlayerScript playerController = player.GetComponent<PlayerScript>();
            playerController.eatHPBottle();
            Destroy(gameObject);
        }
    }
}
