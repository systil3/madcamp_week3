using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpForce = 20.0f;
    public float horizontalForce = 5.0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // 플레이어의 현재 이동 방향을 고려한 수평 힘 계산
                Vector3 horizontalDirection = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);

                // 힘을 종합하여 적용
                Vector3 totalForce = horizontalDirection.normalized * horizontalForce + Vector3.up * jumpForce;

                // Rigidbody의 질량에 따라 힘을 적용
                playerRigidbody.AddForce(totalForce, ForceMode.Impulse);
            }
        }
    }
}
