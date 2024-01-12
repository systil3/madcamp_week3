using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour
{
    // 발사 소리
    public AudioClip GunshotSound;

    // 상태창
    public int Health = 100;

    // 이동 관련
    public float Speed = 20.0f;
    public float RotationSpeed = 3.0f;
    public float JumpForce = 15.0f;

    // 총알 관련
    public GameObject BulletObject;
    public float BulletForce = 20.0f;
    public float MaxShootDelay = 0.15f;

    Rigidbody body;

    bool isAccelerating = false;
    bool isJumping = false;

    float shootDelay = 0;

    float xRotate, yRotate, xRotateMove, yRotateMove;

    AudioSource AudioSource; // AudioSource 컴포넌트
    
    void Awake()
    {   
        //마우스 포인터 잠금 및 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        body = GetComponent<Rigidbody>();
        JumpForce = 15.0f;
        isJumping = false;

        // AudioSource 컴포넌트 초기화
        AudioSource = GetComponent<AudioSource>();
        #if UNITY_EDITOR
            GunshotSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Custom_Yung/PistolShot1.wav");
        #endif
    }

    void Update()
    {
        Shoot();
    }

    void FixedUpdate()
    {
        //print("speed: " + body.velocity.magnitude);
        Move();
        Jump();
    }

    void LateUpdate()
    {
        RotateWithMouse();
    }

    void Move()
    {
        if (isAccelerating) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v * 0.75f);
        Vector3 transformedDir = TransformDirectionRelativeToPlayer(dir);

        if (!(h == 0 && v == 0))
        {     
            body.velocity = new Vector3(transformedDir.x * Speed, body.velocity.y, transformedDir.z * Speed);
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
            body.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
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
        if (Input.GetButton("Fire1") && shootDelay > MaxShootDelay)
        {
            Vector3 forward = TransformDirectionRelativeToPlayer(Vector3.forward);
            GameObject bullet = Instantiate(BulletObject, transform.position + forward * 0.7f, transform.rotation);
            Rigidbody rigid = bullet.GetComponent<Rigidbody>();
            rigid.AddForce(forward * BulletForce, ForceMode.Impulse);

            // 발사 소리 재생
            if (GunshotSound != null)
            {
                AudioSource.PlayOneShot(GunshotSound);
            }
            shootDelay = 0;
        }

        shootDelay += Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Floor"))
        {
            isJumping = false;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Accel"))
        {
            isAccelerating = true;
        } 
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Accel"))
        {
            isAccelerating = false;
        }
    }
}

