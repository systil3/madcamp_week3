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
    public float TimeToDormant = 5.0f;
    public Rigidbody Player;
    public ParticleSystem DieEffectObject;

    protected float distanceToPlayer;
    protected float angleToPlayer;

    protected EnemyState currentState;
    protected AIPath aiPath;
    protected CharacterController character;

    bool isTrackingByDamage = false;
    float elapsedToDormant = 0.0f;
    ParticleSystem dieEffect;

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
        distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        angleToPlayer = Vector3.Angle(Player.position - transform.position, transform.forward);

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

                OnCombat(distanceToPlayer);

                if (!isTrackingByDamage && (distanceToPlayer > DetectionRange || angleToPlayer > DetectionAngle))
                {
                    if (elapsedToDormant >= TimeToDormant)
                    {
                        currentState = EnemyState.Dormant;
                        aiPath.isStopped = true;
                        elapsedToDormant = 0.0f;
                        StartCoroutine(Roaming());
                        Debug.Log("Returning to dormant state.");
                    }
                    elapsedToDormant += Time.deltaTime;
                }
                else
                {
                    elapsedToDormant = 0.0f;
                }
                break;
            case EnemyState.Dead:
                aiPath.isStopped = true;
                break;
        }
    }

    public virtual void LateUpdate()
    {
        //transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 0, Player.position.y + 15.0f), transform.position.z);
    }

    public virtual void OnDestroy()
    {
        dieEffect?.Stop();
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

    public virtual void Die()
    {
        dieEffect = Instantiate(DieEffectObject, transform.position, transform.rotation);
        dieEffect.Play();

        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        Destroy(gameObject, dieEffect.main.duration);
    }
}