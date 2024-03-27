using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerScript : MonoBehaviour
{
    public int life = 3;
    public AudioClip myClip;
    private float delay = 1.5f; // 自身生命值<=0时等待销毁动画放完的时间

    public GameObject shootPrefab;
    private float shootRotationAngle = 270f;
    private float spawnOffset = 0.6f;
    private float shootSpeed = 1.0f;

    private Rigidbody2D rb;
    private Vector2 v;

    public Animator animator;
    public float jumpForce = 50f;

    private int jumpCount = 0;
    private int maxJumpCount = 2;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        v = rb.velocity;
        v.x = Input.GetAxis("Horizontal") * 5;
        rb.velocity = v;

        animator.SetFloat("Horizontal", Math.Abs(v.x));

        if (v.x > 0)
        {
            Transform transform = GetComponent<Transform>();
            Vector3 scale = transform.localScale;
            scale.x = 2;
            transform.localScale = scale;

            shootRotationAngle = 270f;
            spawnOffset = 0.5f;
            shootSpeed = 1.0f;
        }
        else if (v.x < 0)
        {
            Transform transform = GetComponent<Transform>();
            Vector3 scale = transform.localScale;
            scale.x = -2;
            transform.localScale = scale;

            shootRotationAngle = 90f;
            spawnOffset = -0.5f;
            shootSpeed = -1.0f;
        }

        // 获取attack变量的值
        bool isAttacking = animator.GetBool("Attack");

        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (jumpCount < maxJumpCount)
            {
                Jump();
                jumpCount++;
            }
        }

        if (life <= 0)
        {
            animator.SetBool("Alive", false);

            StartCoroutine(DestroyAfterDelay());
        }
    }

    void Attack()
    {
        // 设置攻击状态为 true
        animator.SetBool("Attack", true);

        GameObject shoot = Instantiate(shootPrefab, transform.position, Quaternion.identity);
        shoot.transform.rotation = Quaternion.Euler(0f, 0f, shootRotationAngle);

        // 进行shoot的位置调整
        Vector3 offset = transform.right * spawnOffset;
        shoot.transform.position += offset;

        Rigidbody2D shootRigidbody = shoot.GetComponent<Rigidbody2D>();
        Vector2 v = shootRigidbody.velocity;
        v.x = shootSpeed * 8;
        shootRigidbody.velocity = v;

        // 攻击完成后重置攻击状态为 false
        StartCoroutine(ResetAttackState());
    }

    IEnumerator ResetAttackState()
    {
        // 假设攻击持续 1 秒钟
        yield return new WaitForSeconds(1f);

        // 重置攻击状态为 false
        animator.SetBool("Attack", false);
    }

    public void BeAttacked()
    {
        // 设置攻击状态为 true
        animator.SetBool("BeAttacked", true);
        life--;

        // 攻击完成后重置攻击状态为 false
        StartCoroutine(ResetBeAttackedState());
    }

    IEnumerator ResetBeAttackedState()
    {
        // 假设攻击持续 1 秒钟
        yield return new WaitForSeconds(1f);

        // 重置攻击状态为 false
        animator.SetBool("BeAttacked", false);
    }

    private void Jump()
    {
        // 在这里实现跳跃逻辑，例如给角色施加向上的力
        v = rb.velocity;
        v.y = 0f;
        rb.velocity = v;
        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        animator.SetBool("Jump", true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0; // 角色着陆后重置跳跃次数
            animator.SetBool("Jump", false);
        }
    }
    private IEnumerator DestroyAfterDelay()
    {
        // 等待一段时间
        yield return new WaitForSeconds(delay);

        // 销毁当前对象
        Destroy(gameObject);
    }

}