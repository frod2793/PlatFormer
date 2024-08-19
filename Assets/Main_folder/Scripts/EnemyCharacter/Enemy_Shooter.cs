using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy_Shooter : EnemyBase
{
    [Header("총알 프리펩")]
    [SerializeField] private GameObject bulletPrefab;
    float bulletSpeed = 5f;
    [Range(0, 100f)] [SerializeField] float stopDistance = 10f;
    
    protected override void BaseInit()
    {
        base.BaseInit();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        ChangeEnemyExpression(EnemyExpression.idle);
        playerController = FindObjectOfType<PlayerController>();
    }

    protected override void Attack()
    {
        transform.DOKill(); // 이동 중지

        // 플레이어 방향으로 총알 발사
        Vector3 direction = (playerController.transform.position - transform.position).normalized; // 플레이어 방향 계산

        // 총알 생성
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // 총알의 Rigidbody2D 가져오기
        Rigidbody2D bulletRigidbody = bullet.GetComponent<Rigidbody2D>();

        // 총알에 속도 부여
        bulletRigidbody.velocity = direction * bulletSpeed;

        // 총알과 적 자신 간의 충돌 무시
        Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
        Collider2D enemyCollider = GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(bulletCollider, enemyCollider);
    }
    
    protected override void FallowPlayer(float delay = 0.6f)
    {
        if (!isMove || fallowTarget == null)
        {
            return;
        }

        // 플레이어와의 거리 계산
        float distanceToPlayer = Mathf.Abs(fallowTarget.transform.position.x - transform.position.x);

        // 목표 위치 계산: 플레이어의 일정 거리 앞에서 멈추도록 설정
        float targetPositionX;

        if (fallowTarget.transform.position.x > transform.position.x)
        {
            // 플레이어가 적의 오른쪽에 있을 때
            targetPositionX = fallowTarget.transform.position.x - stopDistance;
        }
        else
        {
            // 플레이어가 적의 왼쪽에 있을 때
            targetPositionX = fallowTarget.transform.position.x + stopDistance;
        }

        // 목표 위치와 현재 위치 사이의 거리 계산
        distanceToPlayer = Mathf.Abs(targetPositionX - transform.position.x);

        // 플레이어와의 거리가 stopDistance보다 크면 이동
        if (distanceToPlayer > 0.1f) // 거리 오차를 줄이기 위해 약간의 허용 범위를 둡니다.
        {
            // 이미 이동 중이면 추가 이동 방지
            if (DOTween.IsTweening(transform))
            {
                return;
            }

            // 플레이어를 따라 이동
            transform.DOMoveX(targetPositionX, speed * delay).OnComplete(() =>
            {
                // 이동 후 다시 플레이어와의 거리 계산
                distanceToPlayer = Mathf.Abs(fallowTarget.transform.position.x - transform.position.x);
                if (distanceToPlayer > stopDistance && enemyExpression == EnemyExpression.fallow)
                {
                    // 짧은 딜레이 후 다시 플레이어를 따라 이동
                    FallowPlayer(0.1f);
                }
            });
        }
    }




    
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fallowRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
        
    }
    
}
