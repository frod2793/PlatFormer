using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera mainCamera;
    public Collider2D playerCollider;
    private Rigidbody2D playerSprite;

    [Range(0, 500)] public float speed = 0f;

    [Range(0, 500)] public float jumpForce = 0;

    private bool isJumping = false;
    private const float Gravity = 1.7f;

    private void Start()
    {
        playerSprite = playerCollider.GetComponent<Rigidbody2D>();
        playerSprite.gravityScale = Gravity;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
        FollowCamera();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        playerSprite.velocity = new Vector2(horizontal * speed, playerSprite.velocity.y);
    }

    public void Btn_jump()
    {
        if (IsGrounded())
        {
            Debug.Log("Jump");
            isJumping = true;
            playerSprite.velocity = new Vector2(playerSprite.velocity.x, jumpForce);
        }
    }

    public void HandleJump()
    {
        // 점프 입력 처리
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            Debug.Log("Jump");
            isJumping = true;
            playerSprite.velocity = new Vector2(playerSprite.velocity.x, jumpForce);
        }

        // 점프 상태 업데이트
        if (IsGrounded())
        {
            Debug.Log("Grounded");
            isJumping = false;
        }
    }

    private bool IsGrounded()
    {
        // 바닥에 닿아 있는지 확인
        return playerCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) ;
    }

    private void FollowCamera()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        cameraPosition.x = playerSprite.transform.position.x;
        mainCamera.transform.position = cameraPosition;
    }
}