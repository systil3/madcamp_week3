using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour
{
    //상태창
    public int health = 100;

    //이동 관련
    public float Speed = 2.0f;
    public float RotationSpeed = 20000.0f;
    public float jumpForce = 15.0f;

    Rigidbody body;

    float h, v;

    bool isJumping = false;
    private float xRotate, yRotate, xRotateMove, yRotateMove;

    public AudioClip gunshotSound;  // 발사 소리
    private AudioSource audioSource; // AudioSource 컴포넌트
    void Start()
    {   
        //마우스 포인터 잠금 및 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        body = GetComponent<Rigidbody>();
        jumpForce = 15.0f;
        isJumping = false;

        // AudioSource 컴포넌트 초기화
        audioSource = GetComponent<AudioSource>();
        string path = "Assets/Custom_Yung/PistolShot1.wav";
        #if UNITY_EDITOR
                gunshotSound = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
        #endif
    }

    void FixedUpdate()
    {
        // 마우스 클릭 시 발사 소리 재생 + 실제 총알 발사(Todo)
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
        //print("speed: " + body.velocity.magnitude);
        Move();
        Jump();
        RotateWithMouse();
    }

    void Move()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v*0.75f);
        Vector3 transformed_dir = TransformDirectionRelativeToPlayer(dir);

        if (!(h == 0 && v == 0))
        {     
            //print(GameManager.instance.getIfAccelerating() );
            
            //transform.position += transformed_dir * Speed * Time.deltaTime;
            //가속 중이 아니라면 조작 가능
            if(true) {
                Vector3 newVelocity = new Vector3(
                    transformed_dir.x * Speed, body.velocity.y, transformed_dir.z * Speed);
                body.velocity = newVelocity;
            }
        }
    }
    Vector3 TransformDirectionRelativeToPlayer(Vector3 direction)
    {
        // 플레이어의 현재 회전을 가져옴
        Quaternion playerRotation = transform.rotation;

        // 플레이어의 전방(Forward) 방향을 기준으로 좌표 변환
        Vector3 transformedDirection = playerRotation * direction;

        return transformedDirection;
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && !isJumping)
        {
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }
    }

    void RotateWithMouse()
    {
        xRotateMove = -Input.GetAxis("Mouse Y") * RotationSpeed;
        yRotateMove = Input.GetAxis("Mouse X") * RotationSpeed;

        yRotate = yRotate + yRotateMove;
        //xRotate = transform.eulerAngles.x + xRotateMove; 
        xRotate = xRotate + xRotateMove;
        
        xRotate = Mathf.Clamp(xRotate, -90, 90); // 위, 아래 고정
        
        //transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
        //float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * RotationSpeed;
        //transform.RotateAround(Target.transform.position, Vector3.up, yRotateMove);
        
        transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
    }

    void Shoot()
    {
        // 총 발사 로직을 여기에 추가 (필요에 따라 총구 이펙트, 총알 등도 추가 가능)


        // 발사 소리 재생
        if (gunshotSound != null)
        {
            audioSource.PlayOneShot(gunshotSound);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJumping = false;
        }  else if (collision.gameObject.tag == "Accel")
        {
        
        } 
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Accel")
        {
            Debug.Log("Collision ended with OtherObject");
        }
    }
}

