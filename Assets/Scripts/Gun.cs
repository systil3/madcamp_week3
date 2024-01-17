using System;
using UnityEngine;

public enum GunType
{
    Pistol,
    SMG,
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
    public AudioClip GunShotSound;

    public static GameObject GunTypeToObject(Transform armTransform, GunType gunType)
    {
        switch (gunType)
        {
            case GunType.Pistol:
                return armTransform.Find("Pistol").gameObject;
            case GunType.SMG:
                return armTransform.Find("SMG").gameObject;
            case GunType.Grenade:
                return armTransform.Find("Grenade").gameObject;
            case GunType.Shotgun:
                return armTransform.Find("Shotgun").gameObject;
            default:
                throw new Exception("Invalid Gun Type");
        }
    }

    public static GameObject GetCrossHairObject(GunType gunType)
    {
        switch (gunType)
        {
            case GunType.Pistol:
            case GunType.SMG:
                return Camera.main.transform.GetChild(0).GetChild(0).gameObject;
            case GunType.Grenade:
                return Camera.main.transform.GetChild(0).GetChild(1).gameObject;
            case GunType.Shotgun:
                return Camera.main.transform.GetChild(0).GetChild(2).gameObject;
            default:
                throw new Exception("Invalid Gun Type");
        }
    }
}