using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IEnemy
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

    ParticleSystem ps;

    void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.isStopped = true;
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

    public void TakeDamage(float damage)
    {
        GameManager.DamageToEnemy(damage);
        if (GameManager.IsEnemyDead) Die();
    }

    public void Die()
    {
        CurrentState = EnemyState.Dead;

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
