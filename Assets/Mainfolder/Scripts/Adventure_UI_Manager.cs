using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Adventure_UI_Manager : MonoBehaviour
{
    public PlayerController playerController;
    
    public Button JumpButton;


    private void Start()
    {
        if (playerController==null)
        {
             playerController = FindObjectOfType<PlayerController>();
            
        }
        JumpButton.onClick.AddListener(() => playerController.Btn_jump());
    }
}
