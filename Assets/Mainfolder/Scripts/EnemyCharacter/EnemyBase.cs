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
        dead
    }

    public EnemyExpression enemyExpression = EnemyExpression.idle;

    public float hp = 100;
    public float speed = 5;
    [Range(0,10f)]
    public float attackRange = 10;
    public float attackDamage = 1;
    public float attackCoolTime = 1;
    private float attackCoolTimeCounter = 0;
    public float moveRange = 10;
    public bool isDead = false;
    public bool isMove = true;
    
    protected Rigidbody2D enemyRigidbody;
    protected GameObject fallowTarget;

    private void Start()
    {
        BaseInit();
    }

    private void Update()
    {
        ColliderDetect();
        AttackCoolTime();
    }

    public virtual void BaseInit()
    {
    }

    private void ColliderDetect()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player"));

        if (collider != null && enemyExpression != EnemyExpression.fallow)
        {
            Debug.Log("플레이어 감지");
            transform.DOKill();
            fallowTarget = collider.gameObject;
            enemyExpression = EnemyExpression.fallow;
            ChangeEnemyExpression(enemyExpression);
        }
        else if (collider == null && enemyExpression == EnemyExpression.fallow&&enemyExpression!=EnemyExpression.idle)
        {
            enemyExpression = EnemyExpression.idle;
            StartCoroutine(co_DeleayAction(1, () =>
            {
                ChangeEnemyExpression(enemyExpression);
            }));
        }
    }

  private IEnumerator co_DeleayAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
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
        switch (newExpression)
        {
            case EnemyExpression.idle:
                RandomMove();
                break;
            case EnemyExpression.attack:
                Attack();
                break;
            case EnemyExpression.fallow:
                FallowPlayer();
                break;
            case EnemyExpression.dead:
                Dead();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void RandomMove()
    {
        if (!isMove)
        {
            return;
        }
        Vector2 moveDirection = new Vector2(Random.Range(-1f, 1f), 0);
        Vector2 frontMove = new Vector2(enemyRigidbody.position.x + moveDirection.x * moveRange, enemyRigidbody.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontMove, Vector3.down, 1, LayerMask.GetMask("Ground"));

        if (rayHit.collider == null)
        {
            moveDirection.x *= -1;
        }

        float targetPositionX = transform.position.x + moveDirection.x * moveRange;
        transform.DOMoveX(targetPositionX, speed).OnComplete(RandomMove);
    }

    private void FallowPlayer()
    {
        if (fallowTarget == null) return;

        Vector2 playerPosition = fallowTarget.transform.position;
        transform.DOMove(playerPosition, speed*0.6f).OnComplete(() =>
        {
            enemyExpression = EnemyExpression.attack;
            ChangeEnemyExpression(enemyExpression);
        });
    }

    private void Attack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerController>().Damage(attackDamage);
            }
        }
    }

    private void Dead()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
