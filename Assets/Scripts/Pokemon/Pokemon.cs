using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon {
    public PokemonBase PkmBase { get; set; }
    public int Level { get; set; }

    public List<Skill> Skills { get; set; }
    public int HP;

    public Pokemon(PokemonBase pBase, int pLevel) {
        PkmBase = pBase;
        Level = pLevel;
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

    public bool TakeDamage(Skill skill, Pokemon attacker) {
        float modifiers = Random.Range(0.85f, 1f);
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * skill.SkillBase.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if(HP <= 0) {
            HP = 0;
            return true;
        }
        return false;
    }

    public Skill GetRandomSkill() {
        int r = Random.Range(0, Skills.Count);
        return Skills[r];
    }
}
