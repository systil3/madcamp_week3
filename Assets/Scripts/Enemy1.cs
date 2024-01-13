using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    enum EnemyState
    {
        Dormant,
        Combat,
        Dead
    }

    public GameManager GameManager;

    public float Damage = 10.0f;
    public float DetectionRange = 100f;
    //public float CombatRange = 30f;
    //public float MoveSpeed = 2f;
    public Transform Player;

    EnemyState currentState;

    //Rigidbody rb;
    ParticleSystem ps;
    NavMeshAgent navMeshAgent;

    void Awake()
    {
        currentState = EnemyState.Dormant;
        //rb = GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = true;

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
                    navMeshAgent.isStopped = false;
                    Debug.Log("Combat mode activated!");
                }
                break;

            case EnemyState.Combat:
                if (distanceToPlayer > DetectionRange)
                {
                    currentState = EnemyState.Dormant;
                    navMeshAgent.isStopped = true;
                    Debug.Log("Returning to dormant state.");
                }/*
                else
                {
                    //print(distanceToPlayer + " ||  " + CombatRange);
                    Vector3 direction = Player.position - transform.position;
                    // 플레이어를 향해 회전
                    //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

                    // 플레이어 쪽으로 Rigidbody의 velocity를 조절하여 이동
                    if (distanceToPlayer > CombatRange)
                    {
                        rb.velocity = direction * MoveSpeed;
                    }
                    else
                    {
                        rb.velocity *= 0.5f;
                    }

                }*/
                break;

            case EnemyState.Dead:
                navMeshAgent.isStopped = true;
                break;
        }

        if (!navMeshAgent.isStopped)
        {
            navMeshAgent.SetDestination(Player.position);
        }
    }

    public void TakeDamage(float damage)
    {
        GameManager.DamageToEnemy(damage);
        if (GameManager.IsEnemyDead) Die();
    }

    void Die()
    {
        currentState = EnemyState.Dead;

        // 여기에는 사망 시 수행할 동작을 추가
        ps.Stop();
        ps.Clear();
        Destroy(gameObject, 3f);
    }

    void OnParticleTrigger()
    {
        GameManager.DamageToPlayer(Damage);
    }
}
