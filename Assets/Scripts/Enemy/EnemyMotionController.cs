using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyResidentEvil {

    public abstract class EnemyMotionController : HumanoidBaseMotionController {

        protected Animator animator;

        protected Rigidbody rigidBody;

        protected NavMeshAgent navMeshAgent;

        public virtual bool AgonizeTrigger() {
            switch (currentMotionState) {
                case MotionState.Idling:
                case MotionState.Moving:
                case MotionState.Attacking:
                case MotionState.UnderAttacking:
                case MotionState.Agonizing:
                    currentMotionState = MotionState.Agonizing;
                    animator.SetBool("Attack", false);
                    animator.SetBool("Move", false);
                    animator.SetBool("ReactionHit", false);
                    navMeshAgent.ResetPath();
                    return true;
            }
            return false;
        }

        protected void Agonize() {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("BaseLayer.Agonize") && state.normalizedTime > 0.9f) {
                animator.SetBool("Agonize", false);
                currentMotionState = MotionState.Idling;
                return;
            }
            animator.SetBool("Agonize", true);
        }

        public override bool WasHitFlyTrigger(Vector3 position, float distance) {
            switch (currentMotionState) {
                case MotionState.Idling:
                case MotionState.Moving:
                case MotionState.Attacking:
                case MotionState.UnderAttacking:
                case MotionState.Agonizing:
                case MotionState.Shouting:
                    base.WasHitFlyTrigger(position, distance);
                    animator.SetBool("Move", false);
                    animator.SetBool("Attack", false);
                    animator.SetBool("Agonize", false);
                    animator.SetBool("ReactionHit", false);
                    navMeshAgent.ResetPath();
                    return true;
            }
            return false;
        }

        protected void WasHitFly() {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("BaseLayer.StandUp")) {
                if (state.normalizedTime > 0.9f) {
                    currentMotionState = MotionState.Idling;
                    animator.SetBool("HitFly", false);
                }
            }
            if (currentMotionState == MotionState.HitFly) {
                animator.SetBool("HitFly", true);
                currentMotionState = MotionState.StandingUp;
                transform.LookAt(wasHitFlyInfo.Position);
                Vector3 direction = transform.position - wasHitFlyInfo.Position;
                direction.Normalize();
                rigidBody.AddForce(wasHitFlyInfo.Distance * direction, ForceMode.VelocityChange);
            }

        }

    }

}

