using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController
{
    private EnemyManager enemyManager;
    private Transform target;

    [SerializeField] private float followRange = 15f;

    // 적의 매니저와 목표를 초기화하는 함수
    public void Init(EnemyManager enemyManager, Transform target)
    {
        this.enemyManager = enemyManager; // 적 매니저 저장
        this.target = target; // 목표 저장
    }

    // 목표와의 거리 계산 함수
    protected float DistanceToTarget()
    {
        return Vector3.Distance(transform.position, target.position);
    }


    // 목표까지의 방향을 반환하는 함수
    protected Vector2 DirectionToTarget()
    {
        return (target.position - transform.position).normalized;
    }


    // 적의 행동을 처리하는 함수
    protected override void HandleAction()
    {
        base.HandleAction(); // 부모 클래스의 HandleAction() 실행

        // 무기 핸들러나 목표가 없을 경우 이동을 멈춤
        if (weaponHandler == null || target == null)
        {
            if (!movementDirection.Equals(Vector2.zero))
            {
                movementDirection = Vector2.zero;
                return;
            }
        }

        // 목표와의 거리 및 방향 계산
        float distance = DistanceToTarget();
        Vector2 direction = DirectionToTarget();

        isAttacking = false; // 초기화

        // 목표가 따라갈 수 있는 범위 내에 있는지 확인
        if (distance <= followRange)
        {
            lookDirection = direction; // 목표 방향으로 바라봄

            // 공격 범위 내에 있는지 확인
            if (distance < weaponHandler.AttackRange)
            {
                int layerMaskTarget = weaponHandler.target;

                // 목표 방향으로 레이캐스트 실행 (공격 가능 여부 확인)
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction,
                    weaponHandler.AttackRange * 1.5f,
                    (1 << LayerMask.NameToLayer("Level")) | layerMaskTarget);

                // 레이캐스트가 목표에 적중했는지 확인
                if (hit.collider != null && layerMaskTarget ==
                    (layerMaskTarget | (1 << hit.collider.gameObject.layer)))
                {
                    isAttacking = true; // 공격 실행 가능
                }

                movementDirection = Vector2.zero; // 공격할 때 이동 정지
                return;
            }
            movementDirection = direction;
        }
    }

    public override void Death()
    {
        base.Death();
        enemyManager.RemoveEnemyOnDeath(this);
    }
}