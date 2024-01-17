using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public GameManager gameManager;
    public float damage;
    public float strength;

    public void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameManager.DamageToPlayer(damage);
        }

        else if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Floor"))
        {
            Destroy();
        }
    }

    public void TakeDamage(float damage)
    {
        if (strength <= damage)
        {
            Destroy();
        }
        else
        {
            strength = -damage;
        }
    }

    public void Destroy()
    {
        print("destroy!!!");
        Destroy(gameObject);
    }
}
