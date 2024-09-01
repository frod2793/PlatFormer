using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase platformTile;
    public TileBase obstacleTile;
    public TileBase powerUpTile;
    public GameObject enemyPrefab; // 적 게임 오브젝트 프리팹
    public int initialMapWidth = 20;
    public int mapHeight = 10;
    public float platformChance = 0.5f;
    public float enemyChance = 0.05f;
    public float obstacleChance = 0.1f;
    public float powerUpChance = 0.02f;
    public float minPlatformLength = 3f;
    public float maxPlatformLength = 6f;
    public float mapExtendDistance = 20f; // 맵을 확장할 거리

    public GameObject backgroundPrefab;  // 배경 프리팹
    public int backgroundCount = 2;      // 배치할 배경의 수
    public float backgroundWidth = 20f;  // 배경의 너비 (이미지의 실제 크기)

    private Transform[] backgrounds;     // 배경 오브젝트들을 저장할 배열
    private Transform player;            // 플레이어 오브젝트

    private float leftMostX;             // 가장 왼쪽 배경의 X 위치
    private float rightMostX;            // 가장 오른쪽 배경의 X 위치
    private float lastGeneratedX;        // 마지막으로 맵이 생성된 X 위치

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        InitBackground();
        GeneratePlatformMap(initialMapWidth);
        lastGeneratedX = initialMapWidth;
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

        // 플레이어가 일정 거리 이상 이동하면 새로운 맵 생성
        if (player.position.x > lastGeneratedX - mapExtendDistance)
        {
            GeneratePlatformMap(initialMapWidth);
            lastGeneratedX += initialMapWidth;
        }
    }

    private void InitBackground()
    {
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

        leftBackground.position = new Vector3(rightMostX + backgroundWidth, 8, leftBackground.position.z);
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

    void GeneratePlatformMap(int width)
    {
        int x = Mathf.FloorToInt(lastGeneratedX);
        int endX = x + width;

        while (x < endX)
        {
            int platformLength = Mathf.FloorToInt(Random.Range(minPlatformLength, maxPlatformLength));
            int y = Random.Range(0, mapHeight);

            for (int i = 0; i < platformLength && x < endX; i++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), platformTile);

                // 적 배치
                if (Random.value < enemyChance)
                {
                    Vector3Int enemyPosition = new Vector3Int(x, y + 1, 0);
                    Instantiate(enemyPrefab, tilemap.CellToWorld(enemyPosition), Quaternion.identity);
                }

                if (obstacleTile != null)
                {
                    // 장애물 배치
                    if (Random.value < obstacleChance)
                    {
                        tilemap.SetTile(new Vector3Int(x, y + 1, 0), obstacleTile);
                    }
                }

                if (powerUpTile != null)
                {
                    // 파워업 배치
                    if (Random.value < powerUpChance)
                    {
                        tilemap.SetTile(new Vector3Int(x, y + 1, 0), powerUpTile);
                    }
                }

                x++;
            }
        }
    }
}
