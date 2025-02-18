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

    protected AnimationHandler animationHandler;
    protected StatHandler statHandler;

    [SerializeField] public WeaponHandler WeaponPrefab;
    protected WeaponHandler weaponHandler;

    protected bool isAttacking;
    private float timeSinceLastAttck = float.MaxValue;

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        animationHandler = GetComponent<AnimationHandler>();
        statHandler = GetComponent<StatHandler>();

        if (WeaponPrefab != null)
            weaponHandler = Instantiate(WeaponPrefab, weaponPivot);
        else
            weaponHandler = GetComponentInChildren<WeaponHandler>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleAction();
        Rotate(lookDirection);

        HandleAttackDelay();
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
    protected virtual void Movement(Vector2 direction)
    {
        // 기본 이동 속도를 증가시킴 (배율: Speed)
        direction = direction * statHandler.Speed;

        // 넉백 상태일 경우 이동 속도를 감소시키고 넉백 벡터를 추가함
        if (knockbackDuration > 0.0f)
        {
            direction *= 0.2f; // 이동 속도를 20%로 감소
            direction += knockback; // 넉백 적용
        }

        // Rigidbody를 이용하여 실제 이동을 적용
        _rigidbody.velocity = direction;
        
        animationHandler.Move(direction);

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

        weaponHandler?.Rotate(isLeft);
    }

    // 캐릭터가 넉백을 받을 때 호출되는 함수
    public void ApplyKnockback(Transform other, float power, float duration)
    {
        // 넉백 지속 시간을 설정
        knockbackDuration = duration;

        // 충돌한 객체(other)와의 거리 차이를 이용하여 넉백 벡터를 계산 (반대 방향으로 밀려남)
        knockback = -(other.position - transform.position).normalized * power;
    }

    private void HandleAttackDelay()
    {
        if (weaponHandler == null)
            return;

        if (timeSinceLastAttck <= weaponHandler.Delay)
        {
            timeSinceLastAttck += Time.deltaTime;
        }

        if (isAttacking && timeSinceLastAttck > weaponHandler.Delay)
        {
            timeSinceLastAttck = 0;
            Attack();
        }
    }

    protected virtual void Attack()
    {
        if (lookDirection != Vector2.zero)
            weaponHandler?.Attack();
    }

    public virtual void Death()
    {
        _rigidbody.velocity = Vector3.zero;

        foreach(SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            Color color = renderer.color;
            color.a = 0.3f;
            renderer.color = color;
        }

        foreach(Behaviour component in transform.GetComponentsInChildren<Behaviour>())
        {
            component.enabled = false;
        }

        Destroy(gameObject, 2);
    }

}
