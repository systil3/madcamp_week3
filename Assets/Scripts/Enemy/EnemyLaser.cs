using UnityEngine;

// 직선 방향 레이저만 사용
public class EnemyLaser : EnemyLaserBase
{
    public override RaycastHit MakeLaser(string laserType = "unknown")
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

    public override void Die()
    {
        Destroy(gameObject, 3f);
    }
}
