using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerator : MonoBehaviour
{
    // 가속도를 가할 시간과 가속도 크기

    public float AccelerateDuration = 0.5f;
    public float MaintainDuration = 1f;
    public float AccelerationSpeed = 30.0f;

    // 충돌이 발생했을 때 호출되는 함수
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            StartCoroutine(ApplyAcceleration(playerRigidbody));
        }
    }

    // 가속을 적용하는 코루틴 함수
    IEnumerator ApplyAcceleration(Rigidbody playerRigidbody)
    {
        // 현재 속도를 서서히 가속시킴
        float startTime = Time.time;
        float elapsedTime = 0.0f;
        Vector3 initialVelocity = playerRigidbody.velocity;
        Vector3 acceleratedVelocity = -transform.forward * AccelerationSpeed;

        while (elapsedTime < AccelerateDuration)
        {
            float t = Mathf.SmoothStep(0.0f, 1.0f, elapsedTime / AccelerateDuration);
            playerRigidbody.velocity = Vector3.Lerp(initialVelocity, acceleratedVelocity, t);
            elapsedTime += Time.deltaTime;
        }

        playerRigidbody.velocity = acceleratedVelocity;
        yield return new WaitForSeconds(MaintainDuration);
        // 일정 속도로 유지
        playerRigidbody.GetComponent<Player>().IsAccelerating = false;
    }
}
