using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class HP_UIManager : MonoBehaviour
{

 [Header("UIcanvas")]
    public GameObject canvas;

    [Header("Enemy_UI")]
    public Slider Enemy_UIPrFeb; // 추가될 체력 UI 프리팹
   // public EnemyBase[] Enemy_targets; // 따라다닐 대상 오브젝트
    public Vector3[] Enemy_offsets; // UI의 오프셋 값
    public RectTransform[] Enemy_uiElements; // UI 요소의 RectTransform
    
    [Header("Player_UI")]
    public Transform target; // 따라다닐 대상 오브젝트
    public Vector3 offset; // UI의 오프셋 값

    [FormerlySerializedAs("uiElement")] public Slider PlayerHp_Slider; // UI 요소의 RectTransform
    private Camera mainCamera;
    private RectTransform PlayerHp_SliderRectTransform;
    
    private PlayerController playerController;
    
    EnemyManager enemyManager;
    
    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera 못찾음 ");
            return;
        }

        Enemy_UI_Init();
        PlayerHp_SliderRectTransform = PlayerHp_Slider.GetComponent<RectTransform>();
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Enemy_UI_Init()
    {
        enemyManager = FindObjectOfType<EnemyManager>();

        for (int i = 0; i < enemyManager.Enemy_targetsList.Count; i++)
        {
            enemyManager.Enemy_targetsList[i].DeadDelegate += DeleteUIElement;
            Debug.Log("이벤트 추가");
        }
        
        if (enemyManager.Enemy_targetsList != null && enemyManager.Enemy_targetsList.Count > 0)
        {
            Enemy_uiElements = new RectTransform[enemyManager.Enemy_targetsList.Count];
            Enemy_offsets = new Vector3[enemyManager.Enemy_targetsList.Count];

            for (int i = 0; i < enemyManager.Enemy_targetsList.Count; i++)
            {
                if (enemyManager.Enemy_targetsList[i] != null)
                {
                    Slider ui = Instantiate(Enemy_UIPrFeb, canvas.transform);
                    ui.maxValue = enemyManager.Enemy_targetsList[i].hp;
                    ui.value = enemyManager.Enemy_targetsList[i].hp;
                    Enemy_uiElements[i] = ui.GetComponent<RectTransform>();
                    Enemy_offsets[i] = new Vector3(0, 1, 0); // UI 오프셋 설정
                }
            }
        }
    }

    private void LateUpdate()
    {
        UpdateUIPosition(target, PlayerHp_SliderRectTransform, offset);
        UpdateUIValue(PlayerHp_Slider, playerController.Hp);

        if (enemyManager.Enemy_targetsList != null && Enemy_uiElements != null)
        {
            for (int i = 0; i < enemyManager.Enemy_targetsList.Count; i++)
            {
                if (enemyManager.Enemy_targetsList[i] != null && Enemy_uiElements[i] != null)
                {
                    UpdateUIPosition(enemyManager.Enemy_targetsList[i].transform, Enemy_uiElements[i], Enemy_offsets[i]);
                    UpdateUIValue(Enemy_uiElements[i].GetComponent<Slider>(), enemyManager.Enemy_targetsList[i].hp);
                }
            }
        }
    }

    
    private void DeleteUIElement(GameObject obj)
    {
        for (int i = 0; i < enemyManager.Enemy_targetsList.Count ; i++)
        {
            if (enemyManager.Enemy_targetsList[i].gameObject == obj)
            {
                Destroy(Enemy_uiElements[i].gameObject);
                enemyManager.RemoveEnemy(obj);
                Debug.Log("적 UI 제거");
            }
        }
        
    }
    

    private void UpdateUIValue(Slider ui, float value)
    {
        if (ui == null) return;

        ui.value = value;
    }

    private void UpdateUIPosition(Transform targetTransform, RectTransform uiTransform, Vector3 offset)
    {
        if (targetTransform == null)
        {
            Destroy(uiTransform.gameObject);
            enemyManager.Enemy_targetsList.Remove(targetTransform.GetComponent<EnemyBase>());
        }
        
        if (targetTransform == null || uiTransform == null || mainCamera == null) return;

        // 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetTransform.position + offset);

        // 스크린 좌표를 월드 캔버스의 로컬 좌표로 변환
        RectTransform parentRectTransform = uiTransform.parent as RectTransform;
        if (parentRectTransform == null)
        {
            Debug.LogError("UI Element 부모없음 ");
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRectTransform,
            screenPos,
            mainCamera,
            out Vector2 localPos))
        {
            // UI 요소의 위치 설정
            uiTransform.localPosition = localPos;
        }
        
    }
}
