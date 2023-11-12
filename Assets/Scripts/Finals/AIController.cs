using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Finals {
public enum Team {
    TeamA = 0,
    TeamB = 1,
}

public enum AI_Types {
    Archer = 0,
    Warrior,
    Mage,
    Ninja
}

public class AIController : MonoBehaviour, IHealthHandler {
    public event Action<int, Team> OnKilled; 
    
    [Header("Reference")]
    public AI_Types Type;
    public Team Team;
    public LayerMask EntityMask;
    public NavMeshAgent Agent;
    public List<GameObject> AIModels;
    [HideInInspector]
    public Animator Anim;
    private Collider _col;
    public int ID => this.transform.GetInstanceID();

    private Dictionary<AI_Types, int> _aiTypes = new Dictionary<AI_Types, int>() {
        { AI_Types.Archer, 0 },
        { AI_Types.Warrior, 1 },
        { AI_Types.Mage, 2 },
        { AI_Types.Ninja, 3 },
    };

    [Header("Patrol")]
    public Vector3 PatrolDestination;
    public float Offset;

    [Header("Attack")]
    public float DetectRange = 0;
    public float AttackRange = 0f;
    public float BeforeAttackTime = 0f;
    
    public AIController Target;

    [Header("Duration")]
    public float AttackDuration;
    public float AttackLogicTime;

    [Header("Melee Attack")]
    public float UpOffset = 1f;
    public float ForwardOffset = 1.5f;
    public float MeleeAttackRadius = 0.4f;
    
    [Header("Range Projectiles")]
    public Transform FirePtTr;
    public Projectile ProjectilePr;

    [Header("States")]
    public IdleState IdleState = new IdleState();
    public PatrolState PatrolState = new PatrolState();
    public ChaseState ChaseState = new ChaseState();
    public AttackState AttackState = new AttackState();
    public DieState DieState = new DieState();
    private BaseState<AIController> _currentState;

    private void Start() {
        Init();
        ChangeState(IdleState);
    }

    private void Init() {
        _col = GetComponent<Collider>();

        foreach (var model in AIModels) {
            model.SetActive(false);
        }
        
        Action<AI_Types> activateAiModel = (AI_Types aiType) => {
            AIModels[_aiTypes[aiType]].SetActive(true);
            Anim = AIModels[_aiTypes[aiType]].GetComponent<Animator>();
        };
        activateAiModel(Type);

        PatrolDestination = this.transform.forward * Offset;

        StatsData sd = GameManager.Instance.AssignValue(Type);
        HP = sd.HP;
        DetectRange = sd.DetectRange;
        AttackRange = sd.AttackRange;
        AttackLogicTime = sd.AttackLogicTime;
        Damage = sd.Damage;
        BeforeAttackTime = sd.PrepareToAttackTime;
        AttackDuration += BeforeAttackTime;

        UpdateAnimClip();
        
        GameManager.Instance.Register(this);
    }
    
    private void UpdateAnimClip() {
        AnimationClip[ ] clips = Anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "Attack":
                    AttackDuration += clip.length;
                    break;
            }
        }
    }

    private void Update() {
        _currentState.UpdateState(this);
    }

    public void DetectEntity() {
        if (Target != null && Target.IsAlive) {
            ChangeState(ChaseState);
            return;
        }
        
        var cols = Physics.OverlapSphere(this.transform.position, DetectRange, EntityMask);
        if (cols.Length > 0) {
            List<AIController> possibleThreats = new List<AIController>(); 
            
            foreach (var col in cols) {
                if (col.transform == this.transform)
                    continue;
                var aiCtrl = col.transform.GetComponent<AIController>();
                if(Team == aiCtrl.Team)
                    continue;
                if (aiCtrl.IsAlive) {
                    possibleThreats.Add(aiCtrl);
                }
            }

            if (possibleThreats.Count > 0) {
                int randIdx = Random.Range(0, possibleThreats.Count);
                Target = possibleThreats[randIdx];
                ChangeState(ChaseState);
            }
        }
    }

    public void ChangeAnimation(int state) {
        Anim.CrossFade(state, 0, 0);
    }

    public void ChangeState(BaseState<AIController> state) {
        _currentState = state;
        _currentState.EnterState(this);
    }

    public void SetNavigation(Vector3 destination) {
        Agent.SetDestination(destination);
    }
    
    public void AttackLogic() {
        switch (Type) {
            case AI_Types.Archer: // Range
            case AI_Types.Mage:
                Projectile projectile = Instantiate(ProjectilePr, FirePtTr.position, this.transform.rotation);
                projectile.Init(Target, Damage);
                break;
            case AI_Types.Warrior: // Melee
            case AI_Types.Ninja:
                Vector3 hitPos = this.transform.position + transform.up * UpOffset + transform.forward * ForwardOffset;
                var hitCols = Physics.OverlapSphere(hitPos, 0.4f, EntityMask);
                if (hitCols.Length > 0) {
                    foreach (var col in hitCols) {
                        var healthHandler = col.GetComponent<IHealthHandler>();
                        if(healthHandler.TeamType != TeamType)
                            healthHandler.ApplyDamage(Damage);
                    }
                }
                break;
        }
    }

    #region IHealthHandler

    public bool IsAlive => HP > 0;
    public Team TeamType => Team;
    public float HP { get; private set; }
    public float Damage { get; private set; }

    public void ApplyDamage(float damage) {
        if (HP <= 0) return;
        HP -= damage;
        if (HP <= 0) {
            OnKilled?.Invoke(ID, Team);
            ChangeState(DieState);
        }
    }

    public void Died() {
        _col.enabled = false;
        Agent.enabled = false;
        this.enabled = false;
    }

    #endregion

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(this.transform.position, transform.forward * PatrolState.Offset);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, DetectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, AttackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position + transform.up * UpOffset + transform.forward * ForwardOffset, MeleeAttackRadius);
    }
}
}