using UnityEngine;

public class Medkit : MonoBehaviour
{
    public float rotationSpeed = 120; // 회전 속도

    void Update()
    {
        // 오브젝트를 회전시키는 코드
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}