using UnityEngine;

//원형 회전 레이저만 사용

public class EnemyLaserCircle : EnemyLaserBase
{
    public float LaserAngularSpeed = 3.0f; // 각속도
    public float LaserPlaneRadius = 15.0f; // 동심원 반지름
    public float LaserAngle = 0;

    public override RaycastHit MakeLaser()
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
}
