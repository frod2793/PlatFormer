using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerater : MonoBehaviour
{
    public Tilemap tilemap;        // 타일맵 참조
    public TileBase groundTile;    // 땅 타일
    public TileBase obstacleTile;  // 장애물 타일

    public int mapWidth = 10;      // 타일맵의 너비
    public int mapHeight = 10;     // 타일맵의 높이
    public float obstacleChance = 0.2f;  // 장애물이 생성될 확률

    
    
    public GameObject backgroundPrefab;  // 배경 프리팹
    public int backgroundCount = 3;      // 배치할 배경의 수
    public float backgroundWidth = 20f;  // 배경의 너비 (이미지의 실제 크기)

    private Transform[] backgrounds;     // 배경 오브젝트들을 저장할 배열
    private Transform player;            // 플레이어 오브젝트

    private float leftMostX;             // 가장 왼쪽 배경의 X 위치
    private float rightMostX;            // 가장 오른쪽 배경의 X 위치
    
    
    private void Start()
    {
       // GenerateMap();
       InitBackground();
    }

    private void FixedUpdate()
    {
      
        // 플레이어의 이동에 따라 배경을 재배치
        if (player.position.x - backgroundWidth > leftMostX)
        {
            RepositionBackgroundToRight();
        }
        else if (player.position.x + backgroundWidth < rightMostX)
        {
            RepositionBackgroundToLeft();
        }
    }

    private void InitBackground()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 배경 오브젝트 배열 초기화
        backgrounds = new Transform[backgroundCount];

        // 초기 배경 배치
        for (int i = 0; i < backgroundCount; i++)
        {
            GameObject bg = Instantiate(backgroundPrefab, new Vector3(i * backgroundWidth, 8, 0), Quaternion.identity);
            bg.transform.SetParent(transform);  // 배경을 부모 오브젝트에 자식으로 설정
            backgrounds[i] = bg.transform;
        }

        UpdateBackgroundBounds();
    }
    
    private void RepositionBackgroundToRight()
    {
        // 가장 왼쪽 배경을 가장 오른쪽으로 이동시켜 재사용
        Transform leftBackground = backgrounds[0];

        // 배열 내의 나머지 배경을 왼쪽으로 이동
        for (int i = 0; i < backgroundCount - 1; i++)
        {
            backgrounds[i] = backgrounds[i + 1];
        }

        leftBackground.position = new Vector3(rightMostX + backgroundWidth,8, leftBackground.position.z);
        backgrounds[backgroundCount - 1] = leftBackground;

        UpdateBackgroundBounds();
    }

    private void RepositionBackgroundToLeft()
    {
        // 가장 오른쪽 배경을 가장 왼쪽으로 이동시켜 재사용
        Transform rightBackground = backgrounds[backgroundCount - 1];

        // 배열 내의 나머지 배경을 오른쪽으로 이동
        for (int i = backgroundCount - 1; i > 0; i--)
        {
            backgrounds[i] = backgrounds[i - 1];
        }

        rightBackground.position = new Vector3(leftMostX - backgroundWidth, rightBackground.position.y, rightBackground.position.z);
        backgrounds[0] = rightBackground;

        UpdateBackgroundBounds();
    }

    private void UpdateBackgroundBounds()
    {
        // 좌우 배경 위치 업데이트
        leftMostX = backgrounds[0].position.x;
        rightMostX = backgrounds[backgroundCount - 1].position.x;
    }
    
    private void GenerateMap()
    {
        // 타일맵 초기화
        tilemap.ClearAllTiles();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // 기본적으로 모든 타일에 땅 타일을 배치
                Vector3Int position = new Vector3Int(x, y, 0);
                tilemap.SetTile(position, groundTile);

                // 일정 확률로 장애물 타일을 배치
                if (Random.value < obstacleChance)
                {
                    tilemap.SetTile(position, obstacleTile);
                }
            }
        }
    }
}
