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
   [SerializeField] bool alwaysHits;
   [SerializeField] int timesCanUse; // số lượt dùng
   [SerializeField] SkillAnimationBase skillAnimationBase;
   [SerializeField] SkillCategory category;
   [SerializeField] SkillEffect effect;
   [SerializeField] List<SecondaryEffect> secondaryEffects;
   [SerializeField] SkillTarget target;


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

    public bool AlwaysHits {
        get { return alwaysHits; }
    }

    public SkillAnimationBase SkillAnimationBase {
        get { return skillAnimationBase; }
    }

    public SkillCategory Category {
        get { return category; }
    }

    public SkillEffect Effect {
        get { return effect; }
    }

    public List<SecondaryEffect> SecondaryEffects {
        get { return secondaryEffects; }
    }

    public SkillTarget Target {
        get { return target; }
    }
}

[System.Serializable]
public class SkillEffect {
    [SerializeField] List<StatBoost> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<StatBoost> Boosts {
        get { return boosts; }
    }

    public ConditionID Status {
        get { return status; }
    }

    public ConditionID VolatileStatus {
        get { return volatileStatus; }
    }
}

[System.Serializable]
public class SecondaryEffect : SkillEffect {
    [SerializeField] int chance;
    [SerializeField] SkillTarget target;

    public int Chance {
        get { return chance; }
    }

    public SkillTarget Target {
        get { return target; }
    }
}

public enum SkillCategory {
    PHYSICAL,
    SPECIAL,
    STATUS
}

public enum SkillTarget {
    FOE,
    SELF
}


[System.Serializable]
public class StatBoost {
    public Stat stat;
    public int boost;
}
