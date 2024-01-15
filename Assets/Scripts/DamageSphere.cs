using UnityEngine;

public class DamageSphere : MonoBehaviour
{
    public GameManager GameManager;
    public float Damage;
    bool isPlayerTriggering = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerTriggering = true;
            GameManager.DamageToPlayer(Damage);
            other.GetComponent<Player>().SlowDownSpeed(0.5f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().ReturnSpeed();
            isPlayerTriggering = false;
        }
    }

    void OnDestroy()
    {
        if (isPlayerTriggering)
        {
            GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>()?.ReturnSpeed();
            isPlayerTriggering = false;
        }
    }
}
