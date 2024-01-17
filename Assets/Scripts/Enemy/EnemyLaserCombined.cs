using UnityEngine;

public enum LaserType
{
    Unknown,
    Line,
    Circle,
}

// 2/3의 확률로 직선, 1/3의 확률로 원형 방향 레이저 사용
public class EnemyLaserCombined : EnemyLaserBase
{
    protected LaserType laserType = LaserType.Unknown; // 발사 유형
    //원형으로 쏠 시
    public float LaserAngularSpeed = 10; // 각속도
    public float LaserPlaneRadius = 5.0f; // 동심원 반지름
    public float LaserAngle = 0;

    public override RaycastHit MakeLaser()
    {
        switch (laserType)
        {
            case LaserType.Line:
                return MakeLaserLine();
            case LaserType.Circle:
                return MakeLaserCircle();
            default:
                throw new System.Exception("unknown type of firing laser");
        }
    }

    public RaycastHit MakeLaserLine()
    {
        Vector3 rayDirection = (laserEndPoint - laserStartPoint).normalized;
        Ray ray = new Ray(laserStartPoint, rayDirection);

        bool cast = Physics.Raycast(ray, out RaycastHit hit, MaxRayLength);
        // 충돌 시 위치 계산 (레이캐스트로)
        // 다른 물체에 닿을 시 거기서 멈추기

        LaserLine.SetPosition(0, laserStartPoint);

        if (!cast)
        {
            hitPosition = laserEndPoint + (laserEndPoint - laserStartPoint) * 10;
        }
        else
        {
            hitPosition = hit.point;
        }

        LaserLine.SetPosition(1, hitPosition);
        return hit;
    }

    public RaycastHit MakeLaserCircle()
    {
        // 플레이어의 x좌표, z좌표를 감지하여 반지름 r의 동심원 형성
        LaserAngle = LaserTime * LaserAngularSpeed;

        Vector3 radEndPoint = new Vector3(laserEndPoint.x + LaserPlaneRadius * Mathf.Cos(LaserAngle),
                            laserEndPoint.y, laserEndPoint.z + LaserPlaneRadius * Mathf.Sin(LaserAngle));
        Vector3 rayDirection = (radEndPoint - laserStartPoint).normalized;
        Ray ray = new Ray(laserStartPoint, rayDirection);

        bool cast = Physics.Raycast(ray, out RaycastHit hit, MaxRayLength);
        // 충돌 시 위치 계산 (레이캐스트로)
        // 다른 물체에 닿을 시 거기서 멈추기

        LaserLine.SetPosition(0, laserStartPoint);

        if (!cast)
        {
            hitPosition = radEndPoint + (radEndPoint - laserStartPoint) * 10;
        }
        else
        {
            hitPosition = hit.point;
        }

        LaserLine.SetPosition(1, hitPosition);
        return hit;
    }

    public override void OnDormant()
    {
        //다음에 쏠 타입 정하기 : 2/3의 확률로 직선, 1/3의 확률로 원형
        float laserRandomValue = Random.Range(0f, 1f);
        laserType = laserRandomValue < 0.33f ? LaserType.Circle : LaserType.Line;
        LaserFiringTime = laserType == LaserType.Circle ? 2f : 0.5f;

        base.OnDormant();
    }
}
