using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Adventure_UI_Manager : UI_manager_Base
{
    [Header("Player Controller")]
    public PlayerController playerController;
    public Button JumpButton;
    public Joystick joystick;
    public Button SettingButton;
[Header("Setting_Menu")]
public GameObject Setting_Menu;
public Button Lobby_Button;
public Button ClosePopuupButton;

public Scenemanager Scenemanager;
    
    private void Start()
    {
        if (playerController==null)
        {
             playerController = FindObjectOfType<PlayerController>();
            
        }
        
        Lobby_Button.onClick.AddListener(() =>
        {
            Scenemanager.LoadScene("#0_Title");
        });
        
        JumpButton.onClick.AddListener(() => playerController.Btn_jump());
        
        SettingButton.onClick.AddListener(() =>
        {
            Show(Setting_Menu);
        });
        
        ClosePopuupButton.onClick.AddListener(() =>
        {
            Hide(Setting_Menu);
        });
        
        init_Popup(Setting_Menu);
        
    }
    
    private void Update()
    {
        playerController.HandleMovement(joystick.Horizontal);
    }
    
    
    
    
}
