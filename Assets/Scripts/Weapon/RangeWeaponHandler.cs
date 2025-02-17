using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeaponHandler : WeaponHandler
{
    [Header("Range Attack Data")]
    [SerializeField] private Transform projectileSpawnPosition;

    [SerializeField] private int bulletIndex;
    public int BulletIndex { get { return bulletIndex; } }

    [SerializeField] private float bulletSize = 1f;
    public float BulletSize { get { return bulletSize; } }

    [SerializeField] private float duration;
    public float Duration { get { return duration; } }

    [SerializeField] private float spread;
    public float Spread { get { return spread; } }

    [SerializeField] private int numerofProjectilesPerShot;
    public int NumerofProjectilesPerShot { get { return numerofProjectilesPerShot; } }

    [SerializeField] private float multipleProjectileAngle;
    public float MultipleProjectileAngle { get { return multipleProjectileAngle; } }

    [SerializeField] private Color projectileColor;
    public Color ProjectileColor { get { return projectileColor; } }

    public override void Attack()
    {
        base.Attack();

        float projectileAngleSpace = multipleProjectileAngle;
        int numerofProjectilePerShot = numerofProjectilesPerShot;

        float minAngle = -(numerofProjectilePerShot / 2) * projectileAngleSpace;

        for(int i =0; i<numerofProjectilePerShot; i++)
        {
            float angle = minAngle + projectileAngleSpace * i;
            float randomSpread = Random.Range(-spread, spread);
            angle += randomSpread;
            CreateProjectile(Controller.LookDirection, angle);
        }
    }

    private void CreateProjectile(Vector2 _lookDir, float angle)
    {

    }

    private static Vector2 RotateVector2(Vector2 v, float degree)
    {
        return Quaternion.Euler(0, 0, degree) * v; // 교환법칙 불가
    }

}
 