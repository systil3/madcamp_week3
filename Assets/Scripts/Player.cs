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

    //반동 제어
    private Vector3 originalPosition;
    private bool isRecoiling = false;
    public float recoilForce = 20.0f; // 반동 힘
    public float recoilDuration = 0.5f;

    //피스톨 객체 찾기
    public Transform pistolTransform;

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

        if (pistolTransform == null)
        {
            // 만약 pistolTransform이 직접 Inspector에서 설정되지 않았다면 자식 오브젝트에서 찾아서 할당
            pistolTransform = transform.Find("Pistol");
        }
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
        } StartRecoil();
    }
    void StartRecoil()
    {
        // 반동 힘을 impulse로 적용
        body.AddForce(-transform.forward * recoilForce, ForceMode.Impulse);

        isRecoiling = true; // 반동이 시작되었음을 표시

        StartCoroutine(RecoilCoroutine()); //velocity로 적용
        //Recoil(); //Impulse로 적용
    }

    IEnumerator RecoilCoroutine()
    {
        float elapsedTime = 0f;

        // 플레이어에 대해 반동을 가함
        body.velocity -= transform.forward * recoilForce;

        /*while (elapsedTime < recoilDuration)
        {
            // 이징 적용
            float t = elapsedTime / recoilDuration;
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            // 반동 힘 계산
            Vector3 recoilOffset = -transform.forward * recoilForce * Time.deltaTime * easedT;

            // 플레이어에게 반동 힘 적용
            body.velocity += recoilOffset;
            elapsedTime += Time.deltaTime;
            yield return null;
        }*/ 

        yield return new WaitForSeconds(recoilDuration);
        // 반동 종료
        isRecoiling = false;
    }

    void Recoil()
    {
        // 반동 중일 때 필요한 작업을 수행
        // (예: 화면 흔들림 효과 등)
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

