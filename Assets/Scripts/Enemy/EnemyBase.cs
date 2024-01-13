using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Dormant,
    Combat,
    Dead,
}

public abstract class EnemyBase : MonoBehaviour
{
    public GameManager GameManager;

    public Health Health;
    public float Damage;
    public float DetectionRange;
    public Transform Player;

    protected EnemyState currentState;
    protected NavMeshAgent navMeshAgent;
    protected Rigidbody body;

    public virtual void Awake()
    {
        Health.CurrentHealth = Health.MaxHealth;
        currentState = EnemyState.Dormant;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = true;
        body = GetComponent<Rigidbody>();
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
                    StartCoroutine(Roaming());
                    Debug.Log("Returning to dormant state.");
                }
                break;
            case EnemyState.Dead:
                body.velocity = Vector3.zero;
                navMeshAgent.isStopped = true;
                break;
        }

        if (!navMeshAgent.isStopped)
        {
            OnCombat();
            navMeshAgent.SetDestination(Player.position);
        }
    }

    IEnumerator Roaming()
    {
        while (currentState == EnemyState.Dormant)
        {
            Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            body.velocity = randomDir * navMeshAgent.speed;
            yield return new WaitForSeconds(5.0f);
        }
    }

    public abstract void OnCombat();

    public void TakeDamage(float damage)
    {
        bool isDead = GameManager.DamageToEnemy(Health, damage);
        if (isDead)
        {
            currentState = EnemyState.Dead;
            Die();
        }
    }

    public abstract void Die();
}