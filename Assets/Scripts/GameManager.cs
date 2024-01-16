using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public ScriptableRendererFeature RendererFeature;

    public Text AlertText;
    public Health PlayerHealth;
    public List<EnemyBase> Enemies;

    public Slider EnemyHealthSlider;
    public Text EnemyHealthText;

    int enemyHealthIndex = 0;

    void Awake()
    {
        // Enemy 태그를 가진 모든 객체들을 찾아서 리스트에 추가
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemyObject in enemyObjects)
        {
            EnemyBase enemy = enemyObject.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                Enemies.Add(enemy);
            }
        }

        //마우스 포인터 잠금 및 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        RendererFeature.SetActive(false);
        AlertText.text = "";
    }

    void OnApplicationQuit()
    {
        RendererFeature.SetActive(false);
    }

    void Update()
    {
        PlayerHealth.Update();

        if (enemyHealthIndex >= 0)
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                Health health = Enemies[i].Health;

                if (i == enemyHealthIndex)
                {
                    health.HealthSlider = EnemyHealthSlider;
                    health.HealthText = EnemyHealthText;
                    health.Update();
                }
                else
                {
                    health.HealthSlider = null;
                    health.HealthText = null;
                }
            }

            Enemies[enemyHealthIndex].Health.Update();
        }
    }

    public void UpdateEnemyHealthIndex(Transform player)
    {
        int minIndex = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i].IsDestroyed()) continue;
            float distance = Vector3.Distance(Enemies[i].GetComponent<Transform>().position, player.position);
            if (distance < minDistance)
            {
                minIndex = i;
                minDistance = distance;
            }
        }

        enemyHealthIndex = minIndex;
    }

    public void SetPlayerHealth(float maxHealth)
    {
        PlayerHealth.SetMaxHealth(maxHealth);
    }

    public void SetEnemyHealth(int index, float maxHealth)
    {
        Enemies[index].Health.SetMaxHealth(maxHealth);
    }

    void PlayerDead()
    {
        StartCoroutine(PlayerDeadCoroutine());
    }

    void EnemyDead(Health health)
    {
        if (Enemies.Count(e => !e.Health.IsDead) == 0)
        {
            AlertText.text = "You cleared!";
        }
        else
        {
            AlertText.text = "You killed!";
        }

        StartCoroutine(ClearAlertCoroutine(2.0f));
    }

    IEnumerator ClearAlertCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        AlertText.text = "";
    }

    IEnumerator PlayerDeadCoroutine()
    {
        for (int i = 3; i > 0; i--)
        {
            AlertText.text = $"You died!\n{i} Seconds";
            yield return new WaitForSeconds(1.0f);
        }

        AlertText.text = "";
        SetPlayerHealth(PlayerHealth.MaxHealth);

        for (int i = 0; i < Enemies.Count; i++)
        {
            SetEnemyHealth(i, Enemies[i].Health.MaxHealth);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator DamageScreenCoroutine()
    {
        RendererFeature.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        RendererFeature.SetActive(false);
    }

    public bool DamageToPlayer(float damage)
    {
        if (PlayerHealth.IsDead) return true;
        StartCoroutine(DamageScreenCoroutine());
        PlayerHealth.Damage(damage);
        if (PlayerHealth.IsDead) PlayerDead();
        return PlayerHealth.IsDead;
    }

    public void HealToPlayer(float heal)
    {
        PlayerHealth.Heal(heal);
    }

    public bool DamageToEnemy(Health health, float damage)
    {
        if (health.IsDead) return true;
        health.Damage(damage);

        if (health.IsDead)
        {
            EnemyDead(health);
        }
        else
        {
            AlertText.text = "Hit!";
            StartCoroutine(ClearAlertCoroutine(0.5f));
        }
        return health.IsDead;
    }
}
