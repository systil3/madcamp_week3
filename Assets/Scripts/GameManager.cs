using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public ScriptableRendererFeature RendererFeature;
    public Canvas LiveCanvas;
    public Canvas StatisticsCanvas;
    public TextMeshProUGUI AlertText;
    public Health PlayerHealth;
    public Slider EnemyHealthSlider;
    public TextMeshProUGUI EnemyHealthText;
    public TextMeshProUGUI StatisticsText;
    public int NumHit = 0, NumShot = 0;

    List<EnemyBase> enemies;
    int enemyHealthIndex = 0;
    float damage = 0.0f;

    void Awake()
    {
        // Enemy 태그를 가진 모든 객체들을 찾아서 리스트에 추가
        enemies = GameObject.FindGameObjectsWithTag("Enemy").Select(e => e.GetComponent<EnemyBase>()).ToList();

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
            for (int i = 0; i < enemies.Count; i++)
            {
                Health health = enemies[i].Health;

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

            enemies[enemyHealthIndex].Health.Update();
        }
    }

    public void UpdateEnemyHealthIndex(Transform player)
    {
        int minIndex = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsDestroyed()) continue;
            float distance = Vector3.Distance(enemies[i].GetComponent<Transform>().position, player.position);
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
        enemies[index].Health.SetMaxHealth(maxHealth);
    }

    void PlayerDead()
    {
        StartCoroutine(PlayerDeadCoroutine());
    }

    void EnemyDead()
    {
        if (enemies.Count(e => !e.Health.IsDead) == 0)
        {
            StartCoroutine(ClearCoroutine());
        }
        else
        {
            AlertText.text = "You killed!";
            StartCoroutine(ClearAlertCoroutine(2.0f));
        }
    }

    IEnumerator ClearAlertCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        AlertText.text = "";
    }

    IEnumerator ClearCoroutine()
    {
        AlertText.text = "You cleared!";
        yield return new WaitForSeconds(4.0f);
        ShowStatistics();
    }

    IEnumerator PlayerDeadCoroutine()
    {
        for (int i = 3; i > 0; i--)
        {
            AlertText.text = $"You died!\n{i} Seconds";
            yield return new WaitForSeconds(1.0f);
        }

        ShowStatistics();
    }

    IEnumerator DamageScreenCoroutine()
    {
        RendererFeature.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        RendererFeature.SetActive(false);
    }

    void ShowStatistics()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GetComponent<AudioSource>().Stop();
        int kill = enemies.Count(e => e.Health.IsDead);
        LiveCanvas.gameObject.SetActive(false);
        StatisticsText.text = $"Health: {PlayerHealth.CurrentHealth} / {PlayerHealth.MaxHealth}\n\nKill: {kill}\n\nDamage: {Math.Round(damage, 1)}\n\nHit Rate: {NumHit} / {NumShot} ({Math.Round(NumHit / (decimal)NumShot * 100, 1)}%)";
        StatisticsCanvas.gameObject.SetActive(true);
    }

    public void Restart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AlertText.text = "";
        SetPlayerHealth(PlayerHealth.MaxHealth);

        for (int i = 0; i < enemies.Count; i++)
        {
            SetEnemyHealth(i, enemies[i].Health.MaxHealth);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GetComponent<AudioSource>().Play();
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
        this.damage += damage;

        if (health.IsDead)
        {
            EnemyDead();
        }
        else
        {
            AlertText.text = "Hit!";
            StartCoroutine(ClearAlertCoroutine(0.5f));
        }
        return health.IsDead;
    }
}
