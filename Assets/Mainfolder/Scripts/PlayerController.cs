using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    
    public Camera mainCamera;
    public Collider2D playerCollider;
    public Rigidbody2D playerSprite;
    public float speed = 100.0f;
    public float jumpForce = 100f;
    private bool isjumping = false;

    
    private void cameraFallowplayer()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        cameraPosition.x = playerSprite.transform.position.x;
        mainCamera.transform.position = cameraPosition;
    }
    
    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0.0f, 0.0f);
        
        playerSprite.transform.DOMove( playerSprite.transform.position + movement * speed * Time.deltaTime, 0.5f);
        

        if (Input.GetButtonDown("Jump") && !isjumping)
        {
            Debug.Log("Jump");
            playerSprite.DOJump( playerSprite.position + Vector2.up, jumpForce, 1, 0.5f);
            isjumping = true;  
        }

        if (isjumping&&playerCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            Debug.Log("Grounded");
            isjumping = false;  
        }
       
        
        
        cameraFallowplayer();
    }
}
