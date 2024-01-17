using UnityEngine;

public class MedkitLarge : MonoBehaviour
{
    public GameManager GameManager;
    public float Heal = 30.0f; // 힐량
    public float RotationSpeed = 80.0f; // 회전 속도

    void Update()
    {
        // 오브젝트를 회전시키는 코드
        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.PlayerHealth.CurrentHealth < GameManager.PlayerHealth.MaxHealth)
        {
            GameManager.HealToPlayer(Heal);
            Destroy(gameObject);
        }
    }
}