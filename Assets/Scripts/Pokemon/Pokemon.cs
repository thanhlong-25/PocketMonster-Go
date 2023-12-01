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

    public void Init() {
        HP = MaxHp;

        // generate skill
        Skills = new List<Skill>();
        foreach (var act in PkmBase.LearnableSkills) {
            if (act.Level <= Level) {
                Skills.Add(new Skill(act.Base));
            }

            if(Skills.Count >= 4) break;
        }
    }

    public int Attack {
        get { return Mathf.FloorToInt((PkmBase.Attack * Level) / 100f) + 5; }
    }

    public int Defense {
        get { return Mathf.FloorToInt((PkmBase.Defense * Level) / 100f) + 5; }
    }

    public int SuperAttack {
        get { return Mathf.FloorToInt((PkmBase.SuperAttack * Level) / 100f) + 5; }
    }

    public int SuperDefense {
        get { return Mathf.FloorToInt((PkmBase.SuperDefense * Level) / 100f) + 5; }
    }

    public int Speed {
        get { return Mathf.FloorToInt((PkmBase.Speed * Level) / 100f) + 5; }
    }

    public int MaxHp {
        get { return Mathf.FloorToInt((PkmBase.MaxHp * Level) / 100f) + 10; }
    }

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

        float attack = (skill.SkillBase.IsSpecial) ? attacker.SuperAttack : attacker.Attack;
        float defense = (skill.SkillBase.IsSpecial) ? attacker.SuperDefense : attacker.Defense;

        float modifiers = Random.Range(0.85f, 1f) * effectiveness * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * skill.SkillBase.Power * ((float)attack / defense) + 2;
        
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if(HP <= 0) {
            HP = 0;
            damageDetails.isFainted = true;
        }
        return damageDetails;
    }

    public Skill GetRandomSkill() {
        int r = Random.Range(0, Skills.Count);
        return Skills[r];
    }
}

public class DamageDetails {
    public bool isFainted { get; set; }

    public float Critical { get; set; }

    public float TypeEffectiveness { get; set; }
}
