using System;
using System.Collections.Generic;
using Finals;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public TextMeshProUGUI NotificationTmp;
    public StatsData[ ] StatsSrc;
    private List<AIController> _entities = new List<AIController>();

    public float mageHealth, archerHealth, warriorHealth, ninjaHealth;
    public float mageDamage, archerDamage, warriorDamage, ninjaDamage;
    public float warriorRange = 1f;
    public float archerRange;
    public float mageRange;
    public float ninjaRange;

    private void Awake() {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
        _entities = new List<AIController>();
    }

    public StatsData AssignValue(Finals.AI_Types type) {
        StatsData result = new StatsData();
        foreach (var sd in StatsSrc) {
            if (sd.Type == type) {
                result = sd;
                break;
            }
        }

        return result;
    }

    public void Register(AIController ai) {
        if (_entities.Contains(ai))
            return;
        _entities.Add(ai);
        ai.OnKilled += OnRemoveEntity;
    }

    private void OnRemoveEntity(int id, Team team) {
        List<Team> teamA = new List<Team>();
        List<Team> teamB = new List<Team>();
        
        foreach (var ai in _entities) {
            if (ai.ID == id) {
                ai.OnKilled -= OnRemoveEntity;
                //_entities.Remove(ai);
                continue;
            }
            if(!ai.IsAlive) continue;
            if(ai.Team == Team.TeamA)
                teamA.Add(ai.Team);
            else
                teamB.Add(ai.Team);
        }

        if (teamA.Count > 0 && teamB.Count <= 0) {
            Debug.Log("Team B won");
            NotificationTmp.text = "Red Won!";
        }
        else if (teamB.Count > 0 && teamA.Count <= 0) {
            Debug.Log("Team A won");
            NotificationTmp.text = "Blue Won!";
        }
    }
    
}

[System.Serializable]
public class StatsData {
    public Finals.AI_Types Type;
    [Header("Stats")]
    public float HP;
    public float Damage;
    public float AttackRange;
    public float PrepareToAttackTime;
    public float AttackLogicTime;
    public float DetectRange;
    [Header("Reference")]
    public Material RedTeamMat;
    public Material BlueTeamMat;
}