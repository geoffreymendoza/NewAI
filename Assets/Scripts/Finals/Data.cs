using UnityEngine;

namespace Finals {
public static class Data {
    public static readonly int IDLE_ANIMATION = Animator.StringToHash("Idle");
    public static readonly int WALK_ANIMATION = Animator.StringToHash("Walk");
    public static readonly int ATTACK_ANIMATION = Animator.StringToHash("Attack");
    public static readonly int DIE_ANIMATION = Animator.StringToHash("Death");
    public static readonly int AIM_ANIMATION = Animator.StringToHash("Aim");
}
}