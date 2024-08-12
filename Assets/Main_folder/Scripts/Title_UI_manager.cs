
using UnityEngine;
using UnityEngine.UI;

public class Title_UI_manager : MonoBehaviour
{
    [Header("타이틀 버튼 목록")] 
    public Button adventureButton;
    public Button trainingButton;
    public Button settingButton;
    
    [Header("씬 이동 효과 및 씬 매니져")] 
    public Scenemanager Scenemanager;
    
    [Header("씬 목록")]
    public SceneReference  adventureSceneName;
    public SceneReference  trainingSceneName;
    
    private void Start()
    {
        adventureButton.onClick.AddListener(() =>
        {
            Scenemanager.LoadScene(adventureSceneName);
        });
        
        trainingButton.onClick.AddListener(() =>
        {
            Scenemanager.LoadScene(trainingSceneName);
        });
        
        settingButton.onClick.AddListener(() =>
        {
            Scenemanager.LoadScene("Setting");
        });
    }
    
    
    
}