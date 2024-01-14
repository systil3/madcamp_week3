using System.Collections;
using UnityEngine;

public class Accelerator : MonoBehaviour
{
    // 가속도를 가할 시간과 가속도 크기
    public float AccelerateDuration = 0.5f;
    public float MaintainDuration = 1f;
    public float AccelerationSpeed = 35.0f;

    // 충돌이 발생했을 때 호출되는 함수
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            StartCoroutine(ApplyAcceleration(player, playerRigidbody));
        }
    }

    // 가속을 적용하는 코루틴 함수
    IEnumerator ApplyAcceleration(Player player, Rigidbody rigidbody)
    {
        player.IsFreeze = true;
        // 현재 속도를 서서히 가속시킴
        float elapsedTime = 0.0f;

        Vector3 initialVelocity = rigidbody.velocity;
        Vector3 acceleratedVelocity = -transform.forward * AccelerationSpeed;

        while (elapsedTime < AccelerateDuration)
        {
            float t = Mathf.SmoothStep(0.0f, 1.0f, elapsedTime / AccelerateDuration);
            rigidbody.velocity = Vector3.Lerp(initialVelocity, acceleratedVelocity, t);
            elapsedTime += Time.deltaTime;
        }

        rigidbody.velocity = acceleratedVelocity;
        yield return new WaitForSeconds(MaintainDuration);
        // 일정 속도로 유지
        player.IsFreeze = false;
    }
}
