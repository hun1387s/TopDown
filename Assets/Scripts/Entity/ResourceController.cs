using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    [SerializeField] private float healthChangeDelay = 0.5f;

    private BaseController baseController;
    private StatHandler statHandler;
    private AnimationHandler animationHandler;

    private float timeSinceLastChange = float.MaxValue;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => statHandler.Health;

    private void Awake()
    {
        baseController = GetComponent<BaseController>();
        statHandler = GetComponent<StatHandler>();
        animationHandler = GetComponent<AnimationHandler>();
    }

    private void Start()
    {
        // statHandler에서 캐릭터의 초기 체력을 가져와 설정
        CurrentHealth = statHandler.Health;
    }

    private void Update()
    {
        // 최근 체력 변경 이후 경과 시간이 설정된 지연 시간보다 짧으면 증가시킴
        if (timeSinceLastChange < healthChangeDelay)
        {
            timeSinceLastChange += Time.deltaTime; // 경과 시간 업데이트

            // 무적 시간이 끝나면 InvincibilityEnd() 호출하여 무적 해제
            if (timeSinceLastChange >= healthChangeDelay)
            {
                animationHandler.InvincibilityEnd();
            }
        }
    }

    // 체력을 변경하는 함수 (데미지 또는 회복)
    public bool ChangeHealth(float change)
    {
        // 변경 값이 0이거나 무적 시간(지연 시간) 내에는 체력 변경 불가
        if (change == 0 || timeSinceLastChange < healthChangeDelay) return false;

        // 체력 변경 시간을 초기화 (무적 상태 해제)
        timeSinceLastChange = 0f;

        // 체력 변경 적용
        CurrentHealth += change;

        // 체력이 최대 체력을 초과하면 최대 체력으로 설정
        CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth;

        // 체력이 0 미만이면 0으로 설정 (체력이 음수가 되지 않도록 방지)
        CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;

        // 체력이 감소했을 경우 데미지 애니메이션 실행
        if (change < 0)
        {
            animationHandler.Damage();
        }

        // 체력이 0 이하가 되면 사망 처리
        if (CurrentHealth <= 0f)
        {
            Death();
        }

        return true; // 체력 변경 성공
    }

    private void Death()
    {
        baseController.Death();
    }


}
