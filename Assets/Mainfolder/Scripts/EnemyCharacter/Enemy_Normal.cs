using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Normal : EnemyBase
{
    private SpriteRenderer enemySpriteRenderer;
    private Collider2D enemyCollider;

    public override void BaseInit()
    {
        base.BaseInit();
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        ChangeEnemyExpression(EnemyExpression.idle);
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}