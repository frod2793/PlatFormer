using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;
    public Collider2D playerCollider;
    private Rigidbody2D playerSprite;

    [Range(0, 500)] public float speed = 0f;

    [Range(0, 500)] public float jumpForce = 0;

    private const float Gravity = 1.7f;
    
    public float Hp = 100;
    public float AttackRange = 10;
    public float AttackDamage = 10;
public float backForce = 10;
    
    private void Start()
    {
        playerSprite = playerCollider.GetComponent<Rigidbody2D>();
        playerSprite.gravityScale = Gravity;
    }

    private void FixedUpdate()
    {
       // HandleMovement();
        HandleJump();
        FollowCamera();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Btn_jump();
        }
        
        
        float horizontal = Input.GetAxis("Horizontal");
        HandleMovement(horizontal);
        
    }
    public void HandleMovement( float horizontal)
    {
        playerSprite.velocity = new Vector2(horizontal * speed, playerSprite.velocity.y);
    }

    public void Btn_jump()
    {
        if (IsGrounded())
        {
            Debug.Log("Jump");
            playerSprite.velocity = new Vector2(playerSprite.velocity.x, jumpForce);
        }
    }

    public void HandleJump()
    {

        // 점프 상태 업데이트
        if (IsGrounded())
        {
       //     Debug.Log("Grounded");
        }
    }

    private bool IsGrounded()
    {
        // 바닥에 닿아 있는지 확인
        return playerCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) ;
    }

    public void Damage(float damage, Transform enemyTransform)
    {
        // 피해를 입었을 때 처리
        Hp -= damage;

        
        float distance = playerSprite.transform.position.x - enemyTransform.position.x;

        if (distance>0)
        {
            backForce = -backForce;
        }
      
        
        Debug.Log("backForce: " + backForce);
        Vector2 Direction = (playerSprite.transform.position - enemyTransform.position).normalized;
        playerSprite.AddForce(Direction * backForce, ForceMode2D.Impulse);
        
StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(0.5f);
        playerSprite.velocity = Vector2.zero;
        
        
    }


    private void FollowCamera()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        cameraPosition.x = playerSprite.transform.position.x;
        mainCamera.transform.position = cameraPosition;
    }
}