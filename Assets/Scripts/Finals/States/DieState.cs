using UnityEngine;

namespace Finals {
[System.Serializable]
public class DieState : BaseState<AIController> {
    public float Duration = 0.3f; 
    public override void EnterState(AIController ctx) {
        ctx.ChangeAnimation(Data.DIE_ANIMATION);
        // TODO disable 
    }

    public override void UpdateState(AIController ctx) {
        Duration -= Time.deltaTime;
        if (Duration <= 0) {
            ctx.Died();
        }
    }
}
}