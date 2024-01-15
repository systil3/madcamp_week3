using UnityEngine;

public class Medkit : MonoBehaviour
{
    public GameManager GameManager;
    public float Heal = 15.0f; // 힐량
    public float RotationSpeed = 120.0f; // 회전 속도

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