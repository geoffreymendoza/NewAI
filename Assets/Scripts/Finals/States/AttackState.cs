using UnityEngine;

namespace Finals {
[System.Serializable]
public class AttackState : BaseState<AIController> {
    public float PrepareBeforeAttackAnimTime;
    public float AttackAnimationDuration;
    public float AttackLogicDuration;
    public bool StartAttackLogic = false;
    public bool StartAttackAnimation = false;
    
    public override void EnterState(AIController ctx) {
        if (ctx.Target == null) {
            ctx.ChangeState(ctx.IdleState);
            return;
        }

        ctx.transform.LookAt(ctx.Target.transform);
        
        PrepareBeforeAttackAnimTime = ctx.BeforeAttackTime;
        AttackAnimationDuration = ctx.AttackDuration;
        AttackLogicDuration = ctx.AttackLogicTime;
        StartAttackLogic = false;
        StartAttackAnimation = false;
        if (PrepareBeforeAttackAnimTime > 0) {
            ctx.ChangeAnimation(Data.AIM_ANIMATION);
        }
    }

    public override void UpdateState(AIController ctx) {
        if (!StartAttackAnimation) {
            PrepareBeforeAttackAnimTime -= Time.deltaTime;
            if (PrepareBeforeAttackAnimTime <= 0) {
                ctx.ChangeAnimation(Data.ATTACK_ANIMATION);
                StartAttackAnimation = true;
            }
        }
        
        if (!StartAttackLogic) {
            AttackLogicDuration -= Time.deltaTime;
            if (AttackLogicDuration <= 0) {
                ctx.AttackLogic();
                StartAttackLogic = true;
            }
        }
        
        AttackAnimationDuration -= Time.deltaTime;
        if (AttackAnimationDuration <= 0) {
            ctx.ChangeState(ctx.IdleState);
        }
    }
}
}