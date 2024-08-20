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
    public float AttackDamage = 1;
    public float backForce = 20;
    bool IsAttack = false;
    bool Ishit = false;
    
    public delegate void PlayerAction(int skillIndex, float delay);
    public event PlayerAction OnPlayerAttack;
    
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
        EnemyDetect();
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
        Blink();
        // 넉백 위치 계산: 현재 위치에서 방향에 따라 넉백
        Vector3 knockbackTarget = playerSprite.transform.position + (Vector3)(direction.normalized * backForce);

        // DOTween을 사용하여 부드럽게 넉백 효과 적용
        playerSprite.transform.DOMove(knockbackTarget, 0.2f).SetEase(Ease.OutQuad).onComplete += () =>
        {
            iscontroll = true;
        };
    }
 
    public void Blink()
    {
        // DOTween을 사용하여 깜빡임 효과 적용

        // 스프라이트 렌더러를 가져옴
        SpriteRenderer spriteRenderer = playerSprite.GetComponent<SpriteRenderer>();

        // 0.1초 동안 알파 값을 0으로 만들어 사라지게 하고, 다시 1로 만들어 나타나게 함
        spriteRenderer.DOFade(0, 0.1f).SetLoops(6, LoopType.Yoyo).SetEase(Ease.InOutQuad).onComplete += () =>
        {
            spriteRenderer.DOKill();
            Debug.Log("Blink complete");
            spriteRenderer.DOFade(1, 0.1f).OnComplete(() => 
            {
                Debug.Log("Blink complete");
            });
        };

    
    }


    private void FollowCamera()
    {
        // 카메라가 이동할 목표 위치 설정
        Vector3 targetPosition = new Vector3(playerSprite.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);

        // DOTween을 사용하여 부드럽게 이동
        mainCamera.transform.DOMove(targetPosition, 0.2f).SetEase(Ease.InOutSine);
    }

    private void EnemyDetect(float delay = 5f)
    {
        // 이미 공격 중이라면 메서드를 종료
        if (IsAttack)
        { 
            return;
        }
            // 플레이어의 위치에서 AttackRange 반경 내에 있는 Collider2D 객체를 모두 가져옴
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(playerSprite.transform.position, AttackRange, LayerMask.GetMask("Enemy"));

            if (hitEnemies.Length > 0)
            {
                IsAttack = true;
                Debug.Log("Attack");

                // 모든 적에 대해 데미지와 넉백 범위 설정
                foreach (Collider2D enemy in hitEnemies)
                {
                    EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
                    if (enemyScript != null)
                    {
                        // 무기에 따라 데미지와 넉백 범위 설정
                        enemyScript.Damage(AttackDamage, playerSprite.transform.position, 5);
                    }
                }
                OnPlayerAttack?.Invoke(0, delay);
                // 쿨타임이 끝난 후 공격 가능
                StartCoroutine(DelayAction(delay, () =>
                {
                    IsAttack = false;
                }));
            }
            if (Ishit)
            { 
                return;
            }
            Collider2D[] hitBullet = Physics2D.OverlapCircleAll(playerSprite.transform.position, 1, LayerMask.GetMask("Bullet"));
            if (hitBullet.Length > 0)
            {
                Ishit = true;
                Debug.Log("Attack");

                // 모든 적에 대해 데미지와 넉백 범위 설정
                foreach (Collider2D Bullet in hitBullet)
                {
                    Destroy(Bullet.gameObject);
                    Damage(2, Bullet.transform.position);
                }
                // 쿨타임이 끝난 후 공격 가능
                StartCoroutine(DelayAction(0.5f, () =>
                {
                    Ishit = false;
                }));
            }

    }

    private IEnumerator DelayAction(float delay, Action action, bool isFront = true)
    {
        if (isFront)
        {
            yield return new WaitForSeconds(delay);
        }

        action();

        // isFront가 false일 경우 추가 딜레이
        if (!isFront)
        {
            yield return new WaitForSeconds(delay);
        }
    }

// 공격 범위를 시각화하기 위한 Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerCollider.transform.position, AttackRange);
    }
    
    
}