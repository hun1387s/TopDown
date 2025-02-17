using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;

    [SerializeField]
    private SpriteRenderer characterRenderer;
    [SerializeField]
    private Transform weaponPivot;

    protected Vector2 movementDirection = Vector2.zero;
    public Vector2 MovementDirection { get { return movementDirection; } }

    protected Vector2 lookDirection = Vector2.zero;
    public Vector2 LookDirection { get { return lookDirection; } }

    private Vector2 knockback = Vector2.zero;
    private float knockbackDuration = 0f;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleAction();
        Rotate(lookDirection);
    }

    protected virtual void FixedUpdate()
    {
        Movement(movementDirection);
        if(knockbackDuration > 0.0f)
        {
            knockbackDuration -= Time.fixedDeltaTime;
        }
    }

    protected virtual void HandleAction()
    {

    }

    // 캐릭터의 이동을 담당하는 함수
    private void Movement(Vector2 direction)
    {
        // 기본 이동 속도를 증가시킴 (배율: 5)
        direction = direction * 5;

        // 넉백 상태일 경우 이동 속도를 감소시키고 넉백 벡터를 추가함
        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f; // 이동 속도를 20%로 감소
            direction += knockback; // 넉백 적용
        }

        // Rigidbody를 이용하여 실제 이동을 적용
        _rigidbody.velocity = direction;
    }

    // 캐릭터의 방향을 회전시키는 함수
    private void Rotate(Vector2 direction)
    {
        // 방향 벡터를 이용하여 회전 각도를 계산
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 캐릭터가 왼쪽을 바라보는지 여부를 판단
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        // 스프라이트의 방향을 조정하여 캐릭터가 올바른 방향을 바라보도록 설정
        characterRenderer.flipX = isLeft;

        // 무기 피벗이 존재하는 경우 무기의 회전도 조정
        if (weaponPivot != null)
        {
            weaponPivot.rotation = Quaternion.Euler(0f, 0f, rotZ);
        }
    }

    // 캐릭터가 넉백을 받을 때 호출되는 함수
    public void AppluKnockback(Transform other, float power, float duration)
    {
        // 넉백 지속 시간을 설정
        knockbackDuration = duration;

        // 충돌한 객체(other)와의 거리 차이를 이용하여 넉백 벡터를 계산 (반대 방향으로 밀려남)
        knockback = -(other.position - transform.position).normalized * power;
    }




}
