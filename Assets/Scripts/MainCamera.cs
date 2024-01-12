using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera_Action : MonoBehaviour
{
    public GameObject Target;               // 카메라가 따라다닐 타겟

    public float offsetX = 0.0f;            // 카메라의 x좌표
    public float offsetY = 10.0f;           // 카메라의 y좌표
    public float offsetZ = -10.0f;          // 카메라의 z좌표

    public float CameraSpeed = 3.0f;       // 카메라의 속도
    public float RotationSpeed = 5000.0f;      // 마우스 회전 속도

    Vector3 TargetPos;                      // 타겟의 위치
    private float xRotate, yRotate, xRotateMove, yRotateMove;

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        // 타겟의 x, y, z 좌표에 카메라의 좌표를 더하여 카메라의 위치를 결정
        TargetPos = new Vector3(
            Target.transform.position.x + offsetX,
            Target.transform.position.y + offsetY,
            Target.transform.position.z + offsetZ
            );

        // 카메라의 움직임을 부드럽게 하는 함수(Lerp)
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * CameraSpeed);

        // 마우스 이동에 따라 카메라 회전
        xRotateMove = -Input.GetAxis("Mouse Y") * RotationSpeed / 2;
        yRotateMove = Input.GetAxis("Mouse X") * RotationSpeed;

        yRotate = transform.eulerAngles.y + yRotateMove;
        //xRotate = transform.eulerAngles.x + xRotateMove; 
        xRotate = xRotate + xRotateMove;
        
        xRotate = Mathf.Clamp(xRotate, -90, 90); // 위, 아래 고정
        
        transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
        //float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * RotationSpeed;
        //transform.RotateAround(Target.transform.position, Vector3.up, yRotateMove);
        */
    }
}