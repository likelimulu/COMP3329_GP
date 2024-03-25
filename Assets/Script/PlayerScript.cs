using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 v;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        rb = GetComponent<Rigidbody2D>();
        v = rb.velocity;
        v.x = Input.GetAxis("Horizontal") * 5;
        v.y = 0;
        rb.velocity = v;

        animator.SetFloat("Horizontal", v.x);
        animator.SetFloat("Speed", v.sqrMagnitude);

        if (v.x > 0)
        {
            Transform transform = GetComponent<Transform>();
            Vector3 scale = transform.localScale;
            scale.x = 3;
            transform.localScale = scale;
        } else if (v.x < 0) {
            Transform transform = GetComponent<Transform>();
            Vector3 scale = transform.localScale;
            scale.x = -3;
            transform.localScale = scale;
        }


        /* if (Input.GetKeyDown("space"))
        {
            Instantiate(bullet, transform.position, Quaternion.identity);
        } */
    }
}