using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy_Normal : EnemyBase
{
    protected override void BaseInit()
    {
        base.BaseInit();
       
        enemyRigidbody = GetComponent<Rigidbody2D>();
        ChangeEnemyExpression(EnemyExpression.idle);
        playerController = FindObjectOfType<PlayerController>();
    }

    protected override void Attack()
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
    
    protected override void FallowPlayer(float delay = 0.6f)
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
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fallowRange);
        
    }
    
   
}