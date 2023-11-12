using UnityEngine;

namespace Finals {
[System.Serializable]
public class ChaseState: BaseState<AIController> {
    public override void EnterState(AIController ctx) {
        ctx.ChangeAnimation(Data.WALK_ANIMATION);
    }

    public override void UpdateState(AIController ctx) {
        ctx.SetNavigation(ctx.Target.transform.position);
        if (Vector3.Distance(ctx.transform.position, ctx.Target.transform.position) <= ctx.AttackRange) {
            ctx.SetNavigation(ctx.transform.position);
            ctx.ChangeState(ctx.AttackState);
        }
    }
}
}