using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    public float backForce = 20;
    bool iscontroll = true;
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
        if (iscontroll)
        {
        playerSprite.velocity = new Vector2(horizontal * speed, playerSprite.velocity.y);
            
        }
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

    public void Damage(float damage, Vector2 direction)
    {
        // 피해를 입었을 때 체력 감소
        Hp -= damage;

        // 방향 설정: 적이 오른쪽에 있으면 왼쪽 위로, 적이 왼쪽에 있으면 오른쪽 위로 넉백
        
        direction.x= playerSprite.transform.position.x - direction.x;
        
        if (direction.x > 0)
        {
            direction = new Vector2(1, 1);  // 왼쪽 위로 넉백
            Debug.Log("왼쪽");
        }
        else
        {
            
            direction = new Vector2(-1, 1);  // 오른쪽 위로 넉백
            Debug.Log("오른쪽");
        }

        // 넉백 적용
        KnockBack(direction);
    }

    public void KnockBack(Vector2 direction)
    {
        iscontroll = false;
        // 기존 움직임을 초기화
        playerSprite.DOKill();  // 기존 DOTween 애니메이션을 모두 중지
        playerSprite.velocity = Vector2.zero;  // 기존 물리적인 속도 초기화

        // 넉백 위치 계산: 현재 위치에서 방향에 따라 넉백
        Vector3 knockbackTarget = playerSprite.transform.position + (Vector3)(direction.normalized * backForce);

        // DOTween을 사용하여 부드럽게 넉백 효과 적용
        playerSprite.transform.DOMove(knockbackTarget, 0.2f).SetEase(Ease.OutQuad).onComplete += () =>
        {
            iscontroll = true;
        };
    }
 


    private void FollowCamera()
    {
        // 카메라가 이동할 목표 위치 설정
        Vector3 targetPosition = new Vector3(playerSprite.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);

        // DOTween을 사용하여 부드럽게 이동
        mainCamera.transform.DOMove(targetPosition, 0.2f).SetEase(Ease.InOutSine);
    }
}