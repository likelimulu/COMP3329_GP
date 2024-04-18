using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class red_eagle_bulletScript : MonoBehaviour
{
    private float speed = 3f; // 子弹速度
    private float rotationSpeed = 200f; // 自旋速度

    private Transform player; // 玩家的位置
    private Vector3 direction; // 子弹移动方向

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        direction = (player.position - transform.position).normalized;
    }

    private void Update()
    {
        // 子弹移动
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // 子弹自旋
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D obj)
    {
        string name = obj.gameObject.name;

        // if collided with bullet
        if (name == "player")
        {
            PlayerScript playerController = player.GetComponent<PlayerScript>();
            playerController.BeAttacked();

            Destroy(gameObject);
        }
    }
}
