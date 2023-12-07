using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon {
    [SerializeField] PokemonBase _PkmBase;
    [SerializeField] int _level;

    public PokemonBase PkmBase {
        get {
            return _PkmBase;
        }
    }
    public int Level {
        get {
            return _level;
        }
    }

    public List<Skill> Skills { get; set; }
    public int HP { get; set; }

    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }

    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }
    public bool HpChanged { get; set; }

    public void Init() {
        // generate skill
        Skills = new List<Skill>();
        foreach (var act in PkmBase.LearnableSkills) {
            if (act.Level <= Level) Skills.Add(new Skill(act.Base));
            if(Skills.Count >= 4) break;

            CalculateStats();
            HP = MaxHp;
            ResetStatBoosts();
        }
    }

    void CalculateStats() {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.ATTACK, Mathf.FloorToInt((PkmBase.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.DEFENSE, Mathf.FloorToInt((PkmBase.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SUPER_ATTACK, Mathf.FloorToInt((PkmBase.SuperAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SUPER_DEFENSE, Mathf.FloorToInt((PkmBase.SuperDefense * Level) / 100f) + 5);
        Stats.Add(Stat.SPEED, Mathf.FloorToInt((PkmBase.Speed * Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((PkmBase.MaxHp * Level) / 100f) + 10 + Level;
    }

    void ResetStatBoosts() {
        StatBoosts = new Dictionary<Stat, int>() {
            {Stat.ATTACK, 0},
            {Stat.DEFENSE, 0},
            {Stat.SUPER_ATTACK, 0},
            {Stat.SUPER_DEFENSE, 0},
            {Stat.SPEED, 0}
        };
    }

    int GetStat(Stat stat) {
        int value =  Stats[stat];

        //Apply boost
        int boost = StatBoosts[stat];
        var boostVal = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };
        if(boost >= 0) {
            value = Mathf.FloorToInt(value * boostVal[boost]);
        } else {
            value = Mathf.FloorToInt(value / boostVal[-boost]);
        }
        return value;
    }

    public void ApplyBoosts(List<StatBoost> statBoosts) {
        foreach (var statBoost in statBoosts) {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            if(boost > 0) {
                StatusChanges.Enqueue($"{PkmBase.Name}'s {stat} raise !!!");
            } else {
                StatusChanges.Enqueue($"{PkmBase.Name}'s {stat} fell !!!");
            }
            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);
        }
    }

    public int Attack {
        get { return GetStat(Stat.ATTACK); }
    }

    public int Defense {
        get { return GetStat(Stat.DEFENSE); }
    }

    public int SuperAttack {
        get { return GetStat(Stat.SUPER_ATTACK); }
    }

    public int SuperDefense {
        get { return GetStat(Stat.SUPER_DEFENSE); }
    }

    public int Speed {
        get { return GetStat(Stat.SPEED); }
    }

    public int MaxHp { get; private set; }

    // bảng công thức tính dame https://pokeviet.blogspot.com/p/blog-page.html
    public DamageDetails TakeDamage(Skill skill, Pokemon attacker) {
        float critical = 1f;
        if(Random.value * 100f <= 6.25f)
            critical = 2f;

        float effectiveness = TypeChart.GetEffectiveness(skill.SkillBase.Type, this.PkmBase.Type_1) * TypeChart.GetEffectiveness(skill.SkillBase.Type, this.PkmBase.Type_2);

        var damageDetails = new DamageDetails(){
            TypeEffectiveness = effectiveness,
            Critical = critical,
            isFainted = false
        };

        float attack = (skill.SkillBase.Category == SkillCategory.SPECIAL) ? attacker.SuperAttack : attacker.Attack;
        float defense = (skill.SkillBase.Category == SkillCategory.SPECIAL) ? attacker.SuperDefense : attacker.Defense;

        float modifiers = Random.Range(0.85f, 1f) * effectiveness * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * skill.SkillBase.Power * ((float)attack / defense) + 2;

        int damage = Mathf.FloorToInt(d * modifiers);
        UpdateHP(damage);

        return damageDetails;
    }

    public void UpdateHP(int damage) {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        HpChanged = true;
    }

    public void SetStatus(ConditionID conditionId) {
        if(Status != null) return;
        Status = ConditionsDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{PkmBase.Name} {Status.StartMessage}");
    }

    public void CureStatus() {
        Status = null;
    }

    public Skill GetRandomSkill() {
        int r = Random.Range(0, Skills.Count);
        return Skills[r];
    }

    public bool OnBeforeSkill() {
        if(Status?.OnBeforeSkill != null) return Status.OnBeforeSkill(this);
        return true;
    }

    public void OnAfterTurn() {
        Status?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver() {
        ResetStatBoosts();
    }
}

public class DamageDetails {
    public bool isFainted { get; set; }

    public float Critical { get; set; }

    public float TypeEffectiveness { get; set; }
}
