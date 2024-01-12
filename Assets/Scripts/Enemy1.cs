using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;
    private EnemyState currentState;

    public float detectionRange = 100f;
    public float combatRange = 30f;
    public float moveSpeed = 2f; // 적의 이동 속도
    public Transform player;

    private Rigidbody rb;
    
    private enum EnemyState
    {
        Dormant,
        Combat,
        Dead
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentState = EnemyState.Dormant;
        rb = GetComponent<Rigidbody>();
        combatRange = 30f;

        // 플레이어를 이름으로 찾아서 할당
        player = GameObject.Find("Player").transform;

        if (player == null)
        {
            Debug.LogError("Player not found. Make sure the Player object has the correct name.");
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Dormant:
                if (distanceToPlayer < detectionRange)
                {
                    currentState = EnemyState.Combat;
                    Debug.Log("Combat mode activated!");
                }
                break;

            case EnemyState.Combat:
                if (distanceToPlayer > detectionRange)
                {
                    currentState = EnemyState.Dormant;
                    Debug.Log("Returning to dormant state.");
                }
                else
                {
                    //print(distanceToPlayer + " ||  " + combatRange);
                    Vector3 direction = player.position - transform.position;
                    // 플레이어를 향해 회전
                    /*Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);*/

                    // 플레이어 쪽으로 Rigidbody의 velocity를 조절하여 이동
                    if(distanceToPlayer > combatRange) {
                        rb.velocity = direction * moveSpeed;
                    } else {
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
}
