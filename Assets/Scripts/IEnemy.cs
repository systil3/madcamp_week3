using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Dormant,
    Combat,
    Dead
}

interface IEnemy
{
    GameManager GameManager { get; set; }

    float Damage { get; set; }

    float DetectionRange { get; set; }

    Transform Player { get; set; }

    EnemyState CurrentState { get; set; }

    NavMeshAgent NavMeshAgent { get; set; }

    void TakeDamage(float damage);

    void Die();
}