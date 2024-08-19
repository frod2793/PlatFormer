using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class HP_UIManager : MonoBehaviour
{
    [Header("Skill Icons")]
    public List<Image> skillIcons = new List<Image>();
    public List<Image> skillIconFills = new List<Image>();
    public List<TMPro.TextMeshProUGUI> skillCoolTimeTexts = new List<TMPro.TextMeshProUGUI>();

    private List<float> skillCooldowns;
    private List<float> skillCooldownTimers;
    private List<bool> skillIsCoolingDown;
    
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

    [FormerlySerializedAs("uiElement")]
    public Slider PlayerHp_Slider; // UI 요소의 RectTransform
    private Camera mainCamera;
    private RectTransform PlayerHp_SliderRectTransform;
    
    private PlayerController playerController;
    
    EnemyManager enemyManager;
    
    private void Start()
    {    
        playerController = FindObjectOfType<PlayerController>();
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera 못찾음 ");
            return;
        }
        Enemy_UI_Init();
        InitializeSkills();
        PlayerHp_SliderRectTransform = PlayerHp_Slider.GetComponent<RectTransform>();
    }


    #region Enemy_UI

    private void Enemy_UI_Init()
    {
        enemyManager = FindObjectOfType<EnemyManager>();

        for (int i = 0; i < enemyManager.Enemy_targetsList.Count; i++)
        {
            enemyManager.Enemy_targetsList[i].DeadDelegate += DeleteUIElement;
            Debug.Log("이벤트 추가");
        }
        
        if (enemyManager.Enemy_targetsList != null)
        {
            Enemy_uiElements = new RectTransform[enemyManager.Enemy_targetsList.Count];
            Enemy_offsets = new Vector3[enemyManager.Enemy_targetsList.Count];

            for (int i = 0; i < enemyManager.Enemy_targetsList.Count; i++)
            {
                if (enemyManager.Enemy_targetsList[i].gameObject != null)
                {
                    Slider ui = Instantiate(Enemy_UIPrFeb, canvas.transform);
                    ui.maxValue = enemyManager.Enemy_targetsList[i].hp;
                    ui.value = enemyManager.Enemy_targetsList[i].hp;
                    Enemy_uiElements[i] = ui.GetComponent<RectTransform>();
                    Enemy_offsets[i] = new Vector3(0, 1, 0); // UI 오프셋 설정
                    Debug.Log("적 UI 생성");
                }
            }
        }
        else
        {
            Debug.LogError("적이 없음");
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
    #endregion



    #region Skill_UI
    private void InitializeSkills()
    {      
        playerController.OnPlayerAttack += TriggerSkill;
        int skillCount = skillIcons.Count;

        // 리스트 초기화
        skillCooldowns = new List<float>(skillCount);
        skillCooldownTimers = new List<float>(skillCount);
        skillIsCoolingDown = new List<bool>(skillCount);

        for (int i = 0; i < skillCount; i++)
        {
            Image fillImage = skillIconFills[i];
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Radial360;
            fillImage.fillOrigin = (int)Image.Origin360.Top;
            fillImage.fillClockwise = false;
            fillImage.fillAmount = 0;

            skillIcons[i].gameObject.SetActive(true);
            skillCoolTimeTexts[i].gameObject.SetActive(false);

            skillCooldowns.Add(0f); // 모든 스킬의 쿨타임을 기본값 5초로 설정
            skillCooldownTimers.Add(0f); // 쿨타임 타이머 초기화
            skillIsCoolingDown.Add(false); // 스킬 쿨타임 상태 초기화
        }
    }
    
    private void UpdateSkillCooldowns()
    {
        int skillCount = skillIcons.Count;

        for (int i = 0; i < skillCount; i++)
        {
            if (skillIsCoolingDown[i])
            {
                skillCooldownTimers[i] -= Time.deltaTime;
                float fillAmount = skillCooldownTimers[i] / skillCooldowns[i];
                skillIconFills[i].fillAmount = fillAmount;
                skillCoolTimeTexts[i].text = Mathf.Ceil(skillCooldownTimers[i]).ToString();
                if (!skillCoolTimeTexts[i].gameObject.activeSelf)
                {
                    skillCoolTimeTexts[i].gameObject.SetActive(true);
                }

                if (skillCooldownTimers[i] <= 0)
                {
                    skillIsCoolingDown[i] = false;
                    skillIconFills[i].fillAmount = 0;
                    if (skillCoolTimeTexts[i].gameObject.activeSelf)
                    {
                        skillCoolTimeTexts[i].gameObject.SetActive(false);
                    }
                    skillIcons[i].gameObject.SetActive(true);
                }
            }
        }
    }
    
    public void TriggerSkill(int index,float SkillCooldowns)
    {
        if (index < 0 || index >= skillIcons.Count) return;

        if (!skillIsCoolingDown[index])
        {
            skillCooldowns[index] = SkillCooldowns;
            skillIsCoolingDown[index] = true;
            skillCooldownTimers[index] = skillCooldowns[index];
            // if (skillIcons[index].gameObject.activeSelf)
            // {
            //     skillIcons[index].gameObject.SetActive(false);
            // }
        }
        else
        {
            Debug.LogError("Skill is on cooldown!");
        }
    }
    #endregion
    
    private void LateUpdate()
    {
        UpdateUIPosition(target, PlayerHp_SliderRectTransform, offset);
        UpdateUIValue(PlayerHp_Slider,playerController.Hp);

        if (enemyManager.Enemy_targetsList.Count>= 0 && Enemy_uiElements != null)
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
        
        UpdateSkillCooldowns();
        
    }

    
 

    private void UpdateUIValue(Slider ui, float value)
    {
        if (ui == null) return;

        // Debug.Log("체력 갱신");
        // Debug.Log(value);
        // Debug.Log(ui.value);
        // Debug.Log(ui.name);
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
