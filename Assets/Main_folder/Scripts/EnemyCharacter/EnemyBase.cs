using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBase : MonoBehaviour
{
    public enum EnemyExpression
    {
        idle,
        attack,
        fallow,
        dead,
        stop
    }
    

    public EnemyExpression enemyExpression = EnemyExpression.idle;

    public float hp = 100;
    public float speed = 5;
    [Range(0, 100f)] public float fallowRange = 10;
    [Range(0, 100f)]  public float attackRange = 1;
    public float attackDamage = 10;
    public float attackCoolTime = 1;
    public float moveRange = 10;
    public bool isDead = false;
    public bool isMove = false;

    public float distant = 1f;
    public LayerMask groundLayer; // Ground 레이어를 설정하세요
    protected Rigidbody2D enemyRigidbody;
    protected GameObject fallowTarget;

    protected bool isAttacking = false;  // 공격 딜레이 플래그
    private bool isChangingState = false;  // 상태 변경 딜레이 플래그
    
    private int playerLayerMask;

    public PlayerController playerController;
    Vector2 moveDirection;
    Vector2 frontMove = Vector2.zero;
    public delegate void ActionDelegate(GameObject obj);

    public ActionDelegate DeadDelegate;
    
    private void Awake()
    {
        BaseInit();
    }

    private void LateUpdate()
    {
        ColliderDetect();
     //   AttackCoolTime();
    }

    protected virtual void BaseInit()
    {
        DeadDelegate += Dead;
        playerLayerMask = LayerMask.GetMask("Player");
        groundLayer = LayerMask.GetMask("Ground");
        enemyRigidbody = GetComponent<Rigidbody2D>();
       
    }
    private void OnEnable()
    {
        StartCoroutine(
            DelayAction(2f, () =>
            {
                isMove = true;
                ChangeEnemyExpression(EnemyExpression.idle);
            }));
    }


    private void ColliderDetect()
    {
        Collider2D fallowCollider = Physics2D.OverlapCircle(transform.position, fallowRange, playerLayerMask);
        Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, attackRange, playerLayerMask);

        // 플레이어에 직접적으로 닿았을 때 대미지를 준다
        if (hitCollider != null && hitCollider.CompareTag("Player") )
        {
            if (playerController == null)
            {
                playerController = hitCollider.GetComponent<PlayerController>();
            }
            ChangeEnemyExpression(EnemyExpression.attack);
        }

        // 플레이어 감지 및 추적 상태 변경
        if (fallowCollider != null && enemyExpression != EnemyExpression.fallow && !isChangingState)
        {
            Debug.Log("플레이어 감지");
            transform.DOKill();
            fallowTarget = fallowCollider.gameObject;
            ChangeEnemyExpression(EnemyExpression.fallow);
        }
        else if (fallowCollider == null && enemyExpression == EnemyExpression.fallow &&
                 enemyExpression != EnemyExpression.idle && !isChangingState)
        {
            isChangingState = true;
            StartCoroutine(DelayAction(1f, () =>
            {
                ChangeEnemyExpression(EnemyExpression.idle);
                isChangingState = false;  // 상태 변경 완료 후 플래그 해제
            }));
        }
        
    }

    protected IEnumerator DelayAction(float delay, Action action,bool isfront  = true)
    {
      
            yield return new WaitForSeconds(delay);
       
        action();
       
            yield return new WaitForSeconds(delay);
      
    }

    public void Damage(float damage, Vector2 direction, float KnockBackforce = 2)
    {
        hp -= damage;
        transform.DOKill();
        isMove = false;
        direction.x= transform.position.x - direction.x;
        
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

        KnockBack(direction, KnockBackforce);

        if (hp <= 0)
        {
            enemyExpression = EnemyExpression.dead;
            ChangeEnemyExpression(enemyExpression);
        }
    }

    public void KnockBack(Vector2 direction, float force)
    { 
        Blink();
    
        // 넉백 위치 계산: 현재 위치에서 방향에 따라 넉백
        Vector3 knockbackTarget = transform.position + (Vector3)(direction.normalized * force);
    
        // DOTween을 사용하여 부드럽게 넉백 효과 적용
        transform.DOMove(knockbackTarget, 0.2f).SetEase(Ease.OutQuad).OnComplete((() =>
        {
            ChangeEnemyExpression(EnemyExpression.fallow);
        } )).SetLink(gameObject); // 메모리 누수 방지
    }

    public void Blink()
    {
        // 스프라이트 렌더러를 가져옴
        SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();

        // 0.1초 동안 알파 값을 0으로 만들어 사라지게 하고, 다시 1로 만들어 나타나게 함
        spriteRenderer.DOFade(0, 0.1f).SetLoops(6, LoopType.Yoyo).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            Debug.Log("Blink complete");
            spriteRenderer.DOFade(1, 0.1f).OnComplete(() => 
            {
             
                ChangeEnemyExpression(EnemyExpression.fallow);
            });
            isMove = true;
        }).SetLink(gameObject); // 메모리 누수 방지
    }
    

    protected void ChangeEnemyExpression(EnemyExpression newExpression)
    {
        enemyExpression = newExpression;
        switch (newExpression)
        {
            case EnemyExpression.idle:
                RandomMove();
                break;
            case EnemyExpression.attack:
                Attack();
                Debug.Log("ATTACK"+enemyExpression);
                break;
            case EnemyExpression.fallow:
                FallowPlayer();
                break;
            case EnemyExpression.dead:
                DeadDelegate(this.gameObject);
                Debug.Log("dead: "+enemyExpression);
                break;
            case EnemyExpression.stop:
                moveStop();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void moveStop()
    {
        isMove = false;
    }

    private void RandomMove()
    {
        if (!isMove)
        {
            return;
        }

        moveDirection = new Vector2(Random.Range(-1f, 1f), 0);
        frontMove = new Vector2(enemyRigidbody.position.x + moveDirection.x * moveRange, enemyRigidbody.position.y);

        // 땅 감지 Raycast
        RaycastHit2D rayHitDown = Physics2D.Raycast(frontMove, Vector2.down, distant, groundLayer);

        if (rayHitDown.collider == null)
        {
            // 땅이 없으면 방향을 반대로 변경
            moveDirection.x *= -1;
        }
        else
        {
            // 벽 감지 Raycast
            RaycastHit2D rayHitSide = Physics2D.Raycast(frontMove, moveDirection.x < 0 ? Vector2.left : Vector2.right, distant, groundLayer);
            if (rayHitSide.collider != null)
            {
                // 벽이 감지되면 방향을 반대로 변경
                moveDirection.x *= -1;
            }
        }

        // 이동 중 충돌 감지를 위한 OnUpdate 사용
        float targetPositionX = transform.position.x + moveDirection.x * moveRange;
        transform.DOMoveX(targetPositionX, speed).OnUpdate(() =>
        {
            // 이동 중 벽 충돌 감지
            RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection.x < 0 ? Vector2.left : Vector2.right, distant, groundLayer);
            if (hit.collider != null)
            {  
                moveDirection.x *= -1;
                transform.DOKill();  // 이동 중지
                ChangeEnemyExpression(EnemyExpression.idle);
            }
        }).OnComplete(() => {
            transform.DOKill(); 
            ChangeEnemyExpression(EnemyExpression.idle);
        });
    }

    protected virtual void FallowPlayer(float delay = 0.6f)
    {
     
    }

    protected virtual void Attack()
    {
      
    }

    private void Dead(GameObject obj)
    {
        isDead = true;
    }
}