using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject {
   [SerializeField] string name;
   [TextArea]
   [SerializeField] string description;
   [SerializeField] Sprite front;
   [SerializeField] Sprite back;
   [SerializeField] PokemonType type_1;
   [SerializeField] PokemonType type_2;
   [SerializeField] Sprite type_1_sprite;
   [SerializeField] Sprite type_2_sprite;

   // Base Stats
   [SerializeField] int maxHp;
   [SerializeField] int atk;
   [SerializeField] int def;
   [SerializeField] int spAtk;
   [SerializeField] int spDef;
   [SerializeField] int spd;

   [SerializeField] List<LearnableSkill> learnableSkills;

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public Sprite BackSprite {
        get { return back; }
    }

    public Sprite FrontSprite {
        get { return front; }
    }

    public Sprite Type_1_Sprite {
        get { return type_1_sprite; }
    }

    public Sprite Type_2_Sprite {
        get { return type_2_sprite; }
    }

    public PokemonType Type_1 {
        get { return type_1; }
    }

    public PokemonType Type_2 {
        get { return type_2; }
    }

    public int MaxHp {
        get { return maxHp; }
    }

    public int Attack {
        get { return atk; }
    }

    public int Defense {
        get { return def; }
    }

    public int SuperAttack {
        get { return spAtk; }
    }

    public int SuperDefense {
        get { return spDef; }
    }

    public int Speed {
        get { return spd; }
    }

    public List<LearnableSkill> LearnableSkills {
        get { return learnableSkills; }
    }
}

[System.Serializable]
public class LearnableSkill {
    [SerializeField] SkillBase action;
    [SerializeField] int level;

    public SkillBase Base {
        get { return action; }
    }

    public int Level {
        get { return level; }
    }
}

public enum PokemonType {
    NONE
    , NORMAL
    , FIRE
    , WATER
    , ELECTRIC
    , GRASS
    , ICE
    , FIGHTING
    , POISON
    , GROUND
    , FLYING
    , PSYCHIC
    , BUG
    , ROCK
    , GHOST
    , DRAGON
    , DARK
    , STEEL
    , FAIRY
}