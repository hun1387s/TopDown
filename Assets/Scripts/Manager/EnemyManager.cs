using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Coroutine waveRoutine;

    [SerializeField] private List<GameObject> enemyPrefabs; // 생성할 적 프리팹 목록

    [SerializeField] List<Rect> spawnAreas; // 적이 스폰될 수 있는 영역 목록
    [SerializeField] private Color gizmoColor = new Color(1, 0, 0, .3f); // 기즈모 색상 설정
    List<EnemyController> activeEnemies = new List<EnemyController>(); // 현재 활성화된 적 리스트

    bool enemySpawnComplite; // 적 스폰 완료 여부

    [SerializeField] float timeBetweenSpawns = 0.2f; // 적 스폰 간격
    [SerializeField] float timeBetweenWaves = 1f; // 웨이브 간격

    GameManager gameManager;

    public void Init(GameManager gameMgr)
    {
        this.gameManager = gameMgr;
    }

    // 웨이브 시작 함수
    public void StartWave(int waveCount)
    {
        if (waveCount <= 0)
        {
            gameManager.EndOfWave();
            return;
        }

        if (waveRoutine != null)
            StopCoroutine(waveRoutine); // 기존 웨이브 중지
        waveRoutine = StartCoroutine(SpawnWave(waveCount)); // 새로운 웨이브 시작
    }

    // 웨이브 정지 함수
    public void StopWave()
    {
        StopAllCoroutines(); // 모든 코루틴 중지
    }

    // 웨이브에 따라 적을 생성하는 코루틴
    IEnumerator SpawnWave(int waveCount)
    {
        enemySpawnComplite = false;
        yield return new WaitForSeconds(timeBetweenWaves); // 웨이브 시작 전 대기

        for (int i = 0; i < waveCount; i++)
        {
            yield return new WaitForSeconds(timeBetweenSpawns); // 적 스폰 간격 대기
            SpawnRandomEnemy(); // 랜덤 적 생성
        }

        enemySpawnComplite = true;
    }

    // 랜덤 적을 생성하는 함수
    void SpawnRandomEnemy()
    {
        if (enemyPrefabs.Count == 0 || spawnAreas.Count == 0)
        {
            Debug.LogWarning("Enemy Prefabs or Spawn Areas가 생성되지 않았다.");
            return;
        }

        // 랜덤한 적 프리팹 선택
        GameObject randomPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        // 랜덤한 스폰 영역 선택
        Rect randomArea = spawnAreas[Random.Range(0, spawnAreas.Count)];

        // 해당 영역 내에서 랜덤 위치 선정
        Vector2 randomPosition = new Vector2(
            Random.Range(randomArea.xMin, randomArea.xMax),
            Random.Range(randomArea.yMin, randomArea.yMax));

        // 적을 스폰하고 리스트에 추가
        GameObject spawnEnemy = Instantiate(randomPrefab,
            new Vector3(randomPosition.x, randomPosition.y), Quaternion.identity);
        EnemyController enemyController = spawnEnemy.GetComponent<EnemyController>();
        enemyController.Init(this, gameManager.player.transform);

        activeEnemies.Add(enemyController);
    }


    // 기즈모를 통해 스폰 영역을 시각적으로 표시하는 함수
    private void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;

        Gizmos.color = gizmoColor;
        foreach (var area in spawnAreas)
        {
            Vector3 center = new Vector3(area.x + area.width / 2, area.y + area.height / 2);
            Vector3 size = new Vector3(area.width, area.height);

            Gizmos.DrawCube(center, size); // 기즈모로 스폰 영역을 시각화
        }
    }

    public void RemoveEnemyOnDeath(EnemyController enemy)
    {
        activeEnemies.Remove(enemy);
        if (enemySpawnComplite && activeEnemies.Count == 0)
        {
            gameManager.EndOfWave();
        }
    }
}
