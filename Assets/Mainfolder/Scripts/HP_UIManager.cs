using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_UIManager : MonoBehaviour
{

 [Header("UIcanvas")]
    public GameObject canvas;

    [Header("Enemy_UI")]
    public GameObject Enemy_UIPrFeb; // UI 요소
    public EnemyBase[] Enemy_targets; // 따라다닐 대상 오브젝트
    public Vector3[] Enemy_offsets; // UI의 오프셋 값
    public RectTransform[] Enemy_uiElements; // UI 요소의 RectTransform

    [Header("Player_UI")]
    public Transform target; // 따라다닐 대상 오브젝트
    public Vector3 offset; // UI의 오프셋 값

    public RectTransform uiElement; // UI 요소의 RectTransform
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera 못찾음 ");
            return;
        }

        Enemy_targets = FindObjectsOfType<EnemyBase>();
        if (Enemy_targets != null && Enemy_targets.Length > 0)
        {
            Enemy_uiElements = new RectTransform[Enemy_targets.Length];
            Enemy_offsets = new Vector3[Enemy_targets.Length];

            for (int i = 0; i < Enemy_targets.Length; i++)
            {
                if (Enemy_targets[i] != null)
                {
                    GameObject ui = Instantiate(Enemy_UIPrFeb, canvas.transform);
                    Enemy_uiElements[i] = ui.GetComponent<RectTransform>();
                    Enemy_offsets[i] = new Vector3(0, 1, 0); // UI 오프셋 설정
                }
            }
        }
    }

    private void LateUpdate()
    {
        UpdateUIPosition(target, uiElement, offset);

        if (Enemy_targets != null && Enemy_uiElements != null)
        {
            for (int i = 0; i < Enemy_targets.Length; i++)
            {
                if (Enemy_targets[i] != null && Enemy_uiElements[i] != null)
                {
                    UpdateUIPosition(Enemy_targets[i].transform, Enemy_uiElements[i], Enemy_offsets[i]);
                }
            }
        }
    }

    private void UpdateUIPosition(Transform targetTransform, RectTransform uiTransform, Vector3 offset)
    {
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
