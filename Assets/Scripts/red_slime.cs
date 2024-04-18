using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class red_slimeScript : MonoBehaviour
{
    private Transform player;
    private float movementSpeed = 3f;
    private float changeDirectionInterval = 3f;
    private float distance = 3f;

    private float directionTimer;
    private bool shouldChasePlayer = false;
    private int currentDirection = 1;
    private float direction = 0;

    public Animator animator;

    private float attackSpeed = 3f; //attack speed
    private float slowdownDuration = 0.6f;
    private float speedupDuration = 0.4f;
    private float attackCooldown = 0.8f;

    private bool isAttacking = false;


    void Start()
    {
        directionTimer = changeDirectionInterval;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player != null)
        {
            //checking attack
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // 追踪程序

            // 计时器递减
            directionTimer -= Time.deltaTime;

            // 判断怪物和玩家是否在同一个高度
            if (Mathf.Abs(transform.position.y - player.position.y) < 1f)
            {
                shouldChasePlayer = true;
            }
            else
            {
                shouldChasePlayer = false;
            }

            // 根据是否追踪玩家执行相应的行为
            if (shouldChasePlayer)
            {
                // 追踪玩家
                Attack(player.position);

                currentDirection = Random.Range(-1, 2);

                direction = Mathf.Sign(player.position.x - transform.position.x);
            }
            else
            {
                // 在不同高度时左右往复运动
                if (directionTimer <= 0f)
                {
                    // 改变方向
                    currentDirection *= -1;
                    directionTimer = changeDirectionInterval;
                }

                direction = Mathf.Sign(currentDirection);
                Vector3 targetPosition = new Vector3(transform.position.x + distance * direction, transform.position.y, transform.position.z);
                Attack(targetPosition);
            }
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
    }


    public void Attack(Vector3 attackPosition)
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackCoroutine(attackPosition));
        }
    }

    private IEnumerator AttackCoroutine(Vector3 attackPosition)
    {
        isAttacking = true;
        animator.SetFloat("Speed", attackSpeed);
        float attackDirection = Mathf.Sign(attackPosition.x - transform.position.x);

        if (attackDirection > 0)
        {
            Transform transform = GetComponent<Transform>();
            Vector3 scale = transform.localScale;
            scale.x = -3;
            transform.localScale = scale;
        }
        else if (attackDirection < 0)
        {
            Transform transform = GetComponent<Transform>();
            Vector3 scale = transform.localScale;
            scale.x = 3;
            transform.localScale = scale;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / speedupDuration;
            float currentSpeed = Mathf.Lerp(0, attackSpeed, t);
            transform.Translate(new Vector3(attackDirection * currentSpeed * Time.deltaTime, 0f, 0f));
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / slowdownDuration;
            float currentSpeed = Mathf.Lerp(attackSpeed, 0, t);
            transform.Translate(new Vector3(attackDirection * currentSpeed * Time.deltaTime, 0f, 0f));
            yield return null;
        }

        animator.SetFloat("Speed", 0);

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private int monsterHealth = 3;

    void OnTriggerEnter2D(Collider2D obj)
    {
        string name = obj.gameObject.name;

        // if collided with bullet
        if (name == "player_bullet(Clone)")
        {
            //AudioSource.PlayClipAtPoint(myClip, transform.position);

            // Reduce monster health
            monsterHealth -= 1;
            StartCoroutine(SlowDownMonster());

            // Check if monster is dead
            if (monsterHealth <= 0)
            {
                // Destroy the monster
                Destroy(gameObject);
            }
            else
            {
                // Make the monster blink
                StartCoroutine(BlinkMonster());

                // Play sound effect (optional)
                // AudioSource.PlayClipAtPoint(myClip, transform.position);
            }

            // And destroy the bullet
            //Destroy(obj.gameObject);
        }
    }
    IEnumerator BlinkMonster()
    {
        // Get the original color of the monster
        Color originalColor = GetComponent<SpriteRenderer>().color;

        // Blink the monster for a few times
        for (int i = 0; i < 3; i++)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SlowDownMonster()
    {
        movementSpeed = 1f;

        // 等待减缓时间
        yield return new WaitForSeconds(0.3f);

        movementSpeed = 3f;
    }

    void OnCollisionEnter2D(Collision2D obj)
    {
        string name = obj.gameObject.name;

        if (name == "player")
        {
            //AudioSource.PlayClipAtPoint(myClip, transform.position);

            PlayerScript playerController = player.GetComponent<PlayerScript>();
            playerController.BeAttacked();
        }
    }
}


