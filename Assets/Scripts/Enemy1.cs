using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float MaxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;
    private EnemyState currentState;

    public float DetectionRange = 100f;
    public float CombatRange = 30f;
    public float MoveSpeed = 2f; // 적의 이동 속도
    public Transform Player;

    private Rigidbody rb;
    private ParticleSystem ps;

    private enum EnemyState
    {
        Dormant,
        Combat,
        Dead
    }

    void Awake()
    {
        currentHealth = MaxHealth;
        currentState = EnemyState.Dormant;
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();

        // 파티클 좌표를 월드 좌표 공간으로 설정
        SetSimulationSpaceToWorld();
    }

    void SetSimulationSpaceToWorld()
    {
        var mainModule = ps.main;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        switch (currentState)
        {
            case EnemyState.Dormant:
                if (distanceToPlayer < DetectionRange)
                {
                    currentState = EnemyState.Combat;
                    Debug.Log("Combat mode activated!");
                }
                break;

            case EnemyState.Combat:
                if (distanceToPlayer > DetectionRange)
                {
                    currentState = EnemyState.Dormant;
                    Debug.Log("Returning to dormant state.");
                }
                else
                {
                    //print(distanceToPlayer + " ||  " + combatRange);
                    Vector3 direction = Player.position - transform.position;
                    // 플레이어를 향해 회전
                    /*Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);*/

                    // 플레이어 쪽으로 Rigidbody의 velocity를 조절하여 이동
                    if (distanceToPlayer > CombatRange)
                    {
                        rb.velocity = direction * MoveSpeed;
                    }
                    else
                    {
                        rb.velocity *= 0.5f;
                    }

                }
                break;

            case EnemyState.Dead:
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isDead)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    void Die()
    {
        currentState = EnemyState.Dead;
        isDead = true;

        // 여기에는 사망 시 수행할 동작을 추가
        Destroy(gameObject, 3f);
    }

    void OnParticleTrigger()
    {
        Debug.Log("Triggered");
    }
}
