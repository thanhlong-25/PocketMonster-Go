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
   [SerializeField] Sprite sprite_3D;

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

    public Sprite Sprite_3D {
        get { return sprite_3D; }
    }

    public List<LearnableSkill> LearnableSkills {
        get { return learnableSkills; }
    }
}

[System.Serializable]
public class LearnableSkill {
    [SerializeField] SkillBase skillBase;
    [SerializeField] int level;

    public SkillBase Base {
        get { return skillBase; }
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

public enum Stat {
    ATTACK,
    DEFENSE,
    SUPER_ATTACK,
    SUPER_DEFENSE,
    SPEED
}

public class TypeChart {
    static float[][] chart = {
        //Defense               Nor   Fir   Wat   Ele   Gra   Ice   Fig   Poi   Gro   Fly   Psy   Bug   Roc   Gho   Dra   Dar  Ste    Fai
        // Attack
        /*Normal*/  new float[] {1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 0,    1f,   1f,   0.5f, 1f},
        /*Fire*/    new float[] {1f,   0.5f, 0.5f, 1f,   2f,   2f,   1f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   0.5f, 1f,   2f,   1f},
        /*Water*/   new float[] {1f,   2f,   0.5f, 1f,   0.5f, 1f,   1f,   1f,   2f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f,   1f,   1f},
        /*Electric*/new float[] {1f,   1f,   2f,   0.5f, 0.5f, 1f,   1f,   1f,   0f,   2f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,   1f},
        /*Grass*/   new float[] {1f,   0.5f, 2f,   1f,   0.5f, 1f,   1f,   0.5f, 2f,   0.5f, 1f,   0.5f, 2f,   1f,   0.5f, 1f,   0.5f, 1f},
        /*Ice*/     new float[] {1f,   0.5f, 0.5f, 1f,   2f,   0.5f, 1f,   1f,   2f,   2f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f},
        /*Fighting*/new float[] {2f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 1f,   0.5f, 0.5f, 0.5f, 2f,   0f,   1f,   2f,   2f,   0.5f},
        /*Poison*/  new float[] {1f,   1f,   1f,   1f,   2f,   1f,   1f,   0.5f, 0.5f, 1f,   1f,   1f,   0.5f, 0.5f, 1f,   1f,   0f,   2f},
        /*Ground*/  new float[] {1f,   2f,   1f,   2f,   0.5f, 1f,   1f,   2f,   1f,   0f,   1f,   0.5f, 2f,   1f,   1f,   1f,   2f,   1f},
        /*Flying*/  new float[] {1f,   1f,   1f,   0.5f, 2f,   1f,   2f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f,   1f,   0.5f, 1f},
        /*Psychic*/ new float[] {1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   1f,   1f,   0.5f, 1f,   1f,   1f,   1f,   0f,   0.5f, 1f},
        /*Bug*/     new float[] {1f,   0.5f, 1f,   1f,   2f,   1f,   0.5f, 0.5f, 1f,   0.5f, 2f,   1f,   1f,   0.5f, 1f,   2f,   0.5f, 0.5f},
        /*Rock*/    new float[] {1f,   2f,   1f,   1f,   1f,   2f,   0.5f, 1f,   0.5f, 2f,   1f,   2f,   1f,   1f,   1f,   1f,   0.5f, 1f},
        /*Ghost*/   new float[] {0f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,   2f,   1f,   0.5f, 1f,   1f},
        /*Dragon*/  new float[] {1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   1f,   2f,   1f,   0.5f, 0f},
        /*Dark*/    new float[] {1f,   1f,   1f,   1f,   1f,   1f,   0.5f, 1f,   1f,   1f,   2f,   1f,   1f,   2f,   1f,   0.5f, 1f,   0.5f},
        /*Steel*/   new float[] {1f,   0.5f, 0.5f, 0.5f, 1f,   2f,   1f,   1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f,   1f,   0.5f, 2f},
        /*Fairy*/   new float[] {1f,   0.5f, 1f,   1f,   1f,   1f,   2f,   0.5f, 1f,   1f,   1f,   1f,   1f,   1f,   2f,   2f,   0.5f, 1f},
    };

    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType) {
        if(attackType == PokemonType.NONE || defenseType == PokemonType.NONE) {
            return 1;
        }

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}