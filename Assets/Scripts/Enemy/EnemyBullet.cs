using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public GameManager GameManager;
    public float Damage;
    public float Strength;

    public void Awake()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.DamageToPlayer(Damage);
        }
        else if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        if (Strength <= damage)
        {
            Destroy(gameObject);
        }
        else
        {
            Strength -= damage;
        }
    }
}
