using UnityEngine;
using UnityEditor;
using System.Collections;

public class Player : MonoBehaviour
{
    public GameManager GameManager;

    //피스톨 객체 찾기
    public Transform pistolTransform;

    // 발사 소리
    public AudioClip GunshotSound;

    public LayerMask GroundLayer;

    // 이동 관련
    public float Speed = 20.0f;
    public float RotationSpeed = 3.0f;
    public float JumpForce = 15.0f;
    public bool IsAccelerating = false;

    // 총알 관련
    public GameObject BulletObject;
    public float BulletForce = 50.0f;
    public float MaxShootDelay = 0.05f;

    //반동 제어
    public float RecoilForce = 15.0f; // 반동 힘
    public float RecoilDuration = 0.5f;
    Vector3 originalPosition;
    Vector3 wallNormal;
    bool isRecoiling = false;

    Rigidbody body;
    AudioSource audioSource; // AudioSource 컴포넌트

    bool isJumping = false;
    bool isClimbing = false;
    float shootDelay = 0;
    float xRotate, yRotate, xRotateMove, yRotateMove;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

#if UNITY_EDITOR
        GunshotSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Custom_Yung/PistolShot1.wav");
#endif

        if (pistolTransform == null)
        {
            // 만약 pistolTransform이 직접 Inspector에서 설정되지 않았다면 자식 오브젝트에서 찾아서 할당
            pistolTransform = transform.Find("Pistol");
        }
    }

    void Update()
    {
        if (GameManager.PlayerHealth.IsDead) return;
        GameManager.UpdateEnemyHealthIndex(transform);
        isJumping = !Physics.Raycast(transform.position, Vector3.down, 2.0f, GroundLayer);
        Shoot();
    }

    void FixedUpdate()
    {
        if (GameManager.PlayerHealth.IsDead) return;
        Move();
        Jump();
    }

    void LateUpdate()
    {
        if (GameManager.PlayerHealth.IsDead) return;
        RotateWithMouse();
    }

    void Move()
    {
        if (IsAccelerating) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (!(h == 0 && v == 0))
        {
            Vector3 dir = new Vector3(h, 0, v * 0.75f);
            Vector3 transformedDir = TransformDirectionRelativeToPlayer(dir);
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
        if (!isClimbing)
        { //벽 타는 중이 아니면 일반 점프
            if (Input.GetKey(KeyCode.Space) && !isJumping)
            {
                body.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            }
        }
        else
        {  // 벽 타는 중이면 벽 점프
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 jumpDirection = Vector3.Reflect(body.velocity, wallNormal).normalized;

                // 기존 속도를 초기화하고 반대 방향으로 힘을 가하여 점프
                body.velocity = new Vector3(0f, 0f, 0f);
                body.AddForce(jumpDirection * JumpForce, ForceMode.Impulse);
                Debug.Log("walljump, wall normal :" + wallNormal + "jump speed: " + body.velocity);
                StartCoroutine(Sleep());
            }
        }
    }

    IEnumerator Sleep()
    {
        yield return new WaitForSeconds(0.3f);
    }

    void RotateWithMouse()
    {
        xRotateMove = -Input.GetAxis("Mouse Y") * RotationSpeed;
        yRotateMove = Input.GetAxis("Mouse X") * RotationSpeed;

        yRotate = yRotate + yRotateMove;
        //xRotate = transform.eulerAngles.x + xRotateMove; 
        xRotate = xRotate + xRotateMove;

        xRotate = Mathf.Clamp(xRotate, -90, 90); // 위, 아래 고정
        transform.eulerAngles = new Vector3(xRotate, yRotate, 0);
    }

    void Shoot()
    {
        // 총 발사 로직을 여기에 추가 (필요에 따라 총구 이펙트, 총알 등도 추가 가능)
        if (Input.GetButtonDown("Fire1") && shootDelay > MaxShootDelay)
        {
            Vector3 forward = TransformDirectionRelativeToPlayer(Vector3.forward);
            GameObject bullet = Instantiate(BulletObject, transform.position + (forward + Vector3.up) * 0.8f, transform.rotation);
            Rigidbody rigid = bullet.GetComponent<Rigidbody>();
            rigid.AddForce(forward * BulletForce, ForceMode.Impulse);

            // 발사 소리 재생
            if (GunshotSound != null)
            {
                audioSource.PlayOneShot(GunshotSound);
            }

            StartRecoil();
            shootDelay = 0;
        }

        shootDelay += Time.deltaTime;
    }

    void StartRecoil()
    {
        if (isRecoiling) return;

        isRecoiling = true; // 반동이 시작되었음을 표시
        // 반동 힘을 impulse로 적용
        body.AddForce(-transform.forward.x * RecoilForce, Mathf.Min(-transform.forward.y * RecoilForce, 1.0f), -transform.forward.z * RecoilForce, ForceMode.Impulse);

        StartCoroutine(RecoilCoroutine()); //velocity로 적용
        //Recoil(); //Impulse로 적용
    }

    IEnumerator RecoilCoroutine()
    {
        //float elapsedTime = 0f;

        // 플레이어에 대해 반동을 가함
        //body.velocity -= transform.forward * recoilForce;

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

        yield return new WaitForSeconds(RecoilDuration);
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
        if (collision.gameObject.CompareTag("Wall"))
        {
            isClimbing = false; //벽을 타고 있는가?(벽의 측면에 충돌 중인가?)
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            isClimbing = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall")) //벽 점프
        {
            // 벽의 측면에 충돌한 경우인지 검사
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) < 0.1f)
                {
                    //Debug.Log("side wall collision, wall normal" + contact.normal);
                    isClimbing = true;
                    // 충돌 지점의 노말 벡터가 거의 수직이면(측면 충돌), 월 점프 가능
                    wallNormal = contact.normal;
                    break; // 한 번만 처리하도록 종료
                }
            }
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Accel"))
        {
            IsAccelerating = true;
        }
    }
}

