using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Walk : StateMachineBehaviour
{
    Rigidbody2D rb;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TargetPlayerPosition(animator);

        if (DemonBoss.Instance.attackCountdown <= 0)
        {
            DemonBoss.Instance.AttackHandler();
            DemonBoss.Instance.attackCountdown = DemonBoss.Instance.attackTimer;
        }
    }
    void TargetPlayerPosition(Animator animator)
    {
        if(DemonBoss.Instance.Grounded())
        {
            DemonBoss.Instance.Flip();
            Vector2 _target = new Vector2(Playercontroller.Instance.transform.position.x, rb.position.y);
            Vector2 _newPos = Vector2.MoveTowards(rb.position, _target, DemonBoss.Instance.runSpeed * Time.fixedDeltaTime);
            DemonBoss.Instance.runSpeed = DemonBoss.Instance.speed;
            rb.MovePosition(_newPos);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, -25);
        }

        if(Vector2.Distance(Playercontroller.Instance.transform.position, rb.position) <= DemonBoss.Instance.attackRange)
        {
            animator.SetBool("Walk", false);
        }
        else
        {
            return;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Walk", false);
    }

}
