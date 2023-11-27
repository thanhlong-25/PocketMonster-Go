using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {
    public SkillBase SkillBase { get; set; }
    public int turnTime { get; set; }

    public Skill(SkillBase pBase) {
        SkillBase = pBase;
        turnTime = pBase.TimesCanUse;
    }
}
