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
}
