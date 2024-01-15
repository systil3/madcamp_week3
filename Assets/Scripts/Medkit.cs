using UnityEngine;

public class Medkit : MonoBehaviour
{   
    public GameManager gameManager;
    public float heal = 15; // 힐량
    public float rotationSpeed = 120; // 회전 속도

    void Update()
    {
        // 오브젝트를 회전시키는 코드
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            // 플레이어에게 힐 적용
            gameManager.HealToPlayer(heal);

            // Medkit 객체 제거
            Destroy(gameObject);
        }
    }
}