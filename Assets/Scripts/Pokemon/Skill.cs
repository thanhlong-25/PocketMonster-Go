using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {
    public SkillBase SkillBase { get; set; }
    public int timeCanUse { get; set; }

    public Skill(SkillBase pBase) {
        SkillBase = pBase;
        timeCanUse = pBase.TimesCanUse;
    }
}
