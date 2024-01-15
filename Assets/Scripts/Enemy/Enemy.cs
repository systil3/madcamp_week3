using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyBase
{
    public float ParticleForce = 30.0f;

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

    public override void OnDormant() { }

    public override void Die()
    {
        ps.Stop();
        ps.Clear();
        Destroy(gameObject, 3f);
    }

    void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();
        ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

        Vector3 dir = (Player.position - particles[0].position).normalized;
        Player.AddForce(dir * ParticleForce, ForceMode.Impulse);
        StartCoroutine(PlayerForceCoroutine(Player.GetComponent<Player>()));
        GameManager.DamageToPlayer(Damage);
    }

    IEnumerator PlayerForceCoroutine(Player player)
    {
        player.IsFreeze = true;
        yield return new WaitForSeconds(DamageDuration);
        player.IsFreeze = false;
    }
}
