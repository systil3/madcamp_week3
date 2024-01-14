using System;
using UnityEngine;

public enum GunType
{
    Pistol,
    Rapid,
    Grenade,
}

[Serializable]
public class Gun
{
    public GunType Type;
    public float Damage;
    public float ShootDelay;
    public Transform Transform;
}