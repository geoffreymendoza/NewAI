using UnityEngine;

namespace Finals {
[System.Serializable]
public class IdleState : BaseState<AIController> {
    public float ResetIdleDuration = 2f;
    public float IdleDuration = 2f;
    
    public override void EnterState(AIController ctx) {
        IdleDuration = ResetIdleDuration;
        ctx.ChangeAnimation(Data.IDLE_ANIMATION);
    }

    public override void UpdateState(AIController ctx) {
        ctx.DetectEntity();
        IdleDuration -= Time.deltaTime;
        if (IdleDuration <= 0) {
            ctx.ChangeState(ctx.PatrolState);
        }
    }
}
}