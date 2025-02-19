using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [SerializeField] TilemapCollider2D tilemapCollider;
    [SerializeField] LayerMask layer;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log($"트리거 충돌 감지: {collider.gameObject.name}");
        
        // 플레이어와 충돌했는지
        if (layer == (layer.value | (1 << collider.gameObject.layer)))
        {
            SceneManager.LoadScene(1); // 미니게임 진입
        }
    }
}
