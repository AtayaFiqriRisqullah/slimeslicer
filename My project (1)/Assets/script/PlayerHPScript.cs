using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class playerHPSystem : MonoBehaviour
{
    [Header("HP")]
    public int maxHP = 100;
    public int currentHP;
    public int currentWave = 1;
    public float hitBoxSize = 1f;
    public float gameoverTime = 0.90f;

    [Header("binding")]
    public Transform hitBox;
    public LayerMask enemyLayers;
    public LayerMask UI;
    public Transform healthBar;
    public Rigidbody2D rb;
    public gameOverScreen gameoverscreen;
    public PlayerScript playerScript;

    public audioManager audioManager;

    void Start()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        healthBar.GetComponent<HealthBar>().SetMaxHealth(maxHP);
        healthBar.GetComponent<HealthBar>().SetHealth(currentHP);
    }

    public void takeDamage(int damage)
    {
        audioManager.PlaySFX(audioManager.damaged);
        if (currentHP > 0)
        {
            currentHP -= damage;
        }

        if (currentHP <= 0)
        {
            audioManager.PlaySFX(audioManager.gameOver);
            playerScript.isDead = true;
            StartCoroutine(gameOverTiming());
        }
    }
    private IEnumerator gameOverTiming()
    {
        yield return new WaitForSeconds(gameoverTime);
        gameoverscreen.Setup(currentWave);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(hitBox.position, hitBoxSize);
    }
}
