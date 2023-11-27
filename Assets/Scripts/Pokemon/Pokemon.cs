using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon {
    PokemonBase pkmBase;
    int level;

    public List<Skill> Skills { get; set; }
    public int HP;

    public Pokemon(PokemonBase pBase, int pLevel) {
        pkmBase = pBase;
        level = pLevel;
        HP = pBase.HealthPoints;

        // generate skill
        Skills = new List<Skill>();
        foreach (var act in pkmBase.LearnableSkills) {
            if (act.Level <= level) {
                Skills.Add(new Skill(act.Base));
            }

            if(Skills.Count >= 4) break;
        }
    }

    public int Attack {
        get { return Mathf.FloorToInt((pkmBase.Attack * level) / 100f) + 5; }
    }

    public int Defense {
        get { return Mathf.FloorToInt((pkmBase.Defense * level) / 100f) + 5; }
    }

    public int SuperAttack {
        get { return Mathf.FloorToInt((pkmBase.SuperAttack * level) / 100f) + 5; }
    }

    public int SuperDefense {
        get { return Mathf.FloorToInt((pkmBase.SuperDefense * level) / 100f) + 5; }
    }

    public int Speed {
        get { return Mathf.FloorToInt((pkmBase.Speed * level) / 100f) + 5; }
    }

    public int MaximumHp {
        get { return Mathf.FloorToInt((pkmBase.HealthPoints * level) / 100f) + 10; }
    }
}
