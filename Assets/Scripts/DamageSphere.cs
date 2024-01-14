using UnityEngine;

public class DamageSphere : MonoBehaviour
{
    public GameManager GameManager;
    public float Damage = 20.0f;
    bool isPlayerTriggering = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerTriggering = true;
            GameManager.DamageToPlayer(Damage);
            other.GetComponent<Player>().SlowDownSpeed();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().ReturnPlayerSpeed();
            isPlayerTriggering = false;
        }
    }

    void OnDestroy()
    {
        if (isPlayerTriggering)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().ReturnPlayerSpeed();
            isPlayerTriggering = false;
        }
    }
}
