using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Pokemon/Create new skill")]
public class SkillBase : ScriptableObject {
   [SerializeField] string name;
   [TextArea]
   [SerializeField] string description;
   [SerializeField] PokemonType type;
   [SerializeField] int power;
   [SerializeField] int accuracy; // chính xác
   [SerializeField] int timesCanUse; // số lượt dùng
   [SerializeField] SkillAnimationBase skillAnimationBase;

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public PokemonType Type {
        get { return type; }
    }

    public int Power {
        get { return power; }
    }

    public int Accuracy {
        get { return accuracy; }
    }

    public int TimesCanUse {
        get { return timesCanUse; }
    }

    public SkillAnimationBase SkillAnimationBase {
        get { return skillAnimationBase; }
    }

    public bool IsSpecial {
        get {
            if (type == PokemonType.FIRE
                || type == PokemonType.WATER
                || type == PokemonType.GRASS
                || type == PokemonType.DRAGON
                || type == PokemonType.ICE
                || type == PokemonType.ELECTRIC
            ) {
                return true;
            }

            return false;
        }
    }
}
