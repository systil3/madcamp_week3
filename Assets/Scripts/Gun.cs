using System;
using UnityEngine;

public enum GunType
{
    Pistol,
    Rapid,
    Grenade,
    Shotgun
}

[Serializable]
public class Gun
{
    public GunType Type;
    public float Damage;
    public float ShootDelay;
    public Transform Transform;
    public float RecoilForce;
    public float RecoilDuration;
}