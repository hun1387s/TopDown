using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private static ProjectileManager instance;
    public static ProjectileManager Instance { get { return instance; } }

    [SerializeField] private GameObject[] projectilePrefabs;

    private void Awake()
    {
        instance = this;
    }

    // 총알을 발사하는 함수
    public void ShootBullet(RangeWeaponHandler rangeWeaponHandler, Vector2 startPosition, Vector2 direction)
    {
        // 무기 핸들러의 탄환 인덱스를 사용하여 발사할 탄환 프리팹을 가져옴
        GameObject origin = projectilePrefabs[rangeWeaponHandler.BulletIndex];

        // 탄환 오브젝트를 생성 (위치: startPosition, 회전: 없음)
        GameObject obj = Instantiate(origin, startPosition, Quaternion.identity);

        // 생성된 탄환의 ProjectileController 컴포넌트를 가져옴
        ProjectileController projectileController = obj.GetComponent<ProjectileController>();

        // 탄환 초기화 (이동 방향 및 무기 핸들러 정보 설정)
        projectileController.Init(direction, rangeWeaponHandler);
    }






}
