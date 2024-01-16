using UnityEngine;

public class SurfaceParticles : MonoBehaviour
{
    public ParticleSystem ParticleSystemPrefab;
    public int NumberOfParticles = 100;

    ParticleSystem particleSystemInstance;
    Transform particleTransform;

    void Start()
    {
        // 파티클 시스템 프리팹을 인스턴스화
        particleSystemInstance = Instantiate(ParticleSystemPrefab);
        particleTransform = particleSystemInstance.transform;

        // 구 모양의 오브젝트 표면에 파티클 배치
        PlaceParticlesOnSurface();
    }

    void PlaceParticlesOnSurface()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.sharedMesh;

            for (int i = 0; i < NumberOfParticles; i++)
            {
                // 랜덤한 표면 점 샘플링
                Vector3 randomPointOnSurface = Random.onUnitSphere * 0.5f; // 0.5f는 반지름 조절

                // 표면 노말 가져오기
                Vector3 surfaceNormal = mesh.normals[0]; // 여기서는 단순하게 첫 번째 정점의 노말 사용

                // 파티클 위치 및 회전 설정
                particleTransform.position = transform.TransformPoint(randomPointOnSurface);
                particleTransform.rotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);

                // 파티클 생성
                particleSystemInstance.Emit(1);
            }
        }
        else
        {
            Debug.LogError("MeshFilter not found on the object.");
        }
    }
}
