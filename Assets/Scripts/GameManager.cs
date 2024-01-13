using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text AlertText;

    public bool IsPlayerDead => playerCurrentHealth <= 0;
    public float PlayerMaxHealth = 100f;
    public Slider PlayerHealthSlider;
    public Text PlayerHealthText;

    public bool IsEnemyDead => enemyCurrentHealth <= 0;
    public float EnemyMaxHealth = 100f;
    public Slider EnemyHealthSlider;
    public Text EnemyHealthText;

    GameObject playerHealthFillArea;
    float playerCurrentHealth;

    GameObject enemyHealthFillArea;
    float enemyCurrentHealth;

    void Awake()
    {
        //마우스 포인터 잠금 및 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AlertText.text = "";
        playerCurrentHealth = PlayerMaxHealth;
        enemyCurrentHealth = EnemyMaxHealth;
        playerHealthFillArea = PlayerHealthSlider.transform.Find("Fill Area").gameObject;
        enemyHealthFillArea = EnemyHealthSlider.transform.Find("Fill Area").gameObject;
    }

    void Update()
    {
        PlayerHealthSlider.value = playerCurrentHealth / PlayerMaxHealth;
        PlayerHealthText.text = $"{playerCurrentHealth} / {PlayerMaxHealth}";
        playerHealthFillArea.SetActive(!IsPlayerDead);

        EnemyHealthSlider.value = enemyCurrentHealth / EnemyMaxHealth;
        EnemyHealthText.text = $"{enemyCurrentHealth} / {EnemyMaxHealth}";
        enemyHealthFillArea.SetActive(!IsEnemyDead);
    }

    public void SetPlayerHealth(float maxHealth)
    {
        PlayerHealthSlider.value = 1;
        PlayerMaxHealth = maxHealth;
        playerCurrentHealth = PlayerMaxHealth;
    }

    public void SetEnemyHealth(float maxHealth)
    {
        EnemyHealthSlider.value = 1;
        EnemyMaxHealth = maxHealth;
        enemyCurrentHealth = EnemyMaxHealth;
    }

    void PlayerDead()
    {
        StartCoroutine(PlayerDeadCoroutine());
    }

    void EnemyDead()
    {
        AlertText.text = "You cleared!";
    }

    IEnumerator PlayerDeadCoroutine()
    {
        for (int i = 3; i > 0; i--)
        {
            AlertText.text = $"You died!\n{i} Seconds";
            yield return new WaitForSeconds(1.0f);
        }
        AlertText.text = "";
        SetPlayerHealth(100.0f);
        SetEnemyHealth(100.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void DamageToPlayer(float damage)
    {
        if (playerCurrentHealth > 0)
        {
            playerCurrentHealth -= damage;
            if (IsPlayerDead) PlayerDead();
        }
    }

    public void DamageToEnemy(float damage)
    {
        if (enemyCurrentHealth > 0)
        {
            enemyCurrentHealth -= damage;
            if (IsEnemyDead) EnemyDead();
        }
    }
}
