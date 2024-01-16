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
    public float DamageDuration;
    public float DetectionRange;
    public float DetectionAngle;
    public Rigidbody Player;

    protected float distanceToPlayer;
    protected float angleToPlayer;

    protected EnemyState currentState;
    protected NavMeshAgent navMeshAgent;
    protected Rigidbody body;

    bool isTrackingByDamage = false;

    public virtual void Awake()
    {
        Health.CurrentHealth = Health.MaxHealth;
        currentState = EnemyState.Dormant;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.isStopped = true;
        navMeshAgent.updateRotation = false;
        body = GetComponent<Rigidbody>();
        StartCoroutine(Roaming());
    }

    public virtual void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        angleToPlayer = Vector3.Angle(Player.position - transform.position, transform.forward);

        switch (currentState)
        {
            case EnemyState.Dormant:
                OnDormant();

                if (isTrackingByDamage || (distanceToPlayer < DetectionRange && angleToPlayer < DetectionAngle))
                {
                    currentState = EnemyState.Combat;
                    navMeshAgent.isStopped = false;
                    Debug.Log("Combat mode activated!");
                }
                break;
            case EnemyState.Combat:
                if (distanceToPlayer < DetectionRange && angleToPlayer < DetectionAngle)
                {
                    isTrackingByDamage = false;
                }
                /*
                Vector2 forward = new Vector2(transform.position.z, transform.position.x);
                Vector2 steeringTarget = new Vector2(navMeshAgent.steeringTarget.z, navMeshAgent.steeringTarget.x);
                Vector2 dir = steeringTarget - forward;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.eulerAngles = Vector3.up * angle;
                */
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, Player.position - transform.position, navMeshAgent.angularSpeed * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDirection);

                OnCombat();

                if (!isTrackingByDamage && (distanceToPlayer > DetectionRange || angleToPlayer > DetectionAngle))
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
            navMeshAgent.SetDestination(Player.position);
        }
    }

    IEnumerator Roaming()
    {
        while (currentState == EnemyState.Dormant)
        {
            Vector3 randomDir = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
            body.velocity = randomDir * navMeshAgent.speed / 2;
            transform.eulerAngles = new Vector3(Random.Range(0.0f, 360.0f), 0.0f, Random.Range(0.0f, 360.0f));
            yield return new WaitForSeconds(2.0f);
        }
    }

    public abstract void OnCombat();

    public abstract void OnDormant();

    public void TakeDamage(float damage)
    {
        Debug.Log($"Damage: {damage}");
        bool isDead = GameManager.DamageToEnemy(Health, damage);
        if (isDead)
        {
            currentState = EnemyState.Dead;
            Die();
        }
        else
        {
            isTrackingByDamage = true;
        }
    }

    public abstract void Die();
}