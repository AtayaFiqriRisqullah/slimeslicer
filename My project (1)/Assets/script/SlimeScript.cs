using System.Collections;
using UnityEngine;

public class Slime : MonoBehaviour
{
    [Header("binding")]
    public Animator animator;
    public GameObject player;
    public Rigidbody2D rb;
    public Transform slimeHitbox;
    public Transform slimeHitpoint;
    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public LayerMask groundLayer;
    public Transform groundCheck;


    [Header("atur atur bg")]
    public int MaxHealth = 100;
    public int currentHealth;
    public int slimeDamage = 10;
    public float hitBoxSize = 0.1f;
    public float knockbackRate = 5f;

    public float preJumpDelay = 0.3f; 
    public float jumpInterval = 3f;
    public float jumpForceX = 3f;
    public float jumpForceY = 5f;

    [Header("booleanjir")]
    public bool isDead = false;
    public bool damageBool = false;
    private bool isFacingRight = true;

    [Header("nyerang")]
    private float lastAttackTime;
    public float attackCooldown = 1.0f;

    audioManager audioManager;

    private void Awake()
    {
        GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
        audioManager = audioObj.GetComponent<audioManager>();
    }
    void Start()
    {
        currentHealth = MaxHealth;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            player = playerObj;
        }

        StartCoroutine(SlimeRoutine());
    }

    void Update()
    {

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);


        if (isGrounded())
        {
            if (player.transform.position.x > transform.position.x && !isFacingRight)
            {
                Flip();
            }
            else if (player.transform.position.x < transform.position.x && isFacingRight)
            {
                Flip();
            }
        }
        

        // attack
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    private IEnumerator SlimeRoutine()
    {
        while (!isDead) 
        {
            if (!isDead)
            {
                yield return new WaitWhile(() => damageBool);

                yield return new WaitForSeconds(jumpInterval);

                animator.SetBool("isJumping", true);
                animator.SetBool("isGrounded", true); 

                yield return new WaitForSeconds(preJumpDelay);

                Jump();
                animator.SetBool("isGrounded", false); 

                yield return new WaitForSeconds(0.3f);

                yield return new WaitUntil(() => isGrounded());

                animator.SetBool("isGrounded", true);

                animator.SetBool("isJumping", false);
            }
        }
    }

    private bool isGrounded()
    {
        if (groundCheck == null) return false;

        int combinedLayer = groundLayer | enemyLayer | playerLayer;

        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, combinedLayer);
    }


    void Jump()
    {
        float direction = isFacingRight ? 1 : -1;

        rb.AddForce(new Vector2(direction * jumpForceX, jumpForceY), ForceMode2D.Impulse);

    }
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    public IEnumerator damageTime() // animation
    {
        animator.Play("Damage");

        yield return new WaitForSeconds(0.5f);
        damageBool = false;
    }
    public void takeDamage(int damage)
    {

        damageBool = true;
        StartCoroutine(damageTime());
        currentHealth -= damage;
        audioManager.PlaySFX(audioManager.hit);

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    public void knockback(float forceX, float forceY)
    {
        float direction = isFacingRight ? 1 : -1;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(-direction * forceX, forceY), ForceMode2D.Impulse);
    }

    public void Attack()
    {
        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(slimeHitpoint.position, hitBoxSize, playerLayer);

        foreach (Collider2D target in hitPlayer)
        {
            var playerHP = target.GetComponent<playerHPSystem>();
            playerHP.takeDamage(slimeDamage);
            player.GetComponent<PlayerScript>().knockback(knockbackRate, knockbackRate, transform);
            lastAttackTime = Time.time; 
        }
    }

    private IEnumerator Die()
    {
        isDead = true;
        animator.SetBool("isDead", true);
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(0.90f);
        Destroy(gameObject);
        this.enabled = false;
    }
    private void OnDrawGizmosSelected() 
    {
        // display hitbox
        Gizmos.DrawWireSphere(slimeHitbox.position, hitBoxSize);
        Gizmos.DrawWireSphere(slimeHitpoint.position, hitBoxSize);
        Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
    }
}
