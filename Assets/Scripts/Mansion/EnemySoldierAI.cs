using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoldierAI : MonoBehaviour
{
    public Transform player, attackPoint;
    public LayerMask playerLayers;
    public float speed = 2f;
    public float attackRange = 1f;
    public int attackDamage = 10;
    public Animator animator;
    public bool flip = false;

    private bool isAttacking = false;

    void Update(){
        float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);
        if(distanceToPlayer < attackRange){
            if(!isAttacking){
                animator.SetBool("isWalking", false);
                StartCoroutine("Attack");
            }
        } else if(!isAttacking){
            FollowPlayer();
        }
    }

    void FollowPlayer(){
        animator.SetBool("isWalking", true);
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        Vector3 scale = transform.localScale;
        if(player.position.x > transform.position.x){
            scale.x = Mathf.Abs(scale.x) * -1 * (flip ? -1 : 1);
        } else{
            scale.x = Mathf.Abs(scale.x) * (flip? -1 : 1);
        }

        transform.localScale = scale;
    }

    IEnumerator Attack(){
        isAttacking = true;
        yield return new WaitForSeconds(0.5f);
        if(this.enabled == false){
            yield break;
        }
        animator.SetTrigger("attack");
        yield return new WaitForSeconds(0.4f);
        Collider2D[] hitplayer = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayers);
        foreach(Collider2D player in hitplayer){
            player.GetComponent<PlayerHealth_Mansion>().TakeDamage(attackDamage);
        }
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    private void OnDrawGizmosSelected() {
        if(attackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}