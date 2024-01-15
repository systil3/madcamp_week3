using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    public GameManager GameManager;
    public LayerMask GroundLayer;

    public GunType CurrentGunType = GunType.Pistol;
    public List<Gun> Guns;
    public AudioClip GunshotSound;

    // 이동 관련
    public float Speed = 20.0f;
    public float RotationSpeed = 3.0f;
    public float JumpForce = 3.0f;
    public bool IsFreeze = false;
    float initialSpeed;

    public float SpeedSlowDownRatio = 0.5f;
    public bool IsSlowedDown = false;

    // 총알 관련

    public GameObject Bullet;
    public GameObject Grenade;
    public GameObject Pellet;
    public GameObject BulletObject;
    public float BulletForce = 50.0f;

    // 반동 관련
    public float RecoilForce = 20.0f; // 반동 힘
    public float RecoilDuration = 0.5f;
    Vector3 originalPosition;
    Vector3 wallNormal;

    // 카메라 관련
    public Vector3 CameraOffset;
    public bool IsShaking = false;
    float xRotate, yRotate, xRotateMove, yRotateMove;

    bool isJumping = false;
    bool isClimbing = false;
    float shootDelay = 0;

    Rigidbody body;
    Transform arm;
    public Vector3 ArmOffset;
    AudioSource audioSource; // AudioSource 컴포넌트
    

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        CameraOffset = Camera.main.transform.localPosition;
        arm = transform.Find("Arm");
        ArmOffset = arm.localPosition;
        initialSpeed = Speed;
        

    #if UNITY_EDITOR
            GunshotSound = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Custom_Yung/PistolShot1.wav");
    #endif
    }

    void Update()
    {
        if (GameManager.PlayerHealth.IsDead) return;
        GameManager.UpdateEnemyHealthIndex(transform);
        isJumping = !Physics.Raycast(transform.position, Vector3.down, 2.0f, GroundLayer);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentGunType = GunType.Pistol;
            Debug.Log("Changed to Pistol!");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentGunType = GunType.Rapid;
            Debug.Log("Change to Rapid!");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentGunType = GunType.Grenade;
            Debug.Log("Changed to Grenade!");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CurrentGunType = GunType.Shotgun;
            Debug.Log("Changed to ShotGun!");
        }

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
        if (!IsShaking) {
            arm.localPosition = ArmOffset;
            Camera.main.transform.localPosition = CameraOffset;
        }
    }

    void Move()
    {
        if (IsFreeze) return;

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

    public void SlowDownSpeed()
    {
        if (!IsSlowedDown)
        {
            Speed *= SpeedSlowDownRatio;
            IsSlowedDown = true;
        }
    }

    public void ReturnPlayerSpeed()
    {
        if (IsSlowedDown)
        {
            Speed = initialSpeed;
            IsSlowedDown = false;
        }
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
        if ((CurrentGunType != GunType.Rapid && Input.GetButtonDown("Fire1")) || (CurrentGunType == GunType.Rapid && Input.GetButton("Fire1")))
        {
            Gun currentGun = Guns.FirstOrDefault(e => e.Type == CurrentGunType);
            RecoilForce = currentGun.RecoilForce;
            RecoilDuration = currentGun.RecoilDuration;
            
            switch (CurrentGunType)
            {
                case GunType.Pistol:
                    BulletObject = Bullet;
                    break;
                case GunType.Rapid:
                    BulletObject = Bullet;
                    break;
                case GunType.Grenade:
                    BulletObject = Grenade;
                    break;
                case GunType.Shotgun:    
                    BulletObject = Pellet;
                    break;
                default:
                    throw new System.Exception("invalid gun type");
            }

            if (shootDelay > currentGun.ShootDelay)
            {
                Ammo bullet = BulletObject.GetComponent<Ammo>();
                bullet.Damage = currentGun.Damage;
                bullet.Radius = CurrentGunType == GunType.Grenade ? 6.0f : 0.0f;

                Vector3 forward = TransformDirectionRelativeToPlayer(Vector3.forward);
                GameObject bulletInstance = Instantiate(BulletObject, transform.position + (forward + Vector3.up) * 0.8f, transform.rotation);
                Rigidbody bulletRigidBody = bulletInstance.GetComponent<Rigidbody>();

                if (CurrentGunType == GunType.Pistol || CurrentGunType == GunType.Rapid)
                {
                    bulletRigidBody.AddForce(forward * BulletForce, ForceMode.Impulse);

                    // 발사 소리 재생
                    if (GunshotSound != null)
                    {
                        audioSource.PlayOneShot(GunshotSound);
                    }

                    StartRecoil();
                }
                else if (CurrentGunType == GunType.Grenade)
                {
                    /*var mainModule = bulletInstance.GetComponent<ParticleSystem>().main;
                    mainModule.duration = 3.5f;
                    mainModule.startSize = new ParticleSystem.MinMaxCurve(10.5f, 11.5f);*/
                    bulletRigidBody.AddForce(forward * BulletForce / 3, ForceMode.Impulse);

                    // 발사 소리 재생
                    if (GunshotSound != null)
                    {
                        audioSource.PlayOneShot(GunshotSound);
                    }
                }
                else if (CurrentGunType == GunType.Shotgun) {

                    int numberOfPellets = 10;
                    for (int i = 0; i < numberOfPellets; i++)
                    {
                        // 단위원 위 무작위 2D 벡터를 얻음
                        Vector2 randomDirection2D = Random.insideUnitCircle;

                        // 플레이어의 방향을 기준으로 1m 앞에 위치한 1m 반경의 원 위로 이동
                        Vector3 spawnPosition = transform.position + (TransformDirectionRelativeToPlayer(Vector3.forward) + Vector3.up) * 0.8f;

                        // 2D 벡터를 3D로 확장하여 힘을 가함
                        Vector3 randomDirection3D = new Vector3(randomDirection2D.x, randomDirection2D.y, 3f).normalized;
                        bulletRigidBody.AddForce(randomDirection3D * BulletForce / 15, ForceMode.Impulse);

                        // 데미지를 펠릿 수로 나눔
                        Pellet pellet = bulletInstance.GetComponent<Pellet>();
                        pellet.Damage = currentGun.Damage / numberOfPellets;
                    }
                    StartRecoil();
                }

                shootDelay = 0;
            }
        }

        shootDelay += Time.deltaTime;
    }

    void StartRecoil()
    {
        if (!IsShaking) StartCoroutine(RecoilCoroutine());
    }

    IEnumerator RecoilCoroutine()
    {
        IsShaking = true;
        body.AddForce(-transform.forward.x * RecoilForce, 
            Mathf.Min(-transform.forward.y * RecoilForce, 1.0f), -transform.forward.z * RecoilForce, ForceMode.Impulse);

        float halfDuration = RecoilDuration / 2;
        float elapsed = 0f;
        float tick = Random.Range(0f, 500f);

        while (elapsed < RecoilDuration)
        {

            Vector3 noise = new Vector3(0, 0, -1) * Mathf.PerlinNoise(10, tick);
                        
            /*Vector3 noise = new Vector3(
                Mathf.PerlinNoise(100, tick), 
                Mathf.PerlinNoise(200, tick), 
                Mathf.PerlinNoise(300, tick)) - 0.5f * Vector3.one;*/
            arm.localPosition = ArmOffset + noise * 5.0f * Mathf.PingPong(elapsed, halfDuration);
            tick += Time.deltaTime * 2.0f;
            elapsed += Time.deltaTime / halfDuration;
            yield return null;
        }

        IsShaking = false;
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
}

