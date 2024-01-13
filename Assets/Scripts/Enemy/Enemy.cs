using UnityEngine;

public class Enemy : EnemyBase
{
    ParticleSystem ps;

    public override void Awake()
    {
        base.Awake();
        ps = GetComponent<ParticleSystem>();

        // 파티클 좌표를 월드 좌표 공간으로 설정
        SetSimulationSpaceToWorld();
    }

    void SetSimulationSpaceToWorld()
    {
        var mainModule = ps.main;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
    }

    public override void OnCombat() { }

    public override void Die()
    {
        ps.Stop();
        ps.Clear();
        Destroy(gameObject, 3f);
    }

    void OnParticleTrigger()
    {
        GameManager.DamageToPlayer(Damage);
    }
}
