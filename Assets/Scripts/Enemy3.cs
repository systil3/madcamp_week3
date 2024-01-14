using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.AI;

public class Enemy3 : MonoBehaviour, IEnemy
{
    [field: SerializeField]
    public GameManager GameManager { get; set; }

    [field: SerializeField]
    public float Damage { get; set; } = 10.0f;

    [field: SerializeField]
    public float DetectionRange { get; set; } = 100.0f;

    [field: SerializeField]
    public Transform Player { get; set; }

    public EnemyState CurrentState { get; set; } = EnemyState.Dormant;

    public NavMeshAgent NavMeshAgent { get; set; }
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;
    private float elapsedTime = 5f+1f;
    public float timeBetweenShots = 8;
    public float sphereLifeTime = 6;
    public GameObject damageSphere;
    public float sphereSpeed = 10f;
    public float sphereScaleRate = 20f;
    private float sphereScale;
    private Transform player;
    private Transform muzzleTransform;
    //파티클 관련
    public ParticleSystem particleSystemPrefab;
    public int numberOfParticles = 100;

    private ParticleSystem particleSystemInstance;
    private Transform particleTransform;
    GameObject projectile;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        muzzleTransform = transform.Find("Muzzle");
        elapsedTime = timeBetweenShots;
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.isStopped = true;

        // 파티클 시스템 프리팹을 인스턴스화
        particleSystemInstance = Instantiate(particleSystemPrefab);
        particleTransform = particleSystemInstance.transform;
    }

    void FixedUpdate()
    {

        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        switch (CurrentState)
        {
            case EnemyState.Dormant:
                if (distanceToPlayer < DetectionRange)
                {
                    CurrentState = EnemyState.Combat;
                    NavMeshAgent.isStopped = false;
                    Debug.Log("Combat mode activated!");
                }
                break;
            case EnemyState.Combat:

                // 적이 일정 시간마다 발사
                elapsedTime += Time.deltaTime;
                if (elapsedTime >= timeBetweenShots)
                {   
                    ShootProjectile();
                    elapsedTime = 0f;
                }

                if (distanceToPlayer > DetectionRange)
                {
                    CurrentState = EnemyState.Dormant;
                    NavMeshAgent.isStopped = true;
                    Debug.Log("Returning to dormant state.");
                }
                break;
            case EnemyState.Dead:
                NavMeshAgent.isStopped = true;
                break;
        }

        if (!NavMeshAgent.isStopped)
        {
            NavMeshAgent.SetDestination(Player.position);
        }
    }

    void ShootProjectile()
    {
        // 플레이어 방향으로 구체 발사
        Vector3 direction = (player.position - muzzleTransform.position).normalized;
        GameObject projectile = Instantiate(damageSphere, muzzleTransform.position, Quaternion.identity);
        // 발사된 구체의 크기를 점점 키우기
        StartCoroutine(ScaleProjectileOverTime(direction, projectile.transform));
    }
    IEnumerator ScaleProjectileOverTime(Vector3 direction, Transform projectileTransform)
    {
        float sphereElapsedTime = 0f;
        while (sphereElapsedTime < sphereLifeTime)
        {
            //속도 적용
            projectileTransform.Translate(direction * sphereSpeed * Time.deltaTime, Space.World);

            //스케일 적용
            sphereScale = Mathf.Lerp(1f, 1f + sphereScaleRate, sphereElapsedTime / sphereLifeTime);
            Transform sphereTransformSurface = projectileTransform.Find("Surface");
            sphereTransformSurface.localScale = new Vector3(sphereScale, sphereScale, sphereScale);

            //파티클 스케일 일정하게 유지
            Transform sphereTransformParticle = projectileTransform.Find("Electricity");
            ParticleSystem sphereParticleSystem = sphereTransformParticle.GetComponent<ParticleSystem>();
            var mainModule = sphereParticleSystem.main;
            mainModule.startSize = 0.1f;
            mainModule.maxParticles += (int)(sphereScale * 20f);
            var startSpeed = mainModule.startSpeed;
            startSpeed.constant += sphereScale * 0.07f;
            sphereElapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(projectileTransform.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        print(other);
        if(other.tag == "Bullet") {
            TakeDamage(10);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isDead)
        {
            currentHealth -= damage;
            print("health : " + currentHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }
    public void Die()
    {
        CurrentState = EnemyState.Dead;

        // 여기에는 사망 시 수행할 동작을 추가
        Destroy(gameObject, 3f);
    }

    void OnParticleTrigger()
    {
        GameManager.DamageToPlayer(Damage);
    }
}
