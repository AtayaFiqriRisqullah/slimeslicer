using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField]private float horizontal = 0f;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpingPower = 24f;

    [Header("Dash Settings")]
    [SerializeField] private float dashingPowerX = 24f;
    [SerializeField] private float dashingPowerY = 20f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingCooldown = 1f;
    public int stamina;
    public int maxStamina = 2;
    public int dashCost = 1;
    public float dashGravity = 0f;

    [Header("Bind")]
    public Transform staminaBar;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public TrailRenderer tr;
    public Animator animator;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    public LayerMask UI;
    public playerHPSystem playerHPSystem;
    public audioManager audioManager;


    [Header("Attack")]
    public int attackCount = 1;
    public float attackRange = 0.5f;
    public float knockbackRate = 10f;
    public float attackRate = 1f;
    float nextAttackTime = 0f;
    public int attackDamage = 30;

    [Header("booleanjir")]
    private bool canDash = true;
    private bool isDashing;
    private bool isFacingRight = true;
    private bool isKnockback = false;
    public bool isDead = false;
    public bool hasDied = false;

    void Update()
    {
        // mati:P
        if (isDead)
        {
            if (!hasDied)
            {
                animator.Play("Player_Die");

                hasDied = true; 

                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = 0;
                rb.bodyType = RigidbodyType2D.Kinematic;
                GetComponent<Collider2D>().enabled = false;

                StartCoroutine(Die());
            }
            return;
        }
        //movement
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocityY > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);
        }

        if(isGrounded())
        {
            speed = 4f;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 10f;
        }

        flip();

        // stamina bar
        staminaBar.GetComponent<StaminaBar>().SetMaxStamina(maxStamina);
        staminaBar.GetComponent<StaminaBar>().SetStamina(stamina);

        //dash
        if (stamina < 1)
        {
            canDash = false;
        }
        if (stamina >= 1)
        {
            canDash = true;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.L) && canDash && (moveX != 0 || moveY != 0))
        {
            StartCoroutine(Dash());
        }

        //animator
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        animator.SetFloat("yVelocity", Mathf.Clamp(rb.linearVelocity.y, -100f, 100f));

        animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));

        if (Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer))
        {
            animator.SetBool("isGrounded", true);
        }
        if (!Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer))
        {
            animator.SetBool("isGrounded", false);
        }

        // attack
        if (Input.GetKeyDown(KeyCode.J))
        {
            // attack function
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
                // attack animation
                if (attackCount == 1)
                {
                    attackCount = 2;
                    animator.Play("Attack1");
                }
                else if (attackCount == 2)
                {
                    attackCount = 1;
                    animator.Play("Attack2");
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDead || isDashing || isKnockback) return;
        

        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.90f);
        gameObject.SetActive(false);
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private void flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        animator.SetBool("isDashing", true);
        float originalGravity = 8f;
        rb.gravityScale = dashGravity;
        rb.linearVelocity = new Vector2(Input.GetAxisRaw("Horizontal") * dashingPowerX, Input.GetAxisRaw("Vertical") * dashingPowerY);
        tr.emitting = true;
        stamina -= dashCost;
        if (stamina < 0) stamina = 0;
        yield return new WaitForSeconds(dashingTime);
        if (!isGrounded())
        {
            speed = 10f;
        }
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        animator.SetBool("isDashing", false);
        yield return new WaitForSeconds(dashingCooldown);
        stamina += 1;
    }

    public void Attack()
    {

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Slime>().takeDamage(attackDamage);
            enemy.GetComponent<Slime>().knockback(knockbackRate, knockbackRate);
        }
    }

    public void knockback(float forceX, float forceY, Transform attacker)
    {
        StartCoroutine(KnockbackRoutine(forceX, forceY, attacker));
    }

    private IEnumerator KnockbackRoutine(float forceX, float forceY, Transform attacker)
    {
        isKnockback = true; 

        rb.linearVelocity = Vector2.zero;

        float direction = transform.position.x - attacker.transform.position.x;

        if (direction > 0) direction = 1;
        else direction = -1;
        
        rb.AddForce(new Vector2(direction * forceX, forceY), ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);

        yield return new WaitUntil(isGrounded);
        rb.linearVelocity = Vector2.zero;
        isKnockback = false; 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
