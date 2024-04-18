using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class red_eagleScript : MonoBehaviour
{
    private Transform player;
    private float movementSpeed = 3f;
    private float changeDirectionInterval = 2f;

    private float directionTimer;
    private bool shouldChasePlayer = false;
    private Vector3 currentDirection = new Vector3(0, 0, 0);
    private Vector3 direction = new Vector3(0,0,0);

    public Animator animator;

    private float attackSpeed = 4f; //attack speed
    private float slowdownDuration = 0.5f;
    private float speedupDuration = 0.5f;
    private float attackCooldown = 1f;
    public GameObject shootPrefab;

    private bool isAttacking = false;


    private float minX = -1f; // x 值的最小范围
    private float maxX = 1f; // x 值的最大范围
    private float minY = -1f; // y 值的最小范围
    private float maxY = 1f; // y 值的最大范围

    public GameObject redBottlePrefab;

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

            float attackRange = 7f; // 设置攻击的距离阈值
            if (distanceToPlayer < attackRange || isAttacking)
            {
                Attack();
            }
            else
            {
                // 追踪程序

                // 计时器递减
                directionTimer -= Time.deltaTime;

                // 判断怪物和玩家是否在同一个高度
                if (Mathf.Abs(transform.position.y - player.position.y) < 7f)
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
                    transform.position = Vector3.MoveTowards(transform.position, player.position, movementSpeed * Time.deltaTime);
                    direction = player.position - transform.position;
                }
                else
                {
                    // 在不同高度时左右往复运动
                    if (directionTimer <= 0f)
                    {
                        // 改变方向
                        float randomX = Random.Range(minX, maxX);
                        float randomY = Random.Range(minY, maxY);
                        if (randomX == 0f)
                        {
                            randomX = 0.01f;
                        }
                        if (randomY == 0f)
                        {
                            randomY = 0.01f;
                        }
                        if (transform.position.x < -8)
                        {
                            randomY = Random.Range(0f, maxX);
                        }
                        if (transform.position.x > 8)
                        {
                            randomY = Random.Range(minX, 0f);
                        }
                        if (transform.position.y < -3.4)
                        {
                            randomY = Random.Range(0f, maxY);
                        }
                        if (transform.position.y > 4)
                        {
                            randomY = Random.Range(minY, 0f);
                        }

                        currentDirection = new Vector3(randomX, randomY, 0);
                        currentDirection = currentDirection / currentDirection.magnitude;

                        directionTimer = changeDirectionInterval;
                    }

                    direction = currentDirection;

                    transform.Translate(direction * movementSpeed * Time.deltaTime);
                }

                //modify the facing direction
                if (direction.x > 0)
                {
                    Transform transform = GetComponent<Transform>();
                    Vector3 scale = transform.localScale;
                    scale.x = 3;
                    transform.localScale = scale;
                }
                else if (direction.x < 0)
                {
                    Transform transform = GetComponent<Transform>();
                    Vector3 scale = transform.localScale;
                    scale.x = -3;
                    transform.localScale = scale;
                }

            }

        }
        else
        {
            animator.SetBool("Attack", false);
        }
    }


    public void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(AttackCoroutine(player.position));
        }
    }

    private IEnumerator AttackCoroutine(Vector3 playerPosition)
    {
        isAttacking = true;
        animator.SetBool("Attack", true);
        Vector3 attackDirection = playerPosition - transform.position;
        attackDirection = attackDirection / attackDirection.magnitude;

        if (attackDirection.x > 0)
        {
            Transform transform = GetComponent<Transform>();
            Vector3 scale = transform.localScale;
            scale.x = 3;
            transform.localScale = scale;
        }
        else if (attackDirection.x < 0)
        {
            Transform transform = GetComponent<Transform>();
            Vector3 scale = transform.localScale;
            scale.x = -3;
            transform.localScale = scale;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / speedupDuration;
            float currentSpeed = Mathf.Lerp(0, attackSpeed, t);
            transform.Translate(attackDirection * currentSpeed * Time.deltaTime);
            yield return null;
        }

        Shoot();

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / slowdownDuration;
            float currentSpeed = Mathf.Lerp(attackSpeed, 0, t);
            transform.Translate(attackDirection * currentSpeed * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("Attack", false);

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void Shoot()
    {
        // 获取玩家位置
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        // 计算敌人朝向玩家的方向
        Vector3 direction = playerPosition - transform.position;
        direction.Normalize();

        // 计算子弹初始位置
        Vector3 bulletPosition = transform.position + direction * Mathf.Abs(transform.localScale.x) / 3;

        // 实例化子弹
        GameObject bullet = Instantiate(shootPrefab, bulletPosition, Quaternion.identity);
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
                float randomValue = Random.Range(0f, 1f);

                if (randomValue <= 0.5f)
                {
                    GameObject redBottle = Instantiate(redBottlePrefab, transform.position, Quaternion.identity);
                }

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


