using System;

public enum GunType
{
    Pistol,
    Rapid,
    Grenade,
    Shotgun,
}

[Serializable]
public class Gun
{
    public GunType Type;
    public float Damage;
    public float Force;
    public float ShootDelay;
    public float RecoilForce;
    public float RecoilDuration;
}