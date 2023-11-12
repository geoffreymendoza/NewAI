using UnityEngine;

namespace Finals {
[System.Serializable]
public class PatrolState : BaseState<AIController> {
    public float Offset;
    
    public override void EnterState(AIController ctx) {
        ctx.SetNavigation(ctx.PatrolDestination);
        ctx.ChangeAnimation(Data.WALK_ANIMATION);
    }

    public override void UpdateState(AIController ctx) {
        ctx.DetectEntity();
        if (Vector3.Distance(ctx.transform.position, ctx.PatrolDestination) <= 1f) {
            ctx.ChangeState(ctx.IdleState);
        }
    }
}
}