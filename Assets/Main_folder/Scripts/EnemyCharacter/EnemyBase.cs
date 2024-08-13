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
    [Range(0, 10f)] public float fallowRange = 10;
    public float attackRange = 1;
    public float attackDamage = 10;
    public float attackCoolTime = 1;
    private float attackCoolTimeCounter = 0;
    public float moveRange = 10;
    public bool isDead = false;
    public bool isMove = true;

    protected Rigidbody2D enemyRigidbody;
    protected GameObject fallowTarget;

    private bool isAttacking = false;  // 공격 딜레이 플래그
    private bool isChangingState = false;  // 상태 변경 딜레이 플래그
    
    private int playerLayerMask;

    public PlayerController playerController;

    private void Start()
    {
        BaseInit();
    }

    private void LateUpdate()
    {
        ColliderDetect();
     //   AttackCoolTime();
    }

    public virtual void BaseInit()
    {
        playerLayerMask = LayerMask.GetMask("Player");
        enemyRigidbody = GetComponent<Rigidbody2D>();
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

    private IEnumerator DelayAction(float delay, Action action,bool isfront  = true)
    {
      
            yield return new WaitForSeconds(delay);
       
        action();
       
            yield return new WaitForSeconds(delay);
      
    }

    public void Damage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            enemyExpression = EnemyExpression.dead;
            ChangeEnemyExpression(enemyExpression);
        }
    }

    public void AttackCoolTime()
    {
        if (enemyExpression == EnemyExpression.attack)
        {
            attackCoolTimeCounter += Time.deltaTime;
            if (attackCoolTimeCounter >= attackCoolTime)
            {
                attackCoolTimeCounter = 0;
                ChangeEnemyExpression(EnemyExpression.idle);
            }
        }
    }

    public void ChangeEnemyExpression(EnemyExpression newExpression)
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
                Dead();
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

        Vector2 moveDirection = new Vector2(Random.Range(-1f, 1f), 0);
        Vector2 frontMove = new Vector2(enemyRigidbody.position.x + moveDirection.x * moveRange,
            enemyRigidbody.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontMove, Vector3.down, 1, LayerMask.GetMask("Ground"));

        if (rayHit.collider == null)
        {
            moveDirection.x *= -1;
        }

        float targetPositionX = transform.position.x + moveDirection.x * moveRange;
        transform.DOMoveX(targetPositionX, speed).OnComplete(RandomMove);
    }

    private void FallowPlayer(float delay = 0.6f)
    {
        if (!isMove)
        {
            return;
        }
        
        if (fallowTarget == null) return;

        transform.DOMoveX(fallowTarget.transform.position.x, speed * 0.6f).OnComplete(() =>
        {
            if (enemyExpression == EnemyExpression.fallow)
            {
                FallowPlayer(0.1f);
            }
        });
    }

    private void Attack()
    {
        Debug.Log("플레이 디텍트 "+attackDamage);

        if (isAttacking == false)
        {
            isAttacking = true;
            
            playerController.Damage(attackDamage, transform.position);
            
            
            StartCoroutine(DelayAction(2f, () =>
            {
                ChangeEnemyExpression(EnemyExpression.fallow);
              
                isAttacking = false;
            }));
        }
    }

    private void Dead()
    {
        isDead = true;
        Destroy(gameObject);
    }
}