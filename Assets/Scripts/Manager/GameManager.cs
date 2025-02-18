using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글톤 인스턴스

    public PlayerController player { get; private set; } // 플레이어 컨트롤러 참조
    ResourceController _playerResourceController; // 플레이어의 자원 컨트롤러

    [SerializeField] int currentWaveIndex = 0; // 현재 웨이브 인덱스

    EnemyManager enemyManager; // 적 매니저 참조
    UIManager uiManager; // UI 매니저 참조
    public static bool isFirstLoading = true;

    private void Awake()
    {
        Instance = this; // 싱글톤 패턴 적용

        // 플레이어 컨트롤러 찾고 초기화
        player = FindObjectOfType<PlayerController>();
        player.Init(this);

        uiManager = FindObjectOfType<UIManager>();

        // 적 매니저 찾고 초기화
        enemyManager = GetComponentInChildren<EnemyManager>();
        enemyManager.Init(this);


        _playerResourceController = player.GetComponent<ResourceController>();
        _playerResourceController.RemoveHealthChangeEvent(uiManager.ChangePlayerHP);
        _playerResourceController.AddHealthChangeEvent(uiManager.ChangePlayerHP);

    }
    private void Start()
    {
        if (!isFirstLoading)
        {
            StartGame();
        }
        else
        {
            isFirstLoading = false;
        }
    }

    // 게임을 시작하는 함수
    public void StartGame()
    {
        uiManager.SetPlayGame();
        StartNextWave(); // 첫 번째 웨이브 시작
    }

    // 다음 웨이브를 시작하는 함수
    void StartNextWave()
    {
        currentWaveIndex += 1; // 웨이브 인덱스 증가
        enemyManager.StartWave(1 + currentWaveIndex / 5); // 웨이브 개수 조정
        uiManager.ChangeWave(currentWaveIndex);
    }

    // 웨이브가 끝났을 때 호출되는 함수
    public void EndOfWave()
    {
        StartNextWave(); // 다음 웨이브 시작
    }

    // 게임 오버 시 실행되는 함수
    public void GameOver()
    {
        enemyManager.StopWave(); // 적 스폰 중지
        uiManager.SetGameOver(); 
    }
}
