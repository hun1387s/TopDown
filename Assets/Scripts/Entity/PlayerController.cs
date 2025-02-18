using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : BaseController
{
    private Camera cameraMain;
    GameManager gameManager;

    public void Init(GameManager gameMgr)
    {
        this.gameManager = gameMgr;
        cameraMain = Camera.main;
        animationHandler = GetComponent<AnimationHandler>();
    }

    protected override void HandleAction()
    {
        
    }

    public override void Death()
    {
        base.Death();
        gameManager.GameOver();
    }

    // 플레이어 점프 구현을 위한 오버라이드
    protected override void Movement(Vector2 direction)
    {
        base.Movement(direction);

        // 점프 해제
        animationHandler.Jump(false);
    }

    // 플레이어 이동
    void OnMove(InputValue inputValue)
    {
        movementDirection = inputValue.Get<Vector2>();
        movementDirection = movementDirection.normalized;
    }

    // 플레이어 방향 설정
    void OnLook(InputValue inputValue)
    {
        Vector2 mousePosition = inputValue.Get<Vector2>();
        Vector2 worldPos = cameraMain.ScreenToWorldPoint(mousePosition);

        lookDirection = (worldPos - (Vector2)transform.position);

        if (lookDirection.magnitude < 0.9f)
        {
            lookDirection = Vector2.zero;
        }
        else
        {
            lookDirection = lookDirection.normalized;
        }
    }

    // 플레이어 공격
    void OnFire(InputValue inputValue)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        isAttacking = inputValue.isPressed;
    }

    // 플레이어 점프
    void OnJump(InputValue inputValue)
    {
        animationHandler.Jump(true);
    }
}
