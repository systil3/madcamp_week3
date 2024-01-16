using System.Collections;
using Pathfinding;
using UnityEngine;

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
    public float MaxHeight = 20.0f;
    public Rigidbody Player;

    protected EnemyState currentState;
    protected AIPath aiPath;
    protected CharacterController character;

    bool isTrackingByDamage = false;

    public virtual void Awake()
    {
        Health.CurrentHealth = Health.MaxHealth;
        currentState = EnemyState.Dormant;
        aiPath = GetComponent<AIPath>();
        aiPath.isStopped = true;
        character = GetComponent<CharacterController>();
        StartCoroutine(Roaming());
    }

    public virtual void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        float angleToPlayer = Vector3.Angle(Player.position - transform.position, transform.forward);

        switch (currentState)
        {
            case EnemyState.Dormant:
                OnDormant();

                if (isTrackingByDamage || (distanceToPlayer < DetectionRange && angleToPlayer < DetectionAngle))
                {
                    currentState = EnemyState.Combat;
                    aiPath.isStopped = false;
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
                //Vector3 newDirection = Vector3.RotateTowards(transform.forward, Player.position - transform.position, aiPath.rotationSpeed * Time.deltaTime, 0.0f);
                //transform.rotation = Quaternion.LookRotation(newDirection);

                OnCombat(distanceToPlayer);

                if (!isTrackingByDamage && (distanceToPlayer > DetectionRange || angleToPlayer > DetectionAngle))
                {
                    currentState = EnemyState.Dormant;
                    aiPath.isStopped = true;
                    StartCoroutine(Roaming());
                    Debug.Log("Returning to dormant state.");
                }
                break;
            case EnemyState.Dead:
                aiPath.isStopped = true;
                break;
        }
    }

    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 0, MaxHeight), transform.position.z);
    }

    IEnumerator Roaming()
    {
        while (currentState == EnemyState.Dormant)
        {
            Vector3 randomDir = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));

            float elapsed = 0.0f;
            while (elapsed < 2.0f)
            {
                character.Move(randomDir * aiPath.maxSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(randomDir), aiPath.maxSpeed * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    public abstract void OnCombat(float distanceToPlayer);

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